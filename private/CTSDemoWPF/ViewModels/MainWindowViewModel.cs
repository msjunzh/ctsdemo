// <copyright file="MainWindowViewModel.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoWPF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using CTSDemoCommon;
    using CTSDemoWPF.ViewModels;
    using Microsoft.CognitiveServices.Speech;

    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            this.TranscriptListViewModel = new TranscriptListViewModel();
            this.ControlViewModel = new ControlViewModel(this.TranscriptListViewModel);
        }

        /// <summary>
        /// PropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// TranscriptListViewModel
        /// </summary>
        public TranscriptListViewModel TranscriptListViewModel { get; set; }

        /// <summary>
        /// ControlViewModel
        /// </summary>
        public ControlViewModel ControlViewModel { get; set; }

        /// <summary>
        /// Close operation
        /// </summary>
        public void Close()
        {
            this.ControlViewModel.Close();
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
