// <copyright file="ResampleWaveProvider.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoCommon.WaveProviders
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using NAudio.Wave;

    /// <summary>
    /// ResampleWaveProvider
    /// </summary>
    public class ResampleWaveProvider : IWaveProvider
    {
        /// <summary>
        /// BufferSize
        /// </summary>
        private const int BufferSize = 16000 * 2 * 360;

        /// <summary>
        /// buffer
        /// </summary>
        private RingBuffer buffer = new RingBuffer(BufferSize);

        /// <summary>
        /// inputWaveProvider
        /// </summary>
        private RegularWaveProvider inputWaveProvider;

        /// <summary>
        /// isClosed
        /// </summary>
        private bool isClosed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResampleWaveProvider"/> class.
        /// </summary>
        /// <param name="inputWaveformat">inputWaveformat</param>
        /// <param name="outputWaveformat">outputWaveformat</param>
        public ResampleWaveProvider(WaveFormat inputWaveformat, WaveFormat outputWaveformat)
        {
            this.WaveFormat = outputWaveformat;
            this.inputWaveProvider = new RegularWaveProvider(inputWaveformat);
            _ = Task.Factory.StartNew(
                () =>
                {
                    using (var resampler = new MediaFoundationResampler(this.inputWaveProvider, outputWaveformat))
                    {
                        byte[] readBuffer = new byte[65536];
                        while (!this.isClosed)
                        {
                            var count = resampler.Read(readBuffer, 0, readBuffer.Length);
                            if (count == 0)
                            {
                                break;
                            }

                            this.buffer.Write(readBuffer, 0, count);
                        }
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        /// <summary>
        /// WaveFormat
        /// </summary>
        public WaveFormat WaveFormat { get; }

        /// <inheritdoc/>
        public int Read(byte[] buffer, int offset, int count)
        {
            return this.buffer.Read(buffer, offset, count);
        }

        /// <summary>
        /// Write operation
        /// </summary>
        /// <param name="buffer">buffer</param>
        /// <param name="offset">offset</param>
        /// <param name="count">count</param>
        public void Write(byte[] buffer, int offset, int count)
        {
            this.inputWaveProvider.Write(buffer, offset, count);
        }

        /// <summary>
        /// Close operation
        /// </summary>
        public void Close()
        {
            this.isClosed = true;
            this.buffer.Close();
        }
    }
}
