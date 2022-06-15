// <copyright file="RegularWaveProvider.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoCommon.WaveProviders
{
    using NAudio.Wave;

    /// <summary>
    /// RegularWaveProvider
    /// </summary>
    public class RegularWaveProvider : IWaveProvider
    {
        /// <summary>
        /// Buffer size
        /// </summary>
        private const int BufferSize = 16000 * 2 * 360;

        /// <summary>
        /// Internal buffer
        /// </summary>
        private RingBuffer buffer = new RingBuffer(BufferSize);

        /// <summary>
        /// Initializes a new instance of the <see cref="RegularWaveProvider"/> class.
        /// </summary>
        /// <param name="format">wave format</param>
        public RegularWaveProvider(WaveFormat format)
        {
            this.WaveFormat = format;
        }

        /// <inheritdoc/>
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
            this.buffer.Write(buffer, offset, count);
        }

        /// <summary>
        /// Close operation
        /// </summary>
        public void Close()
        {
            this.buffer.Close();
        }
    }
}
