// <copyright file="Log.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoCommon
{
    using System;
    using System.IO;

    /// <summary>
    /// Log class
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Log file to be used
        /// </summary>
        private static readonly string LogFile = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "log.txt");

        /// <summary>
        /// lock obj
        /// </summary>
        private static readonly object LockObj = new object();

        /// <summary>
        /// Log text
        /// </summary>
        /// <param name="format">string format</param>
        /// <param name="args">arguments</param>
        public static void LogText(string format, params object[] args)
        {
            lock (LockObj)
            {
                File.AppendAllText(LogFile, DateTime.Now.ToString("s") + ": " + string.Format(format, args) + "\n");
            }
        }
    }
}