// <copyright file="SubscriptionKeyItem.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoCommon
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// SubscriptionKeyItem
    /// </summary>
    public class SubscriptionKeyItem : INotifyPropertyChanged
    {
        /// <summary>
        /// region
        /// </summary>
        private string region;

        /// <summary>
        /// subscriptionKey
        /// </summary>
        private string subscriptionKey;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Region
        /// </summary>
        public string Region
        {
            get
            {
                return this.region;
            }

            set
            {
                if (this.region != value)
                {
                    this.region = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// SubscriptionKey
        /// </summary>
        public string SubscriptionKey
        {
            get
            {
                return this.subscriptionKey;
            }

            set
            {
                if (this.subscriptionKey != value)
                {
                    this.subscriptionKey = value;
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
