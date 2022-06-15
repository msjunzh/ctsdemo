// <copyright file="ControlViewModel.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoWPF.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using CTSDemoCommon;
    using CTSDemoCommon.DataModel;
    using LiveTranscriptUi.Converters;
    using Microsoft.CognitiveServices.Speech.Transcription;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// ControlViewModel
    /// </summary>
    public class ControlViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// IsEnabled
        /// </summary>
        private bool isEnabled = true;

        /// <summary>
        /// startStopButtonText
        /// </summary>
        private string startStopButtonText = string.Empty;

        /// <summary>
        /// meetingStarted
        /// </summary>
        private bool meetingStarted = false;

        /// <summary>
        /// IsSingleChannel
        /// </summary>
        private bool isSingleChannel = true;

        /// <summary>
        /// currentMeetingInfo
        /// </summary>
        private string currentMeetingInfo = string.Empty;

        /// <summary>
        /// transcriptListViewModel
        /// </summary>
        private TranscriptListViewModel transcriptListViewModel;

        /// <summary>
        /// currentMeetingId
        /// </summary>
        private string currentMeetingId = string.Empty;

        /// <summary>
        /// footerText
        /// </summary>
        private string footerText = string.Empty;

        /// <summary>
        /// currentSelectedSignatureCount
        /// </summary>
        private int currentSelectedSignatureCount;

        /// <summary>
        /// offlineTranscriptionTask
        /// </summary>
        private Task offlineTranscriptionTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlViewModel"/> class.
        /// </summary>
        /// <param name="transcriptListViewModel">transcriptListViewModel</param>
        public ControlViewModel(TranscriptListViewModel transcriptListViewModel)
        {
            this.MeetingStarted = false;
            this.transcriptListViewModel = transcriptListViewModel;
            this.OnChannelsRadioButtonChecked = new CustomCommand(o => this.IsSingleChannel = o.ToString().Contains("single", StringComparison.OrdinalIgnoreCase));

            this.Region = Region.Regions[0];
            this.RefreshSubscriptionKey();
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Supported Meeting Languages
        /// </summary>
        public static IReadOnlyList<string> SupportedMeetingLanguages { get; } = new List<string> { "en-US", "zh-CN", "de-DE", "es-ES", "es-MX", "fr-CA", "fr-FR", "it-IT", "ja-JP", "ko-KR", "pt-BR" };

        /// <summary>
        /// Region
        /// </summary>
        public Region Region { get; set; }

        /// <summary>
        /// subscription key
        /// </summary>
        public string SubscriptionKey { get; private set; }

        /// <summary>
        /// SubscriptionKeyCollection
        /// </summary>
        public SubscriptionKeyCollection SubscriptionKeyCollection { get; private set; } = SubscriptionKeyCollection.Load();

        /// <summary>
        /// MeetingStarted
        /// </summary>
        public bool MeetingStarted
        {
            get
            {
                return this.meetingStarted;
            }

            set
            {
                this.meetingStarted = value;
                this.StartStopButtonText = this.meetingStarted ? $"Stop" : "Start";
            }
        }

        /// <summary>
        /// MeetingLanguage
        /// </summary>
        public string MeetingLanguage { get; set; } = SupportedMeetingLanguages.FirstOrDefault();

        /// <summary>
        /// IsSingleChannel
        /// </summary>
        public bool IsSingleChannel
        {
            get
            {
                return this.isSingleChannel;
            }

            set
            {
                if (this.isSingleChannel != value)
                {
                    this.isSingleChannel = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// OnChannelsRadioButtonChecked
        /// </summary>
        public ICommand OnChannelsRadioButtonChecked { get; }

        /// <summary>
        /// OnEnvironmentsRadioButtonChecked
        /// </summary>
        public ICommand OnEnvironmentsRadioButtonChecked { get; }

        /// <summary>
        /// Signatures
        /// </summary>
        public SignatureCollection Signatures { get; } = SignatureCollection.Load();

        /// <summary>
        /// Propertybag
        /// </summary>
        public Dictionary<string, object> PropertyBag { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// FooterText
        /// </summary>
        public string FooterText
        {
            get
            {
                return this.footerText;
            }

            set
            {
                if (this.footerText != value)
                {
                    this.footerText = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// StartStopButtonText
        /// </summary>
        public string StartStopButtonText
        {
            get
            {
                return this.startStopButtonText;
            }

            set
            {
                if (this.startStopButtonText != value)
                {
                    this.startStopButtonText = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// CurrentSelectedSignatureCount
        /// </summary>
        public int CurrentSelectedSignatureCount
        {
            get
            {
                return this.currentSelectedSignatureCount;
            }

            set
            {
                if (this.currentSelectedSignatureCount != value)
                {
                    this.currentSelectedSignatureCount = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// IsEnabled
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                if (this.isEnabled != value)
                {
                    this.isEnabled = value;
                    this.NotifyPropertyChanged();
                    this.NotifyPropertyChanged("IsDisabled");
                }
            }
        }

        /// <summary>
        /// IsDisabled
        /// </summary>
        public bool IsDisabled
        {
            get
            {
                return !this.isEnabled;
            }
        }

        /// <summary>
        /// StartStopButtonClicked
        /// </summary>
        public void StartStopButtonClicked()
        {
            if (this.MeetingStarted)
            {
                this.transcriptListViewModel.MeetingManager.StopMeeting();
                Log.LogText("Meeting {0} stopped.", this.currentMeetingId);
                AliasToImagePathConverter.GeneratedAlias.Clear();
            }
            else
            {
                this.currentMeetingId = this.CreateMeetingId();
                var meetingInfo = new MeetingInfo(
                    this.Region,
                    this.SubscriptionKey,
                    this.MeetingLanguage,
                    this.IsSingleChannel,
                    this.currentMeetingId,
                    this.Signatures.GetSelectedSignaturesGivenRegion(this.Region),
                    OperationMode.Realtime);

                foreach (var pair in this.PropertyBag)
                {
                    meetingInfo.PropertyBag.Add(pair.Key, pair.Value);
                }

                this.transcriptListViewModel.MeetingManager.StartMeeting(
                    meetingInfo,
                    includingPlaybackChannelInSingleChannel: true);

                this.UpdateMeetingInfoAndTrace();
            }

            this.MeetingStarted = !this.MeetingStarted;
        }

        /// <summary>
        /// TranscribeOfflineFile
        /// </summary>
        /// <param name="fileName">fileName</param>
        public void TranscribeOfflineFile(string fileName)
        {
            this.IsEnabled = false;
            this.offlineTranscriptionTask = Task.Factory.StartNew(
                async () =>
                {
                    var waitTask = new TaskCompletionSource<int>();
                    var list = new List<ConversationTranscriptionResult>();
                    var manager = new MeetingManager();

                    manager.SessionStopped += (s, e) =>
                    {
                        waitTask.SetResult(0);
                    };

                    this.currentMeetingId = this.CreateMeetingId();
                    var meetingInfo = new MeetingInfo(
                        this.Region,
                        this.SubscriptionKey,
                        this.MeetingLanguage,
                        this.IsSingleChannel,
                        this.currentMeetingId,
                        this.Signatures.GetSelectedSignaturesGivenRegion(this.Region),
                        OperationMode.Async,
                        fileName);

                    foreach (var pair in this.PropertyBag)
                    {
                        meetingInfo.PropertyBag.Add(pair.Key, pair.Value);
                    }

                    manager.StartMeeting(meetingInfo);

                    this.UpdateMeetingInfoAndTrace();
                    await waitTask.Task.ConfigureAwait(false);

                    Log.LogText("started pulling results");

                    while (true)
                    {
                        try
                        {
                            var url = $"https://transcribe.{this.Region.Hostname}/api/v1/offlinetask?&id={this.currentMeetingId}";
                            using var client = new HttpClient();
                            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.SubscriptionKey);
                            var result = await client.GetAsync(url).ConfigureAwait(false);
                            if (result.IsSuccessStatusCode)
                            {
                                var resultStr = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                                var obj = JObject.Parse(resultStr);
                                var status = int.Parse(obj["processingStatus"].ToString());
                                if (status < 2)
                                {
                                    Log.LogText("result is back with status " + status);
                                    Thread.Sleep(5000);
                                    continue;
                                }

                                var ctsRst = JsonConvert.DeserializeObject<CTSResponse>(resultStr);
                                var utteranceList = ctsRst.Transcriptions.ToList();

                                this.transcriptListViewModel.UpdateUtterances(DateTime.Now, utteranceList);

                                Log.LogText("Meeting {0} finished.", this.currentMeetingId);
                                this.IsEnabled = true;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.LogText(ex.ToString());
                        }

                        Thread.Sleep(1000);
                    }

                    // TODO: update the pull logic and waiting logic/UX
                },
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.Default);
        }

        /// <summary>
        /// Close operation
        /// </summary>
        public void Close()
        {
            this.transcriptListViewModel.MeetingManager.StopMeeting();
        }

        /// <summary>
        /// UpdateCurrentSelectedSignatureCount
        /// </summary>
        public void UpdateCurrentSelectedSignatureCount()
        {
            this.CurrentSelectedSignatureCount = this.Signatures.GetSelectedSignaturesGivenRegion(this.Region).Count();
        }

        /// <summary>
        /// ClearButtonClicked
        /// </summary>
        public void ClearButtonClicked()
        {
            this.transcriptListViewModel.Utterances.Clear();
        }

        /// <summary>
        /// GetSignatureViewModel
        /// </summary>
        /// <returns>generated signature view model from the current region</returns>
        public SignatureViewModel GetSignatureViewModel()
        {
            return new SignatureViewModel(this.Signatures, this.Region, this.SubscriptionKey);
        }

        /// <summary>
        /// GetSubscriptionKeyViewModel
        /// </summary>
        /// <returns>generated signature view model from the current region</returns>
        public SubscriptionKeyViewModel GetSubscriptionKeyViewModel()
        {
            return new SubscriptionKeyViewModel(this);
        }

        /// <summary>
        /// RefreshSubscriptionKey
        /// </summary>
        public void RefreshSubscriptionKey()
        {
            this.SubscriptionKey = this.SubscriptionKeyCollection.GetKey(this.Region);
        }

        /// <summary>
        /// NotifyPropertyChanged
        /// </summary>
        /// <param name="propertyName">propertyName</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// CreateMeetingId
        /// </summary>
        /// <returns>meeting id</returns>
        private string CreateMeetingId()
        {
            return string.Join("-", "demo", DateTime.Now.ToString("yyMMdd-HHmmss"), Guid.NewGuid().ToString().Substring(0, 8));
        }

        /// <summary>
        /// UpdateMeetingInfo
        /// </summary>
        private void UpdateMeetingInfoAndTrace()
        {
            this.currentMeetingInfo = string.Join(
                    " ",
                    this.Region.ToString().ToUpper(CultureInfo.CurrentCulture),
                    this.MeetingLanguage,
                    this.IsSingleChannel ? "single" : "multiple",
                    string.Join(" ", this.PropertyBag.Select(p => p.Key + "_" + p.Value)));

            this.FooterText = this.currentMeetingId;
            Log.LogText("Meeting {0} started. {1}", this.currentMeetingId, this.currentMeetingInfo);
        }
    }
}
