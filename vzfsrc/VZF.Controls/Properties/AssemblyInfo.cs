﻿using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("VZF.Controls")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Tiny Gecko")]
[assembly: AssemblyProduct("VZF")]
[assembly: AssemblyCopyright("Copyright © 2006-2012 Yet Another Forum.NET")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
// [assembly: CLSCompliant(true)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("7a2d4440-346c-4dd5-b8c9-5b05fe0e8248")]

// Version information for an assembly consists of the following four values:
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.9.6.2")]
[assembly: AssemblyFileVersion("1.9.6.2")]

#if (!COMPACT_FRAMEWORK)
[assembly: AllowPartiallyTrustedCallers]
#endif

#if !NCRUNCH
[assembly: AssemblyDelaySign(false)]
// [assembly: AssemblyKeyFile("..\\vzf2013.pfx")]
#endif