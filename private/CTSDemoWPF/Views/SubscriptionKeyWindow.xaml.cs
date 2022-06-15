// <copyright file="SubscriptionKeyWindow.xaml.cs" company="Microsoft">
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
    using CTSDemoWPF.ViewModels;

    /// <summary>
    /// Interaction logic for SubscriptionKeyWindow.xaml
    /// </summary>
    public partial class SubscriptionKeyWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionKeyWindow"/> class.
        /// </summary>
        public SubscriptionKeyWindow()
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
            var keyItem = ((ListViewItem)sender).DataContext as SubscriptionKeyItem;

            if (TextChangeMessageBox.GetNewValue("Edit subscription key", "Region:", keyItem.Region.ToString(), "Key:", keyItem.SubscriptionKey, out var newKey))
            {
                keyItem.SubscriptionKey = newKey;
                ((SubscriptionKeyViewModel)this.DataContext).SaveAndRefresh();
            }
        }
    }
}
