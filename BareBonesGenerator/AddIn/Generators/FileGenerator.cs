// 
// A modification of FileGenReflector: http://filegenreflector.codeplex.com/
// Copyright (c) 2008 (?) Jason R Bock
// Released under the Microsoft Public License:  http://filegenreflector.codeplex.com/license
// Modifications: Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.BuildEngine;
using Reflector.CodeModel;
using Spackle.Extensions;

namespace BinaryFinery.BareBonesGenerator.AddIn.Generators
{
    internal abstract class FileGenerator<T> : IFileGenerator
    {
        protected FileGenerator(FileGeneratorContext<T> context)
        {
            context.CheckParameterForNull("context");
            Context = context;
            SubDirectories = new List<string>();
        }

        internal BuildItemGroup CompileFiles { get; private set; }

        internal FileGeneratorContext<T> Context { get; private set; }

        internal Project Project { get; private set; }

        internal List<string> SubDirectories { get; private set; }

        #region IFileGenerator Members

        public abstract event FileCreatedEventHandler FileCreated;

        public abstract void Generate();
        public int TypeCount { get; protected set; }

        #endregion

        protected void AddGeneratedFileToCompileElement(string filePath)
        {
            if (Context.IsRoot && Context.CreateVsNetProject)
            {
                var codeFileName = Path.GetFileName(filePath);
                var codeDirectory = Path.GetDirectoryName(filePath) ?? string.Empty;
                codeDirectory = codeDirectory.Replace(
                    Context.Directory, string.Empty);

                if (codeDirectory.StartsWith(@"\", StringComparison.CurrentCultureIgnoreCase))
                {
                    codeDirectory = codeDirectory.Substring(1);
                }

                if (codeDirectory.Length > 0)
                {
                    codeDirectory += @"\";
                }

                if (Context.CreateSubdirectories)
                {
                    if (!SubDirectories.Contains(codeDirectory))
                    {
                        SubDirectories.Add(codeDirectory);
                        CompileFiles.AddNewItem("Compile", codeDirectory + "*" +
                                                           Context.Language.FileExtension);
                    }
                }
                else
                {
                    CompileFiles.AddNewItem("Compile", codeDirectory + codeFileName);
                }
            }
        }

        private void CreateImportTargets()
        {
            var importProjectFile = @"$(MSBuildBinPath)\";

            if (Context.Language.Name == "C#")
            {
                importProjectFile += "Microsoft.CSharp.targets";
            }
            else if (Context.Language.Name == "Visual Basic")
            {
                importProjectFile += "Microsoft.VisualBasic.targets";
            }
            else
            {
                importProjectFile += "Microsoft.Common.targets";
            }

            Project.AddNewImport(importProjectFile, string.Empty);
        }

        private void CreatePropertyGroups()
        {
            var debugGroup = Project.AddNewPropertyGroup(false);
            debugGroup.Condition = " '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ";
            debugGroup.AddNewProperty("DebugSymbols", "true");
            debugGroup.AddNewProperty("DebugType", "full");
            debugGroup.AddNewProperty("OutputPath", @"bin\Debug\");
            debugGroup.AddNewProperty("Optimize", "false");

            var releaseGroup = Project.AddNewPropertyGroup(false);
            releaseGroup.Condition = " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ";
            releaseGroup.AddNewProperty("DebugSymbols", "false");
            releaseGroup.AddNewProperty("DebugType", "pdbonly");
            releaseGroup.AddNewProperty("OutputPath", @"bin\Release\");
            releaseGroup.AddNewProperty("Optimize", "true");
        }

        private void InitializeAssemblyReferencesGroup(IAssembly baseAssembly)
        {
            if (baseAssembly.AssemblyManager.Assemblies.Count > 0)
            {
                var referencedAssemblies = new List<IAssembly>();

                foreach (IAssembly referencedAssembly in
                    baseAssembly.AssemblyManager.Assemblies)
                {
                    if (referencedAssembly != baseAssembly)
                    {
                        referencedAssemblies.Add(referencedAssembly);
                    }
                }

                if (referencedAssemblies.Count > 0)
                {
                    var references = Project.AddNewItemGroup();

                    foreach (var referencedAssembly in referencedAssemblies)
                    {
                        references.AddNewItem("Reference", referencedAssembly.Name);
                    }
                }
            }
        }

        private void InitializeCompileGroup()
        {
            if (TypeCount > 0)
            {
                CompileFiles = Project.AddNewItemGroup();
            }
        }

        protected void InitializeProject(IAssembly baseAssembly)
        {
            if (Context.IsRoot && Context.CreateVsNetProject && Context.CreateVsNetProject)
            {
                Project = new Project(new Engine());
                Project.DefaultTargets = "Build";
                CreatePropertyGroups();
                InitializeAssemblyReferencesGroup(baseAssembly);
                InitializeCompileGroup();
                CreateImportTargets();

                if (Context.CreateSubdirectories)
                {
                    SubDirectories = new List<string>();
                }
            }
        }

        protected void SaveProject(string projectName)
        {
            if (Context.IsRoot && Context.CreateVsNetProject && Project != null)
            {
                var projectExtension = string.Empty;

                if (Context.Language.Name == "C#")
                {
                    projectExtension = ".csproj";
                }
                else if (Context.Language.Name == "Visual Basic")
                {
                    projectExtension += ".vbproj";
                }
                else
                {
                    projectExtension += ".msbuild";
                }

                string projectFileName = projectName + projectExtension;
                Project.Save(Path.Combine(Context.Directory, projectFileName));
            }
        }
    }
}