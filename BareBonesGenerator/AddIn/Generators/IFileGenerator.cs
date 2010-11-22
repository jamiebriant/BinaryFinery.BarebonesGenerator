// 
// A modification of FileGenReflector: http://filegenreflector.codeplex.com/
// Copyright (c) 2008 (?) Jason R Bock
// Released under the Microsoft Public License:  http://filegenreflector.codeplex.com/license
// Modifications: Copyright (c) 2010 Jamie Briant, BinaryFinery.com
// 
namespace BinaryFinery.BareBonesGenerator.AddIn.Generators
{
    internal delegate void FileCreatedEventHandler(object sender, FileGeneratedEventArgs e);

    internal interface IFileGenerator
    {
        int TypeCount { get; }
        event FileCreatedEventHandler FileCreated;

        void Generate();
    }
}