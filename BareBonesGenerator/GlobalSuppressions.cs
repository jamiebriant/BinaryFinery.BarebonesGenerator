// 
// A modification of FileGenReflector: http://filegenreflector.codeplex.com/
// Copyright (c) 2008 (?) Jason R Bock
// Released under the Microsoft Public License:  http://filegenreflector.codeplex.com/license
// Modifications: Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System.Diagnostics.CodeAnalysis;

[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "FileGenerator.AddIn")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "FileGenerator.AddIn.UI")]
[assembly:
    SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames",
        MessageId = "serviceProvider", Scope = "member",
        Target = "FileGenerator.AddIn.FileGeneratorPackage.#Load(System.IServiceProvider)")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "FileGenerator.AddIn.TextFormatter.#AllowProperties")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Scope = "type",
        Target = "FileGenerator.AddIn.Generators.AssemblyFileGenerator")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Scope = "type",
        Target = "FileGenerator.AddIn.Generators.NamespaceFileGenerator")]
[assembly:
    SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "typeCount"
        , Scope = "member", Target = "FileGenerator.AddIn.UI.FileGeneratorControl.#SetupProgressBar(System.Int32)")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member",
        Target = "FileGenerator.AddIn.UI.FileGeneratorControl.#SetTargetInformation(System.Object)")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Scope = "member",
        Target = "FileGenerator.AddIn.UI.FileGeneratorControl.#Resolve(System.Object)")]
[assembly:
    SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames",
        MessageId = "assemblyBrowser", Scope = "member",
        Target =
            "FileGenerator.AddIn.UI.FileGeneratorControl.#OnGenerateFilesButtonClick(System.Object,System.EventArgs)")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member",
        Target = "FileGenerator.AddIn.Generators.TypeFileGenerator.#OnGenerate()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "FileGenerator.AddIn.Generators")]