// 
// A modification of FileGenReflector: http://filegenreflector.codeplex.com/
// Copyright (c) 2008 (?) Jason R Bock
// Released under the Microsoft Public License:  http://filegenreflector.codeplex.com/license
// Modifications: Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using Reflector.CodeModel;

namespace BinaryFinery.BareBonesGenerator.AddIn.Generators
{
    internal sealed class NamespaceFileGenerator : FileGenerator<INamespace>
    {
        internal NamespaceFileGenerator(FileGeneratorContext<INamespace> context)
            : base(context)
        {
            base.TypeCount = Context.Item.Types.Count;
        }

        public override event FileCreatedEventHandler FileCreated;

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
            if (Context.Item.Types != null && Context.Item.Types.Count > 0)
            {
                var assembly = ((IModule) Context.Item.Types[0].Owner).Assembly;
                base.InitializeProject(assembly);

                foreach (ITypeDeclaration typeDeclaration in Context.Item.Types)
                {
                    var context = new FileGeneratorContext<ITypeDeclaration>(
                        typeDeclaration, Context.Directory, Context.Language,
                        Context.Translator, Context.Cancel,
                        Context.CreateSubdirectories, Context.CreateVsNetProject);
                    var typeGenerator = new TypeFileGenerator(context);

                    var fileCreatedHandler = new FileCreatedEventHandler(OnFileGenerated);

                    try
                    {
                        typeGenerator.FileCreated += fileCreatedHandler;
                        typeGenerator.Generate();
                    }
                    finally
                    {
                        typeGenerator.FileCreated -= fileCreatedHandler;
                    }

                    if (Context.Cancel.WaitOne(FileGeneratorFactory.EventWaitTime, false))
                    {
                        break;
                    }
                }

                base.SaveProject(Context.Item.Name);
            }
        }
    }
}