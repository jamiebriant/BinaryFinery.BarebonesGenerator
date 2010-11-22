// 
// A modification of FileGenReflector: http://filegenreflector.codeplex.com/
// Copyright (c) 2008 (?) Jason R Bock
// Released under the Microsoft Public License:  http://filegenreflector.codeplex.com/license
// Modifications: Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System.IO;
using BinaryFinery.BareBonesGenerator.AddIn;
using BinaryFinery.BareBonesGenerator.AddIn.Generators;
using Microsoft.Build.BuildEngine;
using Reflector.CodeModel;
using Reflector.CodeModel.Memory;

namespace BinaryFinery.BareBonesGenerator.AddIn.Generators
{
    internal sealed class AssemblyFileGenerator : FileGenerator<IAssembly>
    {
        internal AssemblyFileGenerator(FileGeneratorContext<IAssembly> context)
            : base(context)
        {
            foreach (IModule module in Context.Item.Modules)
            {
                TypeCount += module.Types.Count;
            }
        }

        public override event FileCreatedEventHandler FileCreated;

        private static void AddResourceToBuild(IResource resource, string resourceFileName,
                                               BuildItemGroup resources)
        {
            var resourceItem = resources.AddNewItem(
                "EmbeddedResource", resourceFileName);

            if (resource.Visibility == ResourceVisibility.Private)
            {
                resourceItem.SetMetadata("Visible", "false");
            }
        }

        private void GenerateAssemblyAttributesFile()
        {
            if (Context.Item.Attributes.Count > 0)
            {
                var formatter = new TextFormatter();
                var languageWriterConfiguration =
                    new LanguageWriterConfiguration();
                var writer = Context.Language.GetWriter(
                    formatter, languageWriterConfiguration);
                writer.WriteAssembly(Context.Item);

                string attributesFileName = Path.Combine(
                    Context.Directory, "GeneratedAssemblyInfo" +
                                       Context.Language.FileExtension);

                using (var attributesFile = new StreamWriter(attributesFileName))
                {
                    attributesFile.Write(formatter.ToString());
                    AddGeneratedFileToCompileElement(attributesFileName);

                    if (FileCreated != null)
                    {
                        FileCreated(this, new FileGeneratedEventArgs(attributesFileName));
                    }
                }
            }
        }

        private void InitializeEmbeddedResourcesGroup()
        {
            if (Context.Item.Resources != null && Context.Item.Resources.Count > 0)
            {
                var embeddedResources = Project.AddNewItemGroup();

                foreach (var resource in Context.Item.Resources)
                {
                    var fileRes = resource as FileResource;

                    if (fileRes != null)
                    {
                        SaveFileResource(fileRes, embeddedResources);
                    }
                    else
                    {
                        var embedRes = resource as EmbeddedResource;

                        if (embedRes != null)
                        {
                            SaveEmbeddedResource(embedRes, embeddedResources);
                        }
                    }
                }
            }
        }

        private void OnFileGenerated(object sender, FileGeneratedEventArgs e)
        {
            base.AddGeneratedFileToCompileElement(e.FileName);

            if (FileCreated != null)
            {
                FileCreated(this, e);
            }
        }

        public override void Generate()
        {
            base.InitializeProject(Context.Item);
            InitializeEmbeddedResourcesGroup();
            GenerateAssemblyAttributesFile();

            foreach (IModule module in Context.Item.Modules)
            {
                var context = new FileGeneratorContext<IModule>(module,
                                                                Context.Directory, Context.Language,
                                                                Context.Translator, Context.Cancel,
                                                                Context.CreateSubdirectories, Context.CreateVsNetProject);

                var moduleGenerator = new ModuleFileGenerator(context);

                var fileCreatedHandler = new FileCreatedEventHandler(OnFileGenerated);

                try
                {
                    moduleGenerator.FileCreated += fileCreatedHandler;
                    moduleGenerator.Generate();
                }
                finally
                {
                    moduleGenerator.FileCreated -= fileCreatedHandler;
                }

                if (Context.Cancel.WaitOne(FileGeneratorFactory.EventWaitTime, false))
                {
                    break;
                }
            }

            base.SaveProject(Context.Item.Name);
        }

        private void SaveEmbeddedResource(EmbeddedResource resource, BuildItemGroup resources)
        {
            string resourceFileName = Path.Combine(
                Context.Directory, resource.Name);

            using (var resourceFile = new BinaryWriter(
                File.Create(resourceFileName)))
            {
                resourceFile.Write(resource.Value);
                AddResourceToBuild(resource, resourceFileName, resources);

                if (FileCreated != null)
                {
                    FileCreated(this, new FileGeneratedEventArgs(resourceFileName));
                }
            }
        }

        private void SaveFileResource(FileResource resource, BuildItemGroup resources)
        {
            var formatter = new TextFormatter();
            var languageWriterConfiguration =
                new LanguageWriterConfiguration();
            var writer = Context.Language.GetWriter(
                formatter, languageWriterConfiguration);
            writer.WriteResource(resource);

            string resourceFileName = Path.Combine(
                Context.Directory, resource.Name);

            using (var resourceFile = new StreamWriter(resourceFileName))
            {
                resourceFile.Write(formatter.ToString());
                AddResourceToBuild(resource, resourceFileName, resources);

                if (FileCreated != null)
                {
                    FileCreated(this, new FileGeneratedEventArgs(resourceFileName));
                }
            }
        }
    }
}