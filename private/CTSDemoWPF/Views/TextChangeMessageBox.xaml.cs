// <copyright file="TextChangeMessageBox.xaml.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoWPF.Views
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using CTSDemoCommon;

    /// <summary>
    /// Interaction logic for TextChangeMessageBox.xaml
    /// </summary>
    public partial class TextChangeMessageBox : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextChangeMessageBox"/> class.
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="idName">idName</param>
        /// <param name="idValue">idValue</param>
        /// <param name="valueName">valueName</param>
        /// <param name="oldValue">oldValue</param>
        public TextChangeMessageBox(string title, string idName, string idValue, string valueName, string oldValue)
        {
            this.InitializeComponent();

            this.Title = title;
            this.Owner = Application.Current.MainWindow;
            this.idNameLabel.Content = idName;
            this.idValueLabel.Content = idValue;
            this.valueNameLabel.Content = valueName;
            this.valueTextBox.Text = oldValue ?? string.Empty;
        }

        /// <summary>
        /// NewValue
        /// </summary>
        public string NewValue
        {
            get
            {
                return this.valueTextBox.Text;
            }
        }

        /// <summary>
        /// Get new key
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="keyName">keyName</param>
        /// <param name="keyValue">keyValue</param>
        /// <param name="valueName">valueName</param>
        /// <param name="oldValue">oldValue</param>
        /// <param name="newValue">newValue</param>
        /// <returns>if the message box was closed successfully</returns>
        public static bool GetNewValue(string title, string keyName, string keyValue, string valueName, string oldValue, out string newValue)
        {
            var subscriptionKeyMessageBox = new TextChangeMessageBox(title, keyName, keyValue, valueName, oldValue);

            var result = subscriptionKeyMessageBox.ShowDialog();
            if (result == true)
            {
                newValue = subscriptionKeyMessageBox.NewValue;
                return true;
            }

            newValue = null;
            return false;
        }

        /// <summary>
        /// OkButton_Click
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Window_Loaded
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.valueTextBox.Focus();
            this.valueTextBox.CaretIndex = this.valueTextBox.Text.Length;
        }
    }
}
