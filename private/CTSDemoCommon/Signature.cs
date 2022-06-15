// <copyright file="Signature.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoCommon
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// Signature
    /// </summary>
    public class Signature
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// DisplayName
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// SignatureCaptureTime
        /// </summary>
        public DateTime SignatureCaptureTime { get; set; }

        /// <summary>
        /// Region
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// SignatureData
        /// </summary>
        public string SignatureData { get; set; }

        /// <summary>
        /// IsChecked
        /// </summary>
        [JsonIgnore]
        public bool IsChecked { get; set; }
    }
}
