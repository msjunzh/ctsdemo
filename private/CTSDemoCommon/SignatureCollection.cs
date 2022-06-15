// <copyright file="SignatureCollection.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoCommon
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection.Metadata.Ecma335;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// SignatureCollection
    /// </summary>
    public class SignatureCollection : List<Signature>, ILoadableType
    {
        /// <summary>
        /// FilePath
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Load
        /// </summary>
        /// <returns>Loaded SignatureCollection</returns>
        public static SignatureCollection Load()
        {
            return FileHelper.LoadFromFile<SignatureCollection>("sig.txt");
        }

        /// <summary>
        /// Save operation
        /// </summary>
        public void Save()
        {
            var jsonContent = JsonConvert.SerializeObject(this);
            File.WriteAllText(this.FilePath, jsonContent);
        }

        /// <summary>
        /// GetSignaturesGivenRegion
        /// </summary>
        /// <param name="region">region</param>
        /// <returns>IEnumerable of signatures</returns>
        public List<Signature> GetSignaturesGivenRegion(Region region)
        {
            return this.Where(s => s.Region == region.Name).ToList();
        }

        /// <summary>
        /// GetSelectedSignaturesGivenRegion
        /// </summary>
        /// <param name="region">region</param>
        /// <returns>IEnumerable of signatures</returns>
        public IEnumerable<Signature> GetSelectedSignaturesGivenRegion(Region region)
        {
            return this.Where(s => s.Region == region.Name && s.IsChecked);
        }

        /// <summary>
        /// DeleteSignatures
        /// </summary>
        /// <param name="signatures">signatures to delete</param>
        public void DeleteSignatures(IEnumerable<Signature> signatures)
        {
            bool needSave = false;
            foreach (var signature in signatures)
            {
                this.Remove(signature);
                needSave = true;
            }

            if (needSave)
            {
                this.Save();
            }
        }

        /// <summary>
        /// AddSignature
        /// </summary>
        /// <param name="signature">signature to add</param>
        public void AddSignature(Signature signature)
        {
            this.Add(signature);
            this.Save();
        }
    }
}
