// <copyright file="TranscriptListViewModel.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoWPF
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using CTSDemoCommon;
    using CTSDemoCommon.DataModel;
    using CTSDemoWPF.Views;
    using LiveTranscriptUi.Converters;
    using Microsoft.CognitiveServices.Speech;
    using Microsoft.CognitiveServices.Speech.Transcription;

    /// <summary>
    /// TranscriptListViewModel
    /// </summary>
    public class TranscriptListViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// MaxUtterancesToShowOnScreen
        /// </summary>
        private const int MaxUtterancesToShowOnScreen = 1000;

        /// <summary>
        /// finishedUtterances
        /// </summary>
        private readonly HashSet<string> finishedUtterances = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// UIContext
        /// </summary>
        private SynchronizationContext uiContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranscriptListViewModel"/> class.
        /// </summary>
        public TranscriptListViewModel()
        {
            this.uiContext = SynchronizationContext.Current;

            this.MeetingManager.Transcribing += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizingSpeech)
                {
                    this.UpdateUtterance(this.MeetingManager.MeetingStartTime, new TranscriptionItem(e.Result), true);
                }
            };

            this.MeetingManager.Transcribed += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    this.UpdateUtterance(this.MeetingManager.MeetingStartTime, new TranscriptionItem(e.Result), false);
                }
            };
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Utterances
        /// </summary>
        public ObservableCollection<UtteranceViewModel> Utterances { get; } = new ObservableCollection<UtteranceViewModel>();

        /// <summary>
        /// Utterances
        /// </summary>
        public ObservableCollection<CaptionViewModel> Captions { get; } = new ObservableCollection<CaptionViewModel>();

        /// <summary>
        /// MeetingManager
        /// </summary>
        public MeetingManager MeetingManager { get; } = new MeetingManager();

        /// <summary>
        /// UpdateDisplayName
        /// </summary>
        /// <param name="speakerId">speakerId</param>
        /// <param name="displayName">displayName</param>
        public void UpdateDisplayName(string speakerId, string displayName)
        {
            this.MeetingManager.UpdateDisplayName(speakerId, displayName);
            AliasToImagePathConverter.GeneratedAlias.Remove(speakerId);
            foreach (var utterance in this.Utterances.Where(u => u.Speaker == speakerId))
            {
                utterance.DisplayName = displayName;
            }

            this.NotifyPropertyChanged("Utterances");
        }

        /// <summary>
        /// CopyTranscriptToClipBoard
        /// </summary>
        public void CopyTranscriptToClipBoard()
        {
            var text = string.Join("\r\n", this.Utterances.Select(u => $"{u.Time} {u.DisplayName}:\t{u.Text}"));
            Clipboard.SetText(text);
        }

        /// <summary>
        /// UpdateUtterances
        /// </summary>
        /// <param name="startTime">startTime</param>
        /// <param name="results">results</param>
        public void UpdateUtterances(DateTime startTime, List<TranscriptionItem> results)
        {
            results.Sort((x, y) => Math.Sign(x.OffsetInTicks - y.OffsetInTicks));
            foreach (var result in results)
            {
                this.UpdateUtterance(startTime, result, false);
            }
        }

        /// <summary>
        /// UpdateUtterance
        /// </summary>
        /// <param name="startTime">startTime</param>
        /// <param name="result">result</param>
        /// <param name="isPartial">isPartial</param>
        private void UpdateUtterance(DateTime startTime, TranscriptionItem result, bool isPartial)
        {
#pragma warning disable VSTHRD001
            this.uiContext.Send(
                x =>
                {
                    lock (this.Utterances)
                    {
                        if (!isPartial)
                        {
                            this.finishedUtterances.Add(result.UtteranceId);
                        }
                        else if (this.finishedUtterances.Contains(result.UtteranceId))
                        {
                            return;
                        }

                        if (isPartial)
                        {
                            var currentActiveCaption = this.Captions.LastOrDefault(s => s.Id == result.UtteranceId);
                            if (currentActiveCaption == null)
                            {
                                var captionModel = new CaptionViewModel(result.UtteranceId)
                                {
                                    Text = result.Text,
                                    Time = startTime + TimeSpan.FromTicks(result.OffsetInTicks),
                                    IsActiveUtterance = true
                                };

                                this.Captions.Add(captionModel);
                            }
                            else
                            {
                                currentActiveCaption.Text = result.Text;
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(result.Text))
                        {
                            var offsetTime = startTime + TimeSpan.FromTicks(result.OffsetInTicks);
                            var utteranceBeforeTheCurrentOne = this.Utterances.LastOrDefault(u => u.Subutterances.First().time <= offsetTime);
                            if (utteranceBeforeTheCurrentOne != null
                            && utteranceBeforeTheCurrentOne.Speaker != MeetingManager.UnidentifiedSpeakerId
                            && utteranceBeforeTheCurrentOne.Speaker == result.UserId
                            && utteranceBeforeTheCurrentOne.Text.Length < 200)
                            {
                                utteranceBeforeTheCurrentOne.AddNewText(offsetTime, result.Text);
                            }
                            else
                            {
                                var utteranceAfterTheCurrentOne = this.Utterances.FirstOrDefault(u => u.Subutterances.First().time > offsetTime);
                                if (utteranceAfterTheCurrentOne != null
                                && utteranceAfterTheCurrentOne.Speaker != MeetingManager.UnidentifiedSpeakerId
                                && utteranceAfterTheCurrentOne.Speaker == result.UserId
                                && utteranceAfterTheCurrentOne.Text.Length < 200)
                                {
                                    utteranceAfterTheCurrentOne.AddNewText(offsetTime, result.Text);
                                }
                                else
                                {
                                    var utteranceViewModel = new UtteranceViewModel(this, result.UserId, this.MeetingManager.GetDisplayName(result.UserId), offsetTime, result.Text);

                                    if (utteranceBeforeTheCurrentOne == null)
                                    {
                                        this.Utterances.Insert(0, utteranceViewModel);
                                    }
                                    else
                                    {
                                        var splitModel = utteranceBeforeTheCurrentOne.SplitBasedOnTimestamp(offsetTime);
                                        var indexToInsert = this.Utterances.IndexOf(utteranceBeforeTheCurrentOne) + 1;
                                        this.Utterances.Insert(indexToInsert, utteranceViewModel);
                                        if (splitModel != null)
                                        {
                                            this.Utterances.Insert(indexToInsert + 1, splitModel);
                                        }
                                    }
                                }
                            }
                        }

                        if (this.Utterances.Count > MaxUtterancesToShowOnScreen)
                        {
                            this.Utterances.RemoveAt(0);
                        }
                    }
                },
                null);
#pragma warning restore VSTHRD001
        }

        /// <summary>
        /// NotifyPropertyChanged
        /// </summary>
        /// <param name="propertyName">propertyName</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
