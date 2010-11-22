// 
// A modification of FileGenReflector: http://filegenreflector.codeplex.com/
// Copyright (c) 2008 (?) Jason R Bock
// Released under the Microsoft Public License:  http://filegenreflector.codeplex.com/license
// Modifications: Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
using System;
using Spackle.Extensions;

namespace BinaryFinery.BareBonesGenerator.AddIn
{
    internal sealed class FileGeneratedEventArgs : EventArgs
    {
        private const string ErrorEmptyFileName = "The given file name must contain information.";

        internal FileGeneratedEventArgs(string fileName)
        {
            fileName.CheckParameterForNull("fileName");

            if (fileName.Length == 0)
            {
                throw new ArgumentException(ErrorEmptyFileName, "fileName");
            }

            FileName = fileName;
        }

        internal string FileName { get; private set; }
    }
}