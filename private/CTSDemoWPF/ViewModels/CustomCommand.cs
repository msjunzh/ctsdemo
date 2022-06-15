// <copyright file="CustomCommand.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoWPF.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Input;

    /// <summary>
    /// CustomCommand
    /// </summary>
    public class CustomCommand : ICommand
    {
        /// <summary>
        /// action
        /// </summary>
        private readonly Action<object> action;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomCommand"/> class.
        /// </summary>
        /// <param name="action">action</param>
        public CustomCommand(Action<object> action)
        {
            this.action = action;
        }

#pragma warning disable CS0067
        /// <summary>
        /// CanExecuteChanged
        /// </summary>
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        /// <inheritdoc/>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <inheritdoc/>
        public void Execute(object parameter)
        {
            this.action(parameter);
        }
    }
}
