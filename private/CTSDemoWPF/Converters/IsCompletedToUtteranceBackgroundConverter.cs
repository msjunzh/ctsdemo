// <copyright file="IsCompletedToUtteranceBackgroundConverter.cs" company="Microsoft">
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
    /// IsCompletedToUtteranceBackgroundConverter
    /// </summary>
    public class IsCompletedToUtteranceBackgroundConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var defaultBrush = new SolidColorBrush(Color.FromRgb(68, 68, 68));
            var selectedBrush = new SolidColorBrush(Color.FromRgb(119, 119, 119));
            return (bool)value ? selectedBrush : defaultBrush;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
