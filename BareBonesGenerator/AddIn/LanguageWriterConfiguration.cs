// 
// A modification of FileGenReflector: http://filegenreflector.codeplex.com/
// Copyright (c) 2008 (?) Jason R Bock
// Released under the Microsoft Public License:  http://filegenreflector.codeplex.com/license
// Modifications: Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using Reflector.CodeModel;

namespace BinaryFinery.BareBonesGenerator.AddIn
{
    public class LanguageWriterConfiguration : ILanguageWriterConfiguration
    {
        public LanguageWriterConfiguration()
        {
            Visibility = new VisibilityConfiguration();
        }

        #region ILanguageWriterConfiguration Members

        public IVisibilityConfiguration Visibility { get; private set; }

        public string this[string name]
        {
            get
            {
                switch (name)
                {
                    case "ShowDocumentation":
                        //case "ShowCustomAttributes":
                    case "ShowNamespaceImports":
                    case "ShowNamespaceBody":
                    case "ShowTypeDeclarationBody":
                    case "ShowMethodDeclarationBody":
                        return "true";
                }

                return "false";
            }
        }

        #endregion

        #region Nested type: VisibilityConfiguration

        private class VisibilityConfiguration : IVisibilityConfiguration
        {
            #region IVisibilityConfiguration Members

            public bool Assembly
            {
                get { return true; }
            }

            public bool Family
            {
                get { return true; }
            }

            public bool FamilyAndAssembly
            {
                get { return true; }
            }

            public bool FamilyOrAssembly
            {
                get { return true; }
            }

            public bool Private
            {
                get { return true; }
            }

            public bool Public
            {
                get { return true; }
            }

            #endregion
        }

        #endregion
    }
}