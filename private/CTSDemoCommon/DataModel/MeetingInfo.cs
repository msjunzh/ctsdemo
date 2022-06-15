// <copyright file="MeetingInfo.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoCommon.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// MeetingInfo
    /// </summary>
    public class MeetingInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeetingInfo"/> class.
        /// </summary>
        /// <param name="region">region</param>
        /// <param name="subscriptionKey">subscriptionKey</param>
        /// <param name="language">language</param>
        /// <param name="isSingleChannel">isSingleChannel</param>
        /// <param name="meetingId">meetingId</param>
        /// <param name="signatures">signatures</param>
        /// <param name="operationMode">operationMode</param>
        /// <param name="inputWavFilePath">inputWavFilePath</param>
        public MeetingInfo(
            Region region,
            string subscriptionKey,
            string language,
            bool isSingleChannel,
            string meetingId,
            IEnumerable<Signature> signatures,
            OperationMode operationMode,
            string inputWavFilePath = null)
        {
            if (string.IsNullOrEmpty(subscriptionKey))
            {
                throw new ArgumentException("subscription key should not be null or empty");
            }

            if (string.IsNullOrEmpty(meetingId))
            {
                throw new ArgumentException("meetingId should not be null or empty");
            }

            if (string.IsNullOrEmpty(language))
            {
                throw new ArgumentException("language should not be null or empty");
            }

            this.Region = region;
            this.SubscriptionKey = subscriptionKey;
            this.Language = language;
            this.IsSingleChannel = isSingleChannel;
            this.MeetingId = meetingId;
            this.Signatures = signatures;
            this.OperationMode = operationMode;
            this.InputWavFilePath = inputWavFilePath;
        }

        /// <summary>
        /// Region
        /// </summary>
        public Region Region { get; }

        /// <summary>
        /// SubscriptionKey
        /// </summary>
        public string SubscriptionKey { get; }

        /// <summary>
        /// Language
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Is single channel
        /// </summary>
        public bool IsSingleChannel { get; }

        /// <summary>
        /// meeting id
        /// </summary>
        public string MeetingId { get; }

        /// <summary>
        /// signatures
        /// </summary>
        public IEnumerable<Signature> Signatures { get; }

        /// <summary>
        /// The operation mode of CTS
        /// </summary>
        public OperationMode OperationMode { get; }

        /// <summary>
        /// InputWavFilePath
        /// </summary>
        public string InputWavFilePath { get; }

        /// <summary>
        /// CustomEndpointPath
        /// </summary>
        public string CustomEndpointPath { get; set; } = null;

        /// <summary>
        /// Propertybag
        /// </summary>
        public Dictionary<string, object> PropertyBag { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    }
}
