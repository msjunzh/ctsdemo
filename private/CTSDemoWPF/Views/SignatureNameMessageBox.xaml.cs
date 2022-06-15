// <copyright file="SignatureNameMessageBox.xaml.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoWPF.Views
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for SignatureName.xaml
    /// </summary>
    public partial class SignatureNameMessageBox : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignatureNameMessageBox"/> class.
        /// aaa
        /// </summary>
        /// <param name="originalName">originalName</param>
        /// <param name="displayName">displayName</param>
        public SignatureNameMessageBox(string originalName, string displayName)
        {
            this.InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            if (!string.IsNullOrEmpty(originalName))
            {
                this.nameTextBox.Text = originalName;
            }

            if (!string.IsNullOrEmpty(displayName))
            {
                this.displayNameTextBox.Text = displayName;
            }
        }

        /// <summary>
        /// SignatureName
        /// </summary>
        public string SignatureName
        {
            get
            {
                return this.nameTextBox.Text;
            }
        }

        /// <summary>
        /// DisplayName
        /// </summary>
        public string DisplayName
        {
            get
            {
                return this.displayNameTextBox.Text;
            }
        }

        /// <summary>
        /// GetName
        /// </summary>
        /// <param name="original">original</param>
        /// <param name="newName">newName</param>
        /// <returns>if the message box was closed successfully</returns>
        public static bool GetName((string name, string displayName) original, out (string name, string displayName) newName)
        {
            var signatureNameMessageBox = new SignatureNameMessageBox(original.name, original.displayName);

            var sigNameResult = signatureNameMessageBox.ShowDialog();
            if (sigNameResult == true)
            {
                newName = (signatureNameMessageBox.SignatureName, signatureNameMessageBox.DisplayName);
                return true;
            }

            newName = (null, null);
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
            this.nameTextBox.Focus();
            this.nameTextBox.CaretIndex = this.nameTextBox.Text.Length;
        }
    }
}
