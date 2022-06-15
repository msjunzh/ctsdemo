// <copyright file="FileHelper.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoCommon
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    /// <summary>
    /// FileHelper
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// FolderPath
        /// </summary>
        private static readonly string FolderPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

        /// <summary>
        /// LoadFromFile
        /// </summary>
        /// <typeparam name="T">type </typeparam>
        /// <param name="fileName">file name</param>
        /// <returns>loaded object</returns>
        public static T LoadFromFile<T>(string fileName) where T : ILoadableType, new()
        {
            var result = new T();
            var filePath = Path.Combine(FolderPath, fileName);
            if (File.Exists(filePath))
            {
                var text = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    try
                    {
                        result = JsonConvert.DeserializeObject<T>(text);
                    }
                    catch (JsonException ex)
                    {
                        Log.LogText($"JsonException: {ex.Message}");
                    }
                }
            }

            result.FilePath = filePath;
            return result;
        }
    }
}
