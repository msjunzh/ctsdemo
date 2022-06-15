// <copyright file="TranscriptListView.xaml.cs" company="Microsoft">
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
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for TranscriptListView.xaml
    /// </summary>
    public partial class TranscriptListView : UserControl
    {
        /// <summary>
        /// transcriptionAutoscroll
        /// </summary>
        private bool transcriptionAutoscroll = true;

        /// <summary>
        /// captionAutoscroll
        /// </summary>
        private bool captionAutoscroll = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranscriptListView"/> class.
        /// </summary>
        public TranscriptListView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// ScrollViewer_ScrollChanged
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // User scroll event : set or unset auto-scroll mode
            if (e.ExtentHeightChange == 0)
            {
                // Content unchanged : user scroll event
                if (this.TrancriptScrollViewer.VerticalOffset == this.TrancriptScrollViewer.ScrollableHeight)
                {
                    // Scroll bar is in bottom
                    // Set auto-scroll mode
                    this.transcriptionAutoscroll = true;
                }
                else
                {
                    // Scroll bar isn't in bottom
                    // Unset auto-scroll mode
                    this.transcriptionAutoscroll = false;
                }
            }

            // Content scroll event : auto-scroll eventually
            if (this.transcriptionAutoscroll && e.ExtentHeightChange != 0)
            {
                this.TrancriptScrollViewer.ScrollToVerticalOffset(this.TrancriptScrollViewer.ExtentHeight);
            }
        }

        /// <summary>
        /// CaptionScrollViewer_ScrollChanged
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void CaptionScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // User scroll event : set or unset auto-scroll mode
            if (e.ExtentHeightChange == 0)
            {
                // Content unchanged : user scroll event
                if (this.CaptionScrollViewer.VerticalOffset == this.CaptionScrollViewer.ScrollableHeight)
                {
                    // Scroll bar is in bottom
                    // Set auto-scroll mode
                    this.captionAutoscroll = true;
                }
                else
                {
                    // Scroll bar isn't in bottom
                    // Unset auto-scroll mode
                    this.captionAutoscroll = false;
                }
            }

            // Content scroll event : auto-scroll eventually
            if (this.captionAutoscroll && e.ExtentHeightChange != 0)
            {
                this.CaptionScrollViewer.ScrollToVerticalOffset(this.CaptionScrollViewer.ExtentHeight);
            }
        }
    }
}
