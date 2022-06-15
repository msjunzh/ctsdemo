// <copyright file="TranscriptionItem.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoCommon.DataModel
{
    using System;
    using Microsoft.CognitiveServices.Speech.Transcription;

    /// <summary>
    /// TranscriptionItem
    /// </summary>
    public class TranscriptionItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TranscriptionItem"/> class.
        /// </summary>
        public TranscriptionItem()
        {
            this.UtteranceId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TranscriptionItem"/> class.
        /// </summary>
        /// <param name="result">result</param>
        public TranscriptionItem(ConversationTranscriptionResult result)
        {
            this.UtteranceId = result.UtteranceId;
            this.UserId = result.UserId;
            this.Text = result.Text;
            this.OffsetInTicks = result.OffsetInTicks;
        }

        /// <summary>
        /// UtteranceId
        /// </summary>
        public string UtteranceId { get; }

        /// <summary>
        /// UserId
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// OffsetInTicks
        /// </summary>
        public long OffsetInTicks { get; set; }
    }
}
