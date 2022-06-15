// <copyright file="MeetingManager.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoCommon
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using CTSDemoCommon.DataModel;
    using Microsoft.CognitiveServices.Speech;
    using Microsoft.CognitiveServices.Speech.Audio;
    using Microsoft.CognitiveServices.Speech.Transcription;

    /// <summary>
    /// Meeting manager class
    /// </summary>
    public class MeetingManager
    {
        /// <summary>
        /// UnidentifiedSpeakerId
        /// </summary>
        public const string UnidentifiedSpeakerId = "Unidentified";

        /// <summary>
        /// lock obj
        /// </summary>
        private readonly object lockObj = new object();

        /// <summary>
        /// transcriptionTaskCompletionSource
        /// </summary>
        private TaskCompletionSource<int> transcriptionTaskCompletionSource = null;

        /// <summary>
        /// name mapping dictionary
        /// </summary>
        private Dictionary<string, string> nameMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Transcribing event
        /// </summary>
        public event EventHandler<ConversationTranscriptionEventArgs> Transcribing;

        /// <summary>
        /// Transcribed event
        /// </summary>
        public event EventHandler<ConversationTranscriptionEventArgs> Transcribed;

        /// <summary>
        /// Cancelled event
        /// </summary>
        public event EventHandler<ConversationTranscriptionCanceledEventArgs> Canceled;

        /// <summary>
        /// Session stopped event
        /// </summary>
        public event EventHandler<SessionEventArgs> SessionStopped;

        /// <summary>
        /// Start time
        /// </summary>
        public DateTime MeetingStartTime { get; private set; }

        /// <summary>
        /// Stop meeting
        /// </summary>
        public void StopMeeting()
        {
            lock (this.lockObj)
            {
                if (this.transcriptionTaskCompletionSource != null)
                {
                    this.transcriptionTaskCompletionSource.SetResult(0);
                    this.transcriptionTaskCompletionSource = null;
                }
            }
        }

        /// <summary>
        /// Get display name
        /// </summary>
        /// <param name="name">alias</param>
        /// <returns>display name</returns>
        public string GetDisplayName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || !this.nameMapping.TryGetValue(name, out var displayName))
            {
                return name;
            }

            return displayName;
        }

        /// <summary>
        /// Update display name
        /// </summary>
        /// <param name="speakerId">speakerId</param>
        /// <param name="displayName">display name</param>
        public void UpdateDisplayName(string speakerId, string displayName)
        {
            if (!string.IsNullOrEmpty(speakerId))
            {
                this.nameMapping[speakerId] = displayName;
            }
        }

        /// <summary>
        /// Start meeting method
        /// </summary>
        /// <param name="meetingInfo">meetingInfo</param>
        /// <param name="includingPlaybackChannelInSingleChannel">includingPlaybackChannelInSingleChannel</param>
        public void StartMeeting(
            MeetingInfo meetingInfo,
            bool includingPlaybackChannelInSingleChannel = false)
        {
            if (meetingInfo == null)
            {
                throw new ArgumentNullException(nameof(meetingInfo));
            }

            bool existingTranscriptionTask = true;
            if (this.transcriptionTaskCompletionSource == null)
            {
                lock (this.lockObj)
                {
                    if (this.transcriptionTaskCompletionSource == null)
                    {
                        this.transcriptionTaskCompletionSource = new TaskCompletionSource<int>();
                        existingTranscriptionTask = false;
                    }
                }
            }

            if (existingTranscriptionTask)
            {
                throw new Exception("There's already a transcription task going on");
            }

            SpeechConfig config = string.IsNullOrEmpty(meetingInfo.CustomEndpointPath)
                ? SpeechConfig.FromSubscription(meetingInfo.SubscriptionKey, meetingInfo.Region.Name.ToLowerInvariant())
                : SpeechConfig.FromEndpoint(new Uri(meetingInfo.CustomEndpointPath), meetingInfo.SubscriptionKey);
            config.SpeechRecognitionLanguage = meetingInfo.Language;

            config.SetProperty("ConversationTranscriptionInRoomAndOnline", "true");
            config.SetProperty("DifferentiateGuestSpeakers", "true");

            if (!meetingInfo.IsSingleChannel)
            {
                config.SetProperty("DeviceGeometry", "Circular6+1");
                config.SetProperty("SelectedGeometry", "Raw");
            }

            if (meetingInfo.OperationMode == OperationMode.Async)
            {
                config.SetProperty("SPEECH-TransmitLengthBeforeThrottleMs", "36000000");
            }

            // Create an audio stream from a wav file or from the default microphone if you want to stream live audio from the supported devices
            // Replace with your own audio file name and Helper class which implements AudioConfig using PullAudioInputStreamCallback
            var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            RecordingStream recordingStream = null;

            if (meetingInfo.InputWavFilePath != null)
            {
                audioConfig = AudioConfig.FromWavFileInput(meetingInfo.InputWavFilePath);
            }
            else if (meetingInfo.IsSingleChannel)
            {
                if (meetingInfo.OperationMode == OperationMode.Async)
                {
                    throw new Exception("This meeting was marked as async but without wave file input.");
                }

                recordingStream = new RecordingStream(includingPlaybackChannelInSingleChannel);
                audioConfig = AudioConfig.FromStreamInput(recordingStream);
            }

            _ = Task.Factory.StartNew(
                async () =>
                {
                    using (var conversation = await Conversation.CreateConversationAsync(config, meetingInfo.MeetingId).ConfigureAwait(false))
                    {
                        // Create a conversation transcriber using audio stream input
                        using (var conversationTranscriber = new ConversationTranscriber(audioConfig))
                        {
                            await conversationTranscriber.JoinConversationAsync(conversation).ConfigureAwait(false);

                            var connection = Connection.FromRecognizer(conversationTranscriber);
                            if (!meetingInfo.IsSingleChannel)
                            {
                                connection.SetMessageProperty("speech.config", "multiGuestAudioOnly", $"\"{true}\"");
                            }
                            else
                            {
                                connection.SetMessageProperty("speech.config", "MicSpec", $"\"1_0_0\"");
                                connection.SetMessageProperty("speech.config", "DisableReferenceChannel", $"\"{true}\"");
                            }

                            if (meetingInfo.OperationMode == OperationMode.Async)
                            {
                                connection.SetMessageProperty("speech.config", "transcriptionMode", $"\"Async\"");
                            }

                            // Subscribe to events
                            conversationTranscriber.Transcribing += (s, e) =>
                            {
                                this.Transcribing?.Invoke(s, e);
                            };

                            conversationTranscriber.Transcribed += (s, e) =>
                            {
                                this.Transcribed?.Invoke(s, e);
                            };

                            conversationTranscriber.Canceled += (s, e) =>
                            {
                                this.Canceled?.Invoke(s, e);

                                if (e.Reason == CancellationReason.Error)
                                {
                                    this.StopMeeting();
                                }
                            };

                            conversationTranscriber.SessionStarted += (s, e) =>
                            {
                                Console.WriteLine("\nSession started event.");
                            };

                            conversationTranscriber.SessionStopped += (s, e) =>
                            {
                                this.StopMeeting();
                                this.SessionStopped?.Invoke(s, e);
                            };

                            this.nameMapping.Clear();

                            if (meetingInfo.Signatures != null)
                            {
                                foreach (var signature in meetingInfo.Signatures)
                                {
                                    var speaker = Participant.From(signature.Name, meetingInfo.Language, signature.SignatureData);
                                    await conversation.AddParticipantAsync(speaker).ConfigureAwait(false);

                                    Log.LogText($"Added speaker {signature.Name}.");
                                    if (!string.IsNullOrWhiteSpace(signature.DisplayName))
                                    {
                                        this.nameMapping[signature.Name] = signature.DisplayName;
                                    }
                                }
                            }

                            // Starts transcribing of the conversation. Uses StopTranscribingAsync() to stop transcribing when all participants leave.
                            await conversationTranscriber.StartTranscribingAsync().ConfigureAwait(false);

                            // Waits for completion.
                            // Use Task.WaitAny to keep the task rooted.
                            await this.transcriptionTaskCompletionSource.Task.ConfigureAwait(false);

                            // Stop transcribing the conversation.
                            await conversationTranscriber.StopTranscribingAsync().ConfigureAwait(false);
                            recordingStream?.Close();

                            Console.WriteLine("transcription stopped.");
                        }
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            this.MeetingStartTime = DateTime.Now;
        }
    }
}
