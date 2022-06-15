// <copyright file="CaptionViewModel.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoWPF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Text;

    /// <summary>
    /// UtteranceViewModel
    /// </summary>
    public class CaptionViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// text
        /// </summary>
        private string text = string.Empty;

        /// <summary>
        /// time
        /// </summary>
        private DateTime time;

        /// <summary>
        /// isActiveUtterance
        /// </summary>
        private bool isActiveUtterance;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptionViewModel"/> class.
        /// </summary>
        /// <param name="id">id</param>
        public CaptionViewModel(string id)
        {
            this.Id = id;
            this.isActiveUtterance = true;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// PartialText
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value;
                this.NotifyPropertyChanged("Text");
            }
        }

        /// <summary>
        /// Datetime for last update
        /// </summary>
        public DateTime LastUpdateDatetime { get; set; } = DateTime.Now;

        /// <summary>
        /// Time
        /// </summary>
        public DateTime Time
        {
            get
            {
                return this.time;
            }

            set
            {
                if (this.time != value)
                {
                    this.time = value;
                }
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
        /// NotifyPropertyChanged
        /// </summary>
        /// <param name="propertyName">propertyName</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
