// <copyright file="SignatureHelper.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoCommon
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// SignatureHelper
    /// </summary>
    public static class SignatureHelper
    {
        /// <summary>
        /// GetSignatureAsync
        /// </summary>
        /// <param name="region">region</param>
        /// <param name="subscriptionKey">subscriptionKey</param>
        /// <param name="language">language</param>
        /// <param name="filePath">filePath</param>
        /// <param name="enableTranscription">enableTranscription</param>
        /// <returns>Signature</returns>
        public static async Task<Signature> GetSignatureAsync(Region region, string subscriptionKey, string language, string filePath, bool enableTranscription = false)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"File {filePath} doesn't exist!");
            }

            var url = GetTargetUri(region, language, enableTranscription);

            using var client = new HttpClient();
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var content = new MultipartFormDataContent();

            content.Add(
                new StreamContent(fileStream)
                {
                    Headers =
                    {
                                ContentLength = fileStream.Length,
                                ContentType = new MediaTypeHeaderValue("audio/wav")
                    }
                },
                "File",
                "voice.wav");

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            var response = await client.PostAsync(url, content).ConfigureAwait(false);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var jobj = JObject.Parse(responseContent);
            var signatureData = jobj["Signature"].ToString();

            return new Signature()
            {
                Region = region.Name,
                SignatureCaptureTime = DateTime.Now,
                SignatureData = signatureData
            };
        }

        /// <summary>
        /// GetTargetUri
        /// </summary>
        /// <param name="region">region</param>
        /// <param name="language">language</param>
        /// <param name="enableTranscription">enableTranscription</param>
        /// <returns>target uri</returns>
        private static string GetTargetUri(Region region, string language, bool enableTranscription)
        {
            return $"https://signature.{region.Hostname}/api/v1/Signature/GenerateVoiceSignatureFromFormData?language={language}&enableTranscription={enableTranscription}";
        }
    }
}
