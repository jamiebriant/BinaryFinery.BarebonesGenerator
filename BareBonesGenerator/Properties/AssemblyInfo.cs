// 
// A modification of FileGenReflector: http://filegenreflector.codeplex.com/
// Copyright (c) 2008 (?) Jason R Bock
// Released under the Microsoft Public License:  http://filegenreflector.codeplex.com/license
// Modifications: Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Permissions;

[assembly: AssemblyCopyright("Copyright © 2008")]
[assembly:
    AssemblyDescription(
        "Contains an add-in that can generate bare-bones source code for assemblies, modules, namespaces or individual types."
        )]
[assembly: AssemblyFileVersion("6.0.0.0")]
[assembly: AssemblyProduct("BareBonesGenerator")]
[assembly: AssemblyTitle("BareBonesGenerator")]
[assembly: AssemblyVersion("6.0.0.0")]
[assembly: CLSCompliant(false)]
[assembly: ComVisible(false)]
[assembly: NeutralResourcesLanguage("en-US")]
[assembly: ReflectionPermission(SecurityAction.RequestMinimum, Unrestricted = true)]