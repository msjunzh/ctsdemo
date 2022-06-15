// <copyright file="UtteranceView.xaml.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoWPF.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using CTSDemoCommon;

    /// <summary>
    /// Interaction logic for UtteranceView.xaml
    /// </summary>
    public partial class UtteranceView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UtteranceView"/> class.
        /// </summary>
        public UtteranceView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// SpeakerPhoto_MouseDown
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void SpeakerPhoto_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                var viewModel = (UtteranceViewModel)this.DataContext;
                if (viewModel.Speaker != MeetingManager.UnidentifiedSpeakerId &&
                    TextChangeMessageBox.GetNewValue("Edit speaker name", "Id:", viewModel.Speaker, "Name:", viewModel.DisplayName, out var newDisplayName))
                {
                    viewModel.UpdateDisplayName(newDisplayName);
                }
            }
        }
    }
}
