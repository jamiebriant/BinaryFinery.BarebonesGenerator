// 
// A modification of FileGenReflector: http://filegenreflector.codeplex.com/
// Copyright (c) 2008 (?) Jason R Bock
// Released under the Microsoft Public License:  http://filegenreflector.codeplex.com/license
// Modifications: Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using Reflector.CodeModel;

namespace BinaryFinery.BareBonesGenerator.AddIn.Generators
{
    internal sealed class ModuleFileGenerator : FileGenerator<IModule>
    {
        internal ModuleFileGenerator(FileGeneratorContext<IModule> context)
            : base(context)
        {
            TypeCount = Context.Item.Types.Count;
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
            base.InitializeProject(Context.Item.Assembly);

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