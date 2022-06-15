// <copyright file="SubscriptionKeyCollection.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoCommon
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// SubscriptionKeyCollection
    /// </summary>
    public class SubscriptionKeyCollection : ObservableCollection<SubscriptionKeyItem>, ILoadableType
    {
        /// <summary>
        /// FilePath
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Load
        /// </summary>
        /// <returns>Loaded SignatureCollection</returns>
        public static SubscriptionKeyCollection Load()
        {
            var collection = FileHelper.LoadFromFile<SubscriptionKeyCollection>("key.txt");
            foreach (var region in Region.Regions)
            {
                collection.GetKey(region);
            }

            return collection;
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
        /// Get Key
        /// </summary>
        /// <param name="region">region</param>
        /// <returns>key or null</returns>
        public string GetKey(Region region)
        {
            var item = this.Where(s => s.Region == region.Name).FirstOrDefault();
            if (item == null)
            {
                item = new SubscriptionKeyItem { Region = region.Name, SubscriptionKey = string.Empty };
                this.Add(item);
            }

            return item.SubscriptionKey;
        }

        /// <summary>
        /// Add or update the dictionary
        /// </summary>
        /// <param name="region">region</param>
        /// <param name="key">key</param>
        public void AddOrUpdate(Region region, string key)
        {
            var item = this.Where(s => s.Region == region.Name).FirstOrDefault();
            if (item == null)
            {
                this.Add(new SubscriptionKeyItem { Region = region.Name, SubscriptionKey = key });
            }
            else
            {
                item.SubscriptionKey = key;
            }

            this.Save();
        }
    }
}
