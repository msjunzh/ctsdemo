// <copyright file="RingBuffer.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoCommon
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// RingBuffer
    /// </summary>
    public class RingBuffer
    {
        /// <summary>
        /// internalBuffer
        /// </summary>
        private readonly byte[] internalBuffer;

        /// <summary>
        /// bufferSize
        /// </summary>
        private readonly int bufferSize;

        /// <summary>
        /// closed
        /// </summary>
        private bool closed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="RingBuffer"/> class.
        /// </summary>
        /// <param name="size">initial buffer size</param>
        public RingBuffer(int size)
        {
            this.bufferSize = size;
            this.internalBuffer = new byte[size];
        }

        /// <summary>
        /// ReadOffset
        /// </summary>
        public long ReadOffset { get; private set; } = 0;

        /// <summary>
        /// WriteOffset
        /// </summary>
        public long WriteOffset { get; private set; } = 0;

        /// <summary>
        /// Write operation
        /// </summary>
        /// <param name="buffer">buffer</param>
        /// <param name="offset">offset</param>
        /// <param name="count">count</param>
        public void Write(byte[] buffer, int offset, int count)
        {
            if (!this.closed)
            {
                if (this.WriteOffset + count - this.ReadOffset > this.bufferSize)
                {
                    var message = $"Buffer exceeded. writeoffset: {this.WriteOffset}, readoffset: {this.ReadOffset}, count {count}, buffersize: {this.bufferSize}";
                    Log.LogText(message);
                    throw new Exception(message);
                }

                var realWriteIndex = this.WriteOffset % this.bufferSize;
                var currentWriteCount = Math.Min((int)(this.bufferSize - realWriteIndex), count);
                Array.Copy(buffer, offset, this.internalBuffer, realWriteIndex, currentWriteCount);
                this.WriteOffset += currentWriteCount;
                if (currentWriteCount != count)
                {
                    this.Write(buffer, offset + currentWriteCount, count - currentWriteCount);
                }
            }
        }

        /// <summary>
        /// Read operation
        /// </summary>
        /// <param name="buffer">buffer</param>
        /// <param name="offset">offset</param>
        /// <param name="count">count</param>
        /// <returns>read count</returns>
        public int Read(byte[] buffer, int offset, int count)
        {
            while (!this.closed && this.ReadOffset == this.WriteOffset)
            {
                Thread.Sleep(100);
            }

            if (this.closed)
            {
                return 0;
            }

            int countToReturn = Math.Min((int)(this.WriteOffset - this.ReadOffset), count);
            int realReadIndex = (int)(this.ReadOffset % this.bufferSize);
            int currentReadCount = Math.Min(this.bufferSize - realReadIndex, countToReturn);
            Array.Copy(this.internalBuffer, realReadIndex, buffer, offset, currentReadCount);
            if (currentReadCount != countToReturn)
            {
                Array.Copy(this.internalBuffer, 0, buffer, offset + currentReadCount, countToReturn - currentReadCount);
            }

            this.ReadOffset += countToReturn;
            return countToReturn;
        }

        /// <summary>
        /// Close operation
        /// </summary>
        public void Close()
        {
            this.closed = true;
        }
    }
}
