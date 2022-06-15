// <copyright file="RecordingStream.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoCommon
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection.Metadata.Ecma335;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using CTSDemoCommon.WaveProviders;
    using Microsoft.CognitiveServices.Speech.Audio;
    using NAudio.Wave;
    using NAudio.Wave.SampleProviders;

    /// <summary>
    /// RecordingStream
    /// </summary>
    public class RecordingStream : PullAudioInputStreamCallback
    {
        /// <summary>
        /// OutputWaveFormat
        /// </summary>
        private static readonly WaveFormat OutputWaveFormat = new WaveFormat(16000, 16, 1);

        /// <summary>
        /// activeDevices
        /// </summary>
        private List<IWaveIn> activeDevices = new List<IWaveIn>();

        /// <summary>
        /// inputWaveProvider
        /// </summary>
        private RegularWaveProvider inputWaveProvider = null;

        /// <summary>
        /// outputWaveProvider
        /// </summary>
        private ResampleWaveProvider outputWaveProvider = null;

        /// <summary>
        /// isClosed
        /// </summary>
        private bool isClosed = false;

        /// <summary>
        /// mixer
        /// </summary>
        private MixedWaveProvider mixer = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordingStream"/> class.
        /// </summary>
        /// <param name="includingPlayback">includingPlayback</param>
        public RecordingStream(bool includingPlayback)
        {
            var waveInEvent = new WaveInEvent();
            waveInEvent.WaveFormat = OutputWaveFormat;
            waveInEvent.DeviceNumber = NAudioHelper.GetBestMicrophoneIndex();
            waveInEvent.DataAvailable += this.WaveInEvent_DataAvailable;
            this.inputWaveProvider = new RegularWaveProvider(waveInEvent.WaveFormat);

            this.activeDevices.Add(waveInEvent);

            this.mixer = new MixedWaveProvider(OutputWaveFormat);
            this.mixer.AddWaveProvider(this.inputWaveProvider);
            if (includingPlayback)
            {
                var playBackEvent = new WasapiLoopbackCapture();
                playBackEvent.DataAvailable += this.PlayBackEvent_DataAvailable;
                this.outputWaveProvider = new ResampleWaveProvider(playBackEvent.WaveFormat, OutputWaveFormat);
                this.activeDevices.Add(playBackEvent);
                this.mixer.AddWaveProvider(this.outputWaveProvider);
            }

            foreach (var device in this.activeDevices)
            {
                device.StartRecording();
            }

            if (includingPlayback)
            {
                this.PlayWhiteNoiseInBackground();
            }
        }

        /// <inheritdoc/>
        public override int Read(byte[] dataBuffer, uint size)
        {
            return this.mixer.Read(dataBuffer, 0, (int)size);
        }

        /// <inheritdoc/>
        public override void Close()
        {
            base.Close();
            this.Cleanup();
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.Cleanup();
        }

        /// <summary>
        /// PlayWhiteNoiseInBackground to start playback recording
        /// </summary>
        private void PlayWhiteNoiseInBackground()
        {
            _ = Task.Factory.StartNew(
                () =>
                {
                    var sine20Seconds = new SignalGenerator()
                    {
                        Gain = -1000.0,
                        Frequency = 0,
                        Type = SignalGeneratorType.White
                    }.Take(TimeSpan.FromSeconds(1000));
                    using (var wo = new WaveOutEvent())
                    {
                        wo.Init(sine20Seconds);
                        wo.Play();
                        while (wo.PlaybackState == PlaybackState.Playing && !this.isClosed)
                        {
                            Thread.Sleep(500);
                        }
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        /// <summary>
        /// PlayBackEvent_DataAvailable
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void PlayBackEvent_DataAvailable(object sender, WaveInEventArgs e)
        {
            this.outputWaveProvider.Write(e.Buffer, 0,  e.BytesRecorded);
        }

        /// <summary>
        /// WaveInEvent_DataAvailable
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void WaveInEvent_DataAvailable(object sender, WaveInEventArgs e)
        {
            this.inputWaveProvider.Write(e.Buffer, 0, e.BytesRecorded);
        }

        /// <summary>
        /// Cleanup
        /// </summary>
        private void Cleanup()
        {
            this.isClosed = true;
            foreach (var device in this.activeDevices)
            {
                device.Dispose();
            }

            this.inputWaveProvider?.Close();
            this.outputWaveProvider?.Close();
        }
    }
}
