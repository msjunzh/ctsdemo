// <copyright file="NAudioHelper.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoCommon
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NAudio.CoreAudioApi;

    /// <summary>
    /// NAudioHelper
    /// </summary>
    public static class NAudioHelper
    {
        /// <summary>
        /// GetMicrophoneList
        /// </summary>
        /// <returns>microphone list</returns>
        public static IEnumerable<(string friendlyName, string id)> GetMicrophoneList()
        {
            var enumerator = new MMDeviceEnumerator();
            return enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).Select(s => (s.FriendlyName, s.ID));
        }

        /// <summary>
        /// GetBestMicrophoneIndex
        /// </summary>
        /// <returns>best microphone index</returns>
        public static int GetBestMicrophoneIndex()
        {
            var list = GetMicrophoneList().ToList();
            var arrays = list.Where(l => l.friendlyName.Contains("Microphone Array", StringComparison.Ordinal)).ToList();
            if (arrays.Count == 0)
            {
                return 0;
            }

            var microphoneToUse = arrays.Where(l => l.friendlyName.Contains("Trident", StringComparison.Ordinal)).FirstOrDefault();
            if (microphoneToUse == default)
            {
                microphoneToUse = arrays[0];
            }

            return list.IndexOf(microphoneToUse);
        }
    }
}
