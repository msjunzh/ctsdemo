// <copyright file="SignatureWindow.xaml.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoWPF.Views
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using CTSDemoCommon;
    using CTSDemoWPF.ViewModels;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for SignatureView.xaml
    /// </summary>
    public partial class SignatureWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignatureWindow"/> class.
        /// </summary>
        public SignatureWindow()
        {
            this.InitializeComponent();
            this.Owner = Application.Current.MainWindow;
        }

        /// <summary>
        /// HandleDoubleClick
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var signature = ((ListViewItem)sender).DataContext as Signature;
            if (SignatureNameMessageBox.GetName((signature.Name, signature.DisplayName), out var newName))
            {
                signature.Name = newName.name;
                signature.DisplayName = newName.displayName;
                ((SignatureViewModel)this.DataContext).SaveSignatureChanges();
            }
        }

        /// <summary>
        /// DeleteButton_Click
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (SignatureViewModel)this.DataContext;
            var items = viewModel.GetSelectedSignatures().ToList();
            if (items.Count > 0)
            {
                var result = MessageBox.Show($"Do you want to delete the profiles of the following people? {string.Join(' ', items.Select(i => $"\"{i.Name}\""))}", "Confirmation", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    viewModel.DeleteSignatures(items);
                }
            }
        }

        /// <summary>
        /// AddButton_Click
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (SignatureViewModel)this.DataContext;
            if (string.IsNullOrEmpty(viewModel.SubscriptionKey))
            {
                MessageBox.Show($"Please add the subscription of this region \"{viewModel.Region}\" first.");
                return;
            }

            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
#pragma warning disable VSTHRD002
                var signature = SignatureHelper.GetSignatureAsync(viewModel.Region, viewModel.SubscriptionKey, "en-us", dialog.FileName, false).Result;
#pragma warning restore VSTHRD002

                if (SignatureNameMessageBox.GetName((null, null), out var newName))
                {
                    signature.Name = newName.name;
                    signature.DisplayName = newName.displayName;
                    ((SignatureViewModel)this.DataContext).AddSignature(signature);
                }
            }
        }

        /// <summary>
        /// CloseButton_Click
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
