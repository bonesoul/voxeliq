/*
 * Copyright (C) 2011 - 2013 Voxeliq Engine - http://www.voxeliq.org - https://github.com/raistlinthewiz/voxeliq
 *
 * This program is free software; you can redistribute it and/or modify 
 * it under the terms of the Microsoft Public License (Ms-PL).
 */

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Engine.Platforms;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("VoxeliqEngine")]
[assembly: AssemblyProduct("VoxeliqEngine")]
[assembly: AssemblyDescription("A block engine.")]
[assembly: AssemblyCompany("Voxeliq Studios")]
[assembly: AssemblyCopyright("Copyright ©  2012 - 2013")]
[assembly: AssemblyTrademark("Voxeliq Studios")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type. Only Windows
// assemblies support COM.
[assembly: ComVisible(false)]

// On Windows, the following GUID is for the ID of the typelib if this
// project is exposed to COM. On other platforms, it unique identifies the
// title storage container when deploying this assembly to the device.
[assembly: Guid("dd94853a-9fd3-473a-bbbb-e343defe3450")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//

// Set the assembly version from VersionInfo.cs file.
[assembly: AssemblyVersion(VersionInfo.VersionPattern)]

[assembly: InternalsVisibleTo("EngineTests")] // let internals be visible to EngineTests project.