// <copyright file="SubscriptionKeyViewModel.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoWPF.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Text;
    using CTSDemoCommon;

    /// <summary>
    /// SubscriptionKeyViewModel
    /// </summary>
    public class SubscriptionKeyViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// controlViewModel
        /// </summary>
        private ControlViewModel controlViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionKeyViewModel"/> class.
        /// </summary>
        /// <param name="controlViewModel">controlViewModel</param>
        public SubscriptionKeyViewModel(ControlViewModel controlViewModel)
        {
            this.controlViewModel = controlViewModel;
            this.SubscriptionKeyCollection = this.controlViewModel.SubscriptionKeyCollection;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// subscriptionKeyCollection
        /// </summary>
        public SubscriptionKeyCollection SubscriptionKeyCollection { get; private set; }

        /// <summary>
        /// Save SubscriptionKeyCollection and trigger refresh
        /// </summary>
        public void SaveAndRefresh()
        {
            this.SubscriptionKeyCollection.Save();
            this.controlViewModel.RefreshSubscriptionKey();
            this.NotifyPropertyChanged("SubscriptionKeyCollection");
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
