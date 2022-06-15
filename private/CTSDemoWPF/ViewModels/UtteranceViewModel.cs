// <copyright file="UtteranceViewModel.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoWPF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Windows.Controls;

    /// <summary>
    /// UtteranceViewModel
    /// </summary>
    public class UtteranceViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// speaker
        /// </summary>
        private string speaker = string.Empty;

        /// <summary>
        /// isActiveUtterance
        /// </summary>
        private bool isActiveUtterance;

        /// <summary>
        /// displayName
        /// </summary>
        private string displayName = string.Empty;

        /// <summary>
        /// TranscriptListViewModel
        /// </summary>
        private TranscriptListViewModel listViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="UtteranceViewModel"/> class.
        /// </summary>
        /// <param name="listViewModel">list view model</param>
        /// <param name="speakerId">speakerId</param>
        /// <param name="displayName">displayName</param>
        /// <param name="time">time of first utterance</param>
        /// <param name="text">text of first utterance</param>
        public UtteranceViewModel(TranscriptListViewModel listViewModel, string speakerId, string displayName, DateTime time, string text)
        {
            this.speaker = speakerId;
            this.displayName = displayName;
            this.isActiveUtterance = true;
            this.listViewModel = listViewModel;
            this.Subutterances.Add((time, text));
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Text
        /// </summary>
        public string Text
        {
            get
            {
                return string.Join(" ", this.Subutterances.Select(s => s.text));
            }
        }

        /// <summary>
        /// Speaker
        /// </summary>
        public string Speaker
        {
            get
            {
                return this.speaker;
            }

            set
            {
                if (this.speaker != value)
                {
                    this.speaker = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// DisplayName
        /// </summary>
        public string DisplayName
        {
            get
            {
                return this.displayName;
            }

            set
            {
                if (this.displayName != value)
                {
                    this.displayName = value;
                    this.NotifyPropertyChanged();
                }

                this.NotifyPropertyChanged("ImageKey");
            }
        }

        /// <summary>
        /// Datetime for last update
        /// </summary>
        public DateTime LastUpdateDatetime { get; set; } = DateTime.Now;

        /// <summary>
        /// ImageKey
        /// </summary>
        public string ImageKey => $"{this.Speaker}#{this.DisplayName}";

        /// <summary>
        /// Time
        /// </summary>
        public DateTime Time
        {
            get
            {
                return this.Subutterances.First().time;
            }
        }

        /// <summary>
        /// Time
        /// </summary>
        public string TimeDisplay
        {
            get
            {
                return this.Time.ToShortTimeString();
            }
        }

        /// <summary>
        /// IsActiveUtterance
        /// </summary>
        public bool IsActiveUtterance
        {
            get
            {
                return this.isActiveUtterance;
            }

            set
            {
                if (this.isActiveUtterance != value)
                {
                    this.isActiveUtterance = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Subutterances
        /// </summary>
        public List<(DateTime time, string text)> Subutterances { get; } = new List<(DateTime time, string text)>();

        /// <summary>
        /// AddNewText
        /// </summary>
        /// <param name="time">time</param>
        /// <param name="text">text</param>
        public void AddNewText(DateTime time, string text)
        {
            this.Subutterances.Add((time, text));
            this.Subutterances.Sort((x, y) => x.time.CompareTo(y.time));
            this.NotifyPropertyChanged("Text");
        }

        /// <summary>
        /// SplitBasedOnTimestamp
        /// </summary>
        /// <param name="time">time</param>
        /// <returns>splitted UtteranceModel or null</returns>
        public UtteranceViewModel SplitBasedOnTimestamp(DateTime time)
        {
            int index = -1;
            for (int i = 0; i < this.Subutterances.Count; i++)
            {
                if (this.Subutterances[i].time > time)
                {
                    index = i;
                    break;
                }
            }

            if (index < 0)
            {
                return null;
            }
            else
            {
                var result = new UtteranceViewModel(this.listViewModel, this.Speaker, this.DisplayName, this.Subutterances[index].time, this.Subutterances[index].text);
                for (int i = index + 1; i < this.Subutterances.Count; i++)
                {
                    result.AddNewText(this.Subutterances[i].time, this.Subutterances[i].text);
                }

                this.Subutterances.RemoveRange(index, this.Subutterances.Count - index);
                this.NotifyPropertyChanged("Text");
                return result;
            }
        }

        /// <summary>
        /// UpdateDisplayName
        /// </summary>
        /// <param name="newDisplayName">newDisplayName</param>
        public void UpdateDisplayName(string newDisplayName)
        {
            this.listViewModel.UpdateDisplayName(this.Speaker, newDisplayName);
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
