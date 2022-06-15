// <copyright file="IsCompletedToTransparencyConverter.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace LiveTranscriptUi.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// IsCompletedToTransparencyConverter
    /// </summary>
    public class IsCompletedToTransparencyConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isLastCompletedUtterance = (bool)value;
            Color c = (isLastCompletedUtterance ? Colors.White : Color.FromRgb(201, 201, 201));
            Brush brush = new SolidColorBrush(c)
            {
                Opacity = (isLastCompletedUtterance ? 1.0 : 0.4)
            };
            return brush;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
