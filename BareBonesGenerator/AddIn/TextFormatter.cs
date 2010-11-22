// ---------------------------------------------------------
// Lutz Roeder's .NET Reflector
// Copyright (c) 2000-2006 Lutz Roeder. All rights reserved.
// http://www.aisto.com/roeder
// ---------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using Reflector.CodeModel;

namespace BinaryFinery.BareBonesGenerator.AddIn
{
    internal sealed class TextFormatter : IFormatter, IDisposable
    {
        private readonly StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
        private bool disposed;
        private int indent;
        private bool newLine;
        public bool AllowProperties { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            CheckForDisposed();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IFormatter Members

        public void Write(string text)
        {
            CheckForDisposed();
            ApplyIndent();
            writer.Write(text);
        }

        public void WriteDeclaration(string text)
        {
            CheckForDisposed();
            WriteBold(text);
        }

        public void WriteDeclaration(string value, object target)
        {
            CheckForDisposed();
            Write(value);
        }

        public void WriteComment(string text)
        {
            CheckForDisposed();
            WriteText(text);
        }

        public void WriteLiteral(string text)
        {
            CheckForDisposed();
            WriteText(text);
        }

        public void WriteKeyword(string text)
        {
            CheckForDisposed();
            WriteText(text);
        }

        public void WriteIndent()
        {
            CheckForDisposed();
            indent++;
        }

        public void WriteLine()
        {
            CheckForDisposed();
            writer.WriteLine();
            newLine = true;
        }

        public void WriteOutdent()
        {
            CheckForDisposed();
            indent--;
        }

        public void WriteReference(string text, string toolTip, Object reference)
        {
            CheckForDisposed();
            ApplyIndent();
            writer.Write(text);
        }

        public void WriteProperty(string propertyName, string propertyValue)
        {
            CheckForDisposed();
            if (AllowProperties)
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        ~TextFormatter()
        {
            Dispose(false);
        }

        private void ApplyIndent()
        {
            CheckForDisposed();

            if (newLine)
            {
                for (int i = 0; i < indent; i++)
                {
                    writer.Write("    ");
                }

                newLine = false;
            }
        }

        private void CheckForDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("TextFormatter");
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    writer.Dispose();
                }

                disposed = true;
            }
        }

        public override string ToString()
        {
            CheckForDisposed();
            return writer.ToString();
        }

        private void WriteBold(string text)
        {
            CheckForDisposed();
            ApplyIndent();
            writer.Write(text);
        }

        private void WriteText(string text)
        {
            CheckForDisposed();
            ApplyIndent();
            writer.Write(text);
        }
    }
}