// <copyright file="ControlView.xaml.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoWPF.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using CTSDemoCommon;
    using CTSDemoWPF.ViewModels;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for ControlView.xaml
    /// </summary>
    public partial class ControlView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlView"/> class.
        /// </summary>
        public ControlView()
        {
            this.InitializeComponent();

            foreach (var region in Region.Regions)
            {
                this.regionComboBox.Items.Add(region);
            }

            this.regionComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// StartStopButton_Click
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (ControlViewModel)this.DataContext;

            if (string.IsNullOrEmpty(viewModel.MeetingLanguage))
            {
                MessageBox.Show("Please select a meeting language", "Invalid Settings");
                return;
            }

            if (string.IsNullOrEmpty(viewModel.SubscriptionKey))
            {
                if (TextChangeMessageBox.GetNewValue("Edit subscription key", "Region:", viewModel.Region.ToString(), "Key:", viewModel.SubscriptionKey, out var newKey))
                {
                    viewModel.SubscriptionKeyCollection.AddOrUpdate(viewModel.Region, newKey);
                    viewModel.RefreshSubscriptionKey();
                }
                else
                {
                    MessageBox.Show($"Please add the subscription key of this region \"{viewModel.Region}\" first.", "Invalid Settings");
                    return;
                }
            }

            viewModel.StartStopButtonClicked();
        }

        /// <summary>
        /// ClearButton_Click
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ((ControlViewModel)this.DataContext).ClearButtonClicked();
        }

        /// <summary>
        /// SignatureButton_Click
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void SignatureButton_Click(object sender, RoutedEventArgs e)
        {
            var signatureWindow = new SignatureWindow();
            var viewModel = ((ControlViewModel)this.DataContext).GetSignatureViewModel();
            signatureWindow.DataContext = viewModel;
            signatureWindow.ShowDialog();

            ((ControlViewModel)this.DataContext).UpdateCurrentSelectedSignatureCount();
        }

        /// <summary>
        /// ManageSubscriptionKeyButton_Click
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void ManageSubscriptionKeyButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = new SubscriptionKeyViewModel((ControlViewModel)this.DataContext);
            var subscriptionKeyWindow = new SubscriptionKeyWindow();
            subscriptionKeyWindow.DataContext = viewModel;
            subscriptionKeyWindow.ShowDialog();
        }

        /// <summary>
        /// LanguagesListBox_SelectionChanged
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">args</param>
        private void LanguagesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = (ControlViewModel)this.DataContext;
            viewModel.MeetingLanguage = string.Join(",", ((ListBox)sender).SelectedItems.Cast<string>());
        }

        /// <summary>
        /// RegionComboBox_SelectionChanged
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void RegionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var controlViewModel = (ControlViewModel)this.DataContext;
            if (controlViewModel != null)
            {
                controlViewModel.Region = (Region)this.regionComboBox.SelectedItem;
                controlViewModel.RefreshSubscriptionKey();
                controlViewModel.UpdateCurrentSelectedSignatureCount();
            }
        }

        /// <summary>
        /// SelectFileButton_Click
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var controlViewModel = (ControlViewModel)this.DataContext;
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var fileName = dialog.FileName;
                controlViewModel.TranscribeOfflineFile(fileName);
            }
        }
    }
}
