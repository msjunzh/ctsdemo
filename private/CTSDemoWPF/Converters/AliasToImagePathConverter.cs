// <copyright file="AliasToImagePathConverter.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace LiveTranscriptUi.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// AliasToImagePathConverter
    /// </summary>
    public class AliasToImagePathConverter : IValueConverter
    {
        /// <summary>
        /// Hashset to track the generated alias
        /// </summary>
        public static readonly HashSet<string> GeneratedAlias = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// BaseFolder
        /// </summary>
        private static readonly string BaseFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Images");

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var imageKey = value as string;
            if (imageKey == null)
            {
                return null;
            }

            var alias = imageKey.Substring(0, imageKey.IndexOf('#', StringComparison.OrdinalIgnoreCase));
            var displayName = imageKey.Substring(alias.Length + 1);

            var path = Path.Combine(BaseFolder, alias + ".jpg");

            if (!File.Exists(path))
            {
                path = Path.Combine(BaseFolder, alias + "_generated.jpg");
                lock (GeneratedAlias)
                {
                    if (!GeneratedAlias.Contains(alias))
                    {
                        GeneratedAlias.Add(alias);
                        CreateImage(displayName, path);
                    }
                }
            }

            var imgTemp = new BitmapImage();
            imgTemp.BeginInit();
            imgTemp.CacheOption = BitmapCacheOption.OnLoad;
            imgTemp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            imgTemp.UriSource = new Uri(path);
            imgTemp.EndInit();
            return imgTemp;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create image and save
        /// </summary>
        /// <param name="displayName">fileName</param>
        /// <param name="filePath">path to be saved</param>
        private static void CreateImage(string displayName, string filePath)
        {
            var strs = displayName.Split(new char[] { ' ', '_' }, StringSplitOptions.RemoveEmptyEntries);
            var index = 0;
            var text = string.Empty;
            foreach (var s in strs)
            {
                text += s[0];
                if (++index >= 2)
                {
                    break;
                }
            }

            text = text.ToUpperInvariant();
            var font = new Font("Courier New", 42, FontStyle.Bold);
            const int ImageSize = 100;

            var random = new Random();
            var list = new List<int>();

            for (var i = 0; i < 3; i++)
            {
                list.Add(random.Next(128, 256));
            }

            list[random.Next(3)] = 0;

            var r = list[0];
            var g = list[1];
            var b = list[2];
            using var img = new Bitmap(ImageSize, ImageSize);
            using Graphics gfx = Graphics.FromImage(img);
            using SolidBrush brush = new SolidBrush(Color.FromArgb(r, g, b));
            gfx.FillRectangle(brush, 0, 0, ImageSize, ImageSize);
            gfx.DrawString(text, font, new SolidBrush(Color.White), new PointF(5, 20));

            var retryCount = 0;
            while (true)
            {
                try
                {
                    img.Save(filePath);
                    break;
                }
                catch (ExternalException)
                {
                    if (++retryCount >= 4)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
