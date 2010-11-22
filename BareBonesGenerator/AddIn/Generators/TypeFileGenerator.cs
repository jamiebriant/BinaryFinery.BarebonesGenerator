// 
// A modification of FileGenReflector: http://filegenreflector.codeplex.com/
// Copyright (c) 2008 (?) Jason R Bock
// Released under the Microsoft Public License:  http://filegenreflector.codeplex.com/license
// Modifications: Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Reflector.CodeModel;
using Reflector.CodeModel.Memory;

namespace BinaryFinery.BareBonesGenerator.AddIn.Generators
{
    internal sealed class TypeFileGenerator : FileGenerator<ITypeDeclaration>
    {
        private readonly string fullTypeName = string.Empty;

        internal TypeFileGenerator(FileGeneratorContext<ITypeDeclaration> context)
            : base(context)
        {
            TypeCount = 1;
            fullTypeName = Context.Item.Namespace + "." + Context.Item.Name;
        }

        public override event FileCreatedEventHandler FileCreated;

        private static string RemoveInvalidCharacters(string data)
        {
            // TODO - 1/14/2006 - This feels awkward - is there a better way to 
            // cleanse a file path with bad characters?
            return data.Replace('/', '_').Replace('\\', '_').Replace(':', '_')
                .Replace('*', '_').Replace('?', '_').Replace('"', '_')
                .Replace('<', '_').Replace('>', '_').Replace('|', '_');
        }

        private void CreateFile(IFormatter formatter)
        {
            // TODO - 1/17/2006 - If the file exists...do we overwrite it, or 
            // create a new file? The issue deals with obfuscation and type names
            // being the same, yet...is that an old file, or one created before with the
            // same name? Maybe type handles could be used...

            string fileName = RemoveInvalidCharacters(
                fullTypeName);

            if (fileName.StartsWith(".", StringComparison.CurrentCultureIgnoreCase))
            {
                fileName = fileName.Substring(1);
            }

            if (Context.CreateSubdirectories)
            {
                fileName = fileName.Replace('.', '\\');
            }

            fileName = Path.Combine(Context.Directory,
                                    fileName + Context.Language.FileExtension);

            string filePath = Path.GetDirectoryName(fileName);

            if (Directory.Exists(filePath) == false)
            {
                Directory.CreateDirectory(filePath);
            }

            using (var typeFile = new StreamWriter(fileName))
            {
                typeFile.Write(formatter.ToString());

                if (Context.CreateVsNetProject)
                {
                    AddGeneratedFileToCompileElement(fileName);
                }

                if (FileCreated != null)
                {
                    FileCreated(this, new FileGeneratedEventArgs(fileName));
                }
            }
        }

        private IFormatter GetFormatter()
        {
            var formatter = new TextFormatter();
            var languageWriterConfiguration = new LanguageWriterConfiguration();
            ILanguageWriter writer = Context.Language.GetWriter(
                formatter, languageWriterConfiguration);

            Context.Item = Context.Translator.TranslateTypeDeclaration(
                Context.Item, true, true);

            ITypeDeclaration typeDeclaration = Context.Item;
            string baseTypeName = typeDeclaration.BaseType != null ? typeDeclaration.BaseType.ToString() : null;
            if (baseTypeName != "MulticastDelegate" && baseTypeName != "Enum")
            {
                FilterFields(typeDeclaration);
                FilterNested(typeDeclaration);
                FilterMethods(typeDeclaration, baseTypeName);
            }


            // NOTE - 1/18/2006 - This is done to ensure all of the type information is written, esp. if namespace information is present.
            if (!string.IsNullOrEmpty(Context.Item.Namespace))
            {
                var typeNamespace = new Namespace {Name = Context.Item.Namespace};

                typeNamespace.Types.Add(Context.Item);
                writer.WriteNamespace(typeNamespace);
            }
            else
            {
                writer.WriteTypeDeclaration(Context.Item);
            }

            return formatter;
        }

        private static void FilterMethods(ITypeDeclaration typeDeclaration, string baseTypeName)
        {
            IMethodDeclarationCollection methods = typeDeclaration.Methods;
            var keepMethods = new List<IMethodDeclaration>();
            bool foundDefaultCtor = false;
            foreach (IMethodDeclaration methodDeclaration in methods)
            {
                if (methodDeclaration.Body != null)
                {
                    IType rt = methodDeclaration.ReturnType.Type;
                    bool giveupGenerateThrowExceptionInstead = false;

                    Expression newResult = GetDefaultValueExpression(rt);

                    string returnTypeName = methodDeclaration.ReturnType != null
                                                ? methodDeclaration.ReturnType.Type.ToString()
                                                : "null";
                    var newBody = new BlockStatement();
                    foreach (IParameterDeclaration parameterDeclaration in methodDeclaration.Parameters)
                    {
                        IType parameterType = parameterDeclaration.ParameterType;
                        if (parameterType is IReferenceType)
                        {
                            var referenceType = parameterType as IReferenceType;
                            Expression defaultValueExpression = GetDefaultValueExpression(referenceType.ElementType);
                            if (defaultValueExpression == null)
                            {
                                giveupGenerateThrowExceptionInstead = true;
                                break;
                            }
                            newBody.Statements.Add(new ExpressionStatement
                                                       {
                                                           Expression = new AssignExpression
                                                                            {
                                                                                Target =
                                                                                    new ArgumentReferenceExpression
                                                                                        {
                                                                                            Parameter =
                                                                                                parameterDeclaration
                                                                                        },
                                                                                Expression = defaultValueExpression
                                                                            }
                                                       });
                        }
                    }
                    if (returnTypeName != "Void")
                    {
                        if (newResult == null || giveupGenerateThrowExceptionInstead)
                        {
                            // shit.
                            ClearStatementsAndGenerateThrowException(newBody);
                        }
                        else
                        {
                            var rs = new MethodReturnStatement {Expression = newResult};
                            newBody.Statements.Add(rs);
                        }
                    }
                    else if (giveupGenerateThrowExceptionInstead)
                    {
                        newBody.Statements.Clear();
                        ClearStatementsAndGenerateThrowException(newBody);
                    }

                    methodDeclaration.Body = newBody;
                    if (methodDeclaration.Name == "Finalize")
                    {
                        // finalize fucks things up. dunno why.
                        continue;
                    }
                    if (methodDeclaration.Visibility == MethodVisibility.Private ||
                        methodDeclaration.Visibility == MethodVisibility.PrivateScope)
                    {
                        if (methodDeclaration.Overrides.Count == 0)
                        {
                            // private override usually means its implementing an interface that would otherwise complain.
                            // otherwise, fuck it:
                            continue;
                        }
                    }

                    var constructorDeclaration = methodDeclaration as IConstructorDeclaration;
                    if (constructorDeclaration != null)
                    {
                        constructorDeclaration.Initializer = null;
                        if (constructorDeclaration.Parameters.Count == 0)
                        {
                            foundDefaultCtor = true;
                        }
                        if (baseTypeName == "ValueType")
                            InitValueTypeConstructor(typeDeclaration, methodDeclaration);
                    }

                    keepMethods.Add(methodDeclaration);
                }
                else // it has no body.
                {
                    if (methodDeclaration.Static == false || methodDeclaration.Visibility == MethodVisibility.Public)
                    {
                        keepMethods.Add(methodDeclaration);
                    }
                }
            }

            methods.Clear();
            methods.AddRange(keepMethods);

            if (!foundDefaultCtor && baseTypeName != "ValueType" && !typeDeclaration.Sealed &&
                !typeDeclaration.Interface)
            {
                methods.Add(GetDefaultConstructorDeclaration(typeDeclaration));
            }
        }

        private static ConstructorDeclaration GetDefaultConstructorDeclaration(ITypeDeclaration td)
        {
            return new ConstructorDeclaration
                       {
                           Visibility = MethodVisibility.Assembly,
                           Body = new BlockStatement(),
                           HideBySignature = true,
                           HasThis = true,
                           Name = ".ctor",
                           RuntimeSpecialName = true,
                           SpecialName = true,
                           ReturnType = new MethodReturnType
                                            {
                                                Type =
                                                    new TypeReference {Name = "Void", Namespace = "System"}
                                            },
                           DeclaringType = td
                       };
        }

        private static void FilterNested(ITypeDeclaration typeDeclaration)
        {
            var keepNested =
                typeDeclaration.NestedTypes.Cast<ITypeDeclaration>().Where(
                    itd => itd.Visibility == TypeVisibility.NestedPublic).ToList();

            typeDeclaration.NestedTypes.Clear();
            typeDeclaration.NestedTypes.AddRange(keepNested);
        }

        private static void FilterFields(ITypeDeclaration typeDeclaration)
        {
            var keepFields =
                typeDeclaration.Fields.Cast<IFieldDeclaration>().Where(ifd => ifd.Visibility == FieldVisibility.Public).
                    ToList();
            typeDeclaration.Fields.Clear();
            typeDeclaration.Fields.AddRange(keepFields);
        }

        private static void ClearStatementsAndGenerateThrowException(BlockStatement blockStatement)
        {
            var snippet = new SnippetExpression {Value = "throw new System.NotImplementedException();"};
            var es = new ExpressionStatement {Expression = snippet};
            blockStatement.Statements.Clear();
            blockStatement.Statements.Add(es);
        }

        private static void InitValueTypeConstructor(ITypeDeclaration icd, IMethodDeclaration md)
        {
            var blockStatement = new BlockStatement();
            foreach (IFieldDeclaration field in icd.Fields)
            {
                if (!field.Static && !field.SpecialName)
                {
                    IExpression dflt = GetDefaultValueExpression(field.FieldType);
                    if (dflt == null)
                    {
                        blockStatement.Statements.Clear();
                        ClearStatementsAndGenerateThrowException(blockStatement);
                        return;
                    }
                    var ass = new AssignExpression
                                  {
                                      Target = new FieldReferenceExpression
                                                   {
                                                       Field = field,
                                                       Target = new ThisReferenceExpression()
                                                   },
                                      Expression = dflt,
                                  };
                    blockStatement.Statements.Add(new ExpressionStatement
                                                      {
                                                          Expression = ass
                                                      });
                }
            }
            md.Body = blockStatement;
        }


        private static Expression GetDefaultValueExpression(IType type)
        {
            Expression newResult = null;
            if (type is ITypeDeclaration)
            {
                var typeDeclaration = type as ITypeDeclaration;
                string baseTypeName = typeDeclaration.BaseType.ToString();
                if (baseTypeName == "Enum" || baseTypeName == "ValueType")
                {
                    newResult = new SnippetExpression {Value = String.Format("default({0})", typeDeclaration.Name)};
                }
                else
                {
                    newResult = new LiteralExpression {Value = null};
                }
            }
            else if (type is IArrayType)
            {
                newResult = new LiteralExpression {Value = null};
            }
            else
            {
                string typeName = type.ToString();

                if (type is ITypeReference)
                {
                    var typeReference = (ITypeReference) type;
                    typeName = typeReference.Name;
                }
                if (typeName == "IntPtr")
                {
                    newResult = new SnippetExpression {Value = "new IntPtr(0)"};
                }
                else if (typeName.Contains("Int"))
                {
                    newResult = new LiteralExpression {Value = 0};
                }
                else if (typeName == "Boolean")
                {
                    newResult = new LiteralExpression {Value = false};
                }
                else if (typeName == "Single")
                {
                    newResult = new LiteralExpression {Value = 0.0f};
                }
                else if (typeName == "Double")
                {
                    newResult = new LiteralExpression {Value = 0.0};
                }
                else if (typeName == "String")
                {
                    newResult = new LiteralExpression {Value = null};
                }
            }
            return newResult;
        }


        public override void Generate()
        {
            IAssembly assembly = ((IModule) Context.Item.Owner).Assembly;

            InitializeProject(assembly);

            IFormatter formatter = GetFormatter();
            CreateFile(formatter);

            SaveProject(Context.Item.Name);
        }
    }
}