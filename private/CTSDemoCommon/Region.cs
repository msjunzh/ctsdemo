// <copyright file="Region.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace CTSDemoCommon
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Region
    /// </summary>
    public class Region
    {
        /// <summary>
        /// Regions
        /// </summary>
        public static readonly List<Region> Regions = new List<Region>();

        /// <summary>
        /// Initializes static members of the <see cref="Region"/> class.
        /// </summary>
        static Region()
        {
            var regionFile = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "region.txt");
            if (File.Exists(regionFile))
            {
                foreach (var line in File.ReadAllLines(regionFile))
                {
                    if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var strs = line.Split(new char[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);

                    Regions.Add(new Region(strs[0], strs.Length >= 2 ? strs[1] : null));
                }
            }
            else
            {
                Regions.Add(new Region("CentralUs", null));
                Regions.Add(new Region("EastUs", null));
                Regions.Add(new Region("EastAsia", null));
                Regions.Add(new Region("WestEurope", null));
                Regions.Add(new Region("WestUs", null));
            }
        }

        ////CTSDev,
        ////CTSPpe,
        ////CentralUs,
        ////EastAsia,
        ////EastUs,
        ////WestEurope,

        /// <summary>
        /// Initializes a new instance of the <see cref="Region"/> class.
        /// </summary>
        /// <param name="name">name of region</param>
        /// <param name="hostName">hostname for custom environment</param>
        public Region(string name, string hostName)
        {
            this.Name = name;
            this.Hostname = string.IsNullOrEmpty(hostName) ? $"{name}.cts.speech.microsoft.com" : hostName;
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Hostname
        /// </summary>
        public string Hostname { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
