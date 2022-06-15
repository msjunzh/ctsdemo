// <copyright file="CTSResponse.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoCommon.DataModel
{
    /// <summary>
    /// CTS Response
    /// </summary>
    public class CTSResponse
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Transcriptions
        /// </summary>
        public TranscriptionItem[] Transcriptions { get; set; }
    }
}
