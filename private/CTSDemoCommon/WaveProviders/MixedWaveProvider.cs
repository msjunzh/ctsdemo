// <copyright file="MixedWaveProvider.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoCommon.WaveProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NAudio.Wave;

    /// <summary>
    /// MixedWaveProvider
    /// </summary>
    public class MixedWaveProvider : IWaveProvider
    {
        /// <summary>
        /// BufferSize
        /// </summary>
        private const int BufferSize = 1024 * 256;

        /// <summary>
        /// lock obj
        /// </summary>
        private readonly object lockObj = new object();

        /// <summary>
        /// providers to mix together
        /// </summary>
        private List<IWaveProvider> providers = new List<IWaveProvider>();

        /// <summary>
        /// buffer position list
        /// </summary>
        private List<Position> positionList = new List<Position>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MixedWaveProvider"/> class.
        /// </summary>
        /// <param name="format">waveformat</param>
        public MixedWaveProvider(WaveFormat format)
        {
            this.WaveFormat = format;
        }

        /// <inheritdoc/>
        public WaveFormat WaveFormat { get; }

        /// <summary>
        /// AddWaveProvider
        /// </summary>
        /// <param name="provider">provider</param>
        public void AddWaveProvider(IWaveProvider provider)
        {
            lock (this.lockObj)
            {
                this.providers.Add(provider);
                this.positionList.Add(new Position());
            }
        }

        /// <inheritdoc/>
        public int Read(byte[] buffer, int offset, int count)
        {
            lock (this.lockObj)
            {
                for (int i = 0; i < this.providers.Count; i++)
                {
                    if (this.positionList[i].Count == 0)
                    {
                        this.positionList[i].Count = this.providers[i].Read(this.positionList[i].Buffer, 0, BufferSize);
                        this.positionList[i].Offset = 0;
                    }
                }

                var minCount = this.positionList.Min(p => p.Count);
                var byteToRead = Math.Min(count, minCount);

                for (int i = 0; i < byteToRead; i += 2)
                {
                    short sum = 0;
                    foreach (var p in this.positionList)
                    {
                        var v = BitConverter.ToInt16(p.Buffer, p.Offset + i);
                        sum += v;
                    }

                    var b = BitConverter.GetBytes(sum);
                    Array.Copy(b, 0, buffer, offset + i, 2);
                }

                foreach (var p in this.positionList)
                {
                    p.Count -= byteToRead;
                    p.Offset += byteToRead;
                }

                return byteToRead;
            }
        }

        /// <summary>
        /// Position
        /// </summary>
        private class Position
        {
            /// <summary>
            /// Buffer
            /// </summary>
            public byte[] Buffer { get; } = new byte[BufferSize];

            /// <summary>
            /// Offset
            /// </summary>
            public int Offset { get; set; } = 0;

            /// <summary>
            /// Count
            /// </summary>
            public int Count { get; set; } = 0;
        }
    }
}
