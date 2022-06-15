// <copyright file="SignatureViewModel.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoWPF.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using CTSDemoCommon;

    /// <summary>
    /// SignatureViewModel
    /// </summary>
    public class SignatureViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// signatureCollection
        /// </summary>
        private readonly SignatureCollection signatureCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignatureViewModel"/> class.
        /// </summary>
        /// <param name="signatureCollection">signatureCollection</param>
        /// <param name="region">region</param>
        /// <param name="subscriptionKey">subscriptionKey</param>
        public SignatureViewModel(SignatureCollection signatureCollection, Region region, string subscriptionKey)
        {
            this.signatureCollection = signatureCollection;
            this.Region = region;
            this.SubscriptionKey = subscriptionKey;
            this.RefreshSignatureList();
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Region
        /// </summary>
        public Region Region { get; private set; }

        /// <summary>
        /// SubscriptionKey
        /// </summary>
        public string SubscriptionKey { get; private set; }

        /// <summary>
        /// Signatures
        /// </summary>
        public ObservableCollection<Signature> Signatures { get; } = new ObservableCollection<Signature>();

        /// <summary>
        /// AddSignature
        /// </summary>
        /// <param name="signature">signature</param>
        public void AddSignature(Signature signature)
        {
            this.signatureCollection.AddSignature(signature);
            this.RefreshSignatureList();
        }

        /// <summary>
        /// GetSelectedSignatures
        /// </summary>
        /// <returns>signatures</returns>
        public IEnumerable<Signature> GetSelectedSignatures()
        {
            return this.signatureCollection.GetSelectedSignaturesGivenRegion(this.Region);
        }

        /// <summary>
        /// DeleteSignatures
        /// </summary>
        /// <param name="signatures">signatures to delete</param>
        public void DeleteSignatures(IEnumerable<Signature> signatures)
        {
            this.signatureCollection.DeleteSignatures(signatures);
            this.RefreshSignatureList();
        }

        /// <summary>
        /// SaveSignatureChanges
        /// </summary>
        public void SaveSignatureChanges()
        {
            this.signatureCollection.Save();
            this.RefreshSignatureList();
        }

        /// <summary>
        /// RefreshSignatureList
        /// </summary>
        public void RefreshSignatureList()
        {
            this.Signatures.Clear();
            foreach (var sig in this.signatureCollection.GetSignaturesGivenRegion(this.Region))
            {
                this.Signatures.Add(sig);
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
