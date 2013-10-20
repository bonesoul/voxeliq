/*
 * Voxeliq Engine, Copyright (C) 2011 - 2013 Int6 Studios - All Rights Reserved. - http://www.int6.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This file is part of Voxeliq Engine project. This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

namespace Engine.Platforms
{
    /// <summary>
    /// Version info.
    /// </summary>
    public static class VersionInfo
    {
        /// <summary>
        /// Main assemblies version pattern to be used in AssemblyInfo.cs.
        /// </summary>
        public const string VersionPattern = "0.2.0.*";

        /// <summary>
        /// Main assemblies version.
        /// </summary>
        public static string Version { get; private set; }

        static VersionInfo()
        {
            #if METRO
                Version = string.Format("{0}.{1}.{2}.{3}", Windows.ApplicationModel.Package.Current.Id.Version.Major,
                            Windows.ApplicationModel.Package.Current.Id.Version.Minor,
                            Windows.ApplicationModel.Package.Current.Id.Version.Build,
                            Windows.ApplicationModel.Package.Current.Id.Version.Revision);
            #elif WINPHONE7
                Version = System.Reflection.Assembly.GetExecutingAssembly().FullName.Split(',')[1].Split('=')[1];
            #else
                Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            #endif
        }
    }
}
