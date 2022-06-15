# Conversation transcription service demo
It'a s demo app for Microsoft Azure Speech Service - Conversation Transcription
https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/conversation-transcription

The signature wav only support 16K mono wav file. Use following command to convert wav files.
> ffmpeg -i [InputAudioSignatureFile] -acodec pcm_s16le -ac 1 -ar 16000 OutputWAV.wav
