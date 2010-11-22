// 
// A modification of FileGenReflector: http://filegenreflector.codeplex.com/
// Copyright (c) 2008 (?) Jason R Bock
// Released under the Microsoft Public License:  http://filegenreflector.codeplex.com/license
// Modifications: Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System;
using System.IO;
using System.Threading;
using Reflector;
using Reflector.CodeModel;
using Spackle.Extensions;
using SI = System.IO;

namespace BinaryFinery.BareBonesGenerator.AddIn.Generators
{
    internal sealed class FileGeneratorContext<T>
    {
        internal FileGeneratorContext(T item, string directory, ILanguage language, ITranslator translator,
                                      ManualResetEvent cancel, bool createSubdirectories, bool createVsNetProject)
            : this(item, directory, language, translator, cancel, createSubdirectories, createVsNetProject, false)
        {
        }

        internal FileGeneratorContext(T item, string directory, ILanguage language, ITranslator translator,
                                      ManualResetEvent cancel, bool createSubdirectories, bool createVsNetProject,
                                      bool isRoot)
        {
            directory.CheckParameterForNull("directory");
            translator.CheckParameterForNull("translator");
            language.CheckParameterForNull("language");
            cancel.CheckParameterForNull("cancel");
            item.CheckParameterForNull("item");

            if (!System.IO.Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException(directory + " cannot be found.");
            }

            IsRoot = isRoot;
            Item = item;
            CreateSubdirectories = createSubdirectories;
            CreateVsNetProject = createVsNetProject;

            var separator = new string(Path.DirectorySeparatorChar, 1);

            if (directory.EndsWith(separator, StringComparison.CurrentCultureIgnoreCase))
            {
                directory = directory.Substring(0, directory.Length - 1);
            }

            Directory = directory;
            Translator = translator;
            Language = language;
            Cancel = cancel;
        }

        internal ManualResetEvent Cancel { get; private set; }

        internal bool CreateSubdirectories { get; private set; }

        internal bool CreateVsNetProject { get; private set; }

        internal string Directory { get; private set; }

        internal bool IsRoot { get; private set; }

        internal T Item { get; set; }

        internal ILanguage Language { get; private set; }

        internal ITranslator Translator { get; private set; }
    }
}