// <copyright file="StringToIsEnabledConverter.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoWPF.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Windows.Data;

    /// <summary>
    /// StringToIsEnabledConverter
    /// </summary>
    public class StringToIsEnabledConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string val = System.Convert.ToString(value).ToUpper(CultureInfo.CurrentCulture);
            return !string.IsNullOrWhiteSpace(val);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
