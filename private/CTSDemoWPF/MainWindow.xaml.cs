// <copyright file="MainWindow.xaml.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoWPF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// isPresentMode
        /// </summary>
        private bool isPresentMode = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.KeyDown += this.MainWindow_KeyDown;
        }

        /// <summary>
        /// MainWindow_KeyDown
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.P && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
            {
                if (this.isPresentMode)
                {
                    this.controlView.Visibility = Visibility.Visible;
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                }
                else
                {
                    this.controlView.Visibility = Visibility.Collapsed;
                    this.WindowStyle = WindowStyle.None;
                }

                this.isPresentMode = !this.isPresentMode;
                this.Topmost = this.isPresentMode;
            }

            if (e.Key == Key.C && (Keyboard.Modifiers & (ModifierKeys.Control)) == ModifierKeys.Control)
            {
                ((TranscriptListViewModel)this.transcriptListView.DataContext).CopyTranscriptToClipBoard();
            }
        }

        /// <summary>
        /// Window_Deactivated
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (this.isPresentMode)
            {
                this.Topmost = true;
            }
        }

        /// <summary>
        /// Window_Closing
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((MainWindowViewModel)this.DataContext).Close();
        }
    }
}
