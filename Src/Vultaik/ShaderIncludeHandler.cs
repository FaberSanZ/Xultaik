// Copyright(c) 2019 - 2021 Faber Leonardo.All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)
// Copyright (c) Amer Koleci and contributors.



using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using SharpGen.Runtime;
using Vortice.Dxc;

namespace Vultaik
{
    public class ShaderIncludeHandler : CallbackBase, IDxcIncludeHandler
    {
        private readonly string[] _includeDirectories;
        private readonly Dictionary<string, SourceCodeBlob> _sourceFiles = new Dictionary<string, SourceCodeBlob>();

        public ShaderIncludeHandler(params string[] includeDirectories)
        {
            _includeDirectories = includeDirectories;
        }

        protected override void Dispose(bool disposing)
        {
            foreach (SourceCodeBlob? pinnedObject in _sourceFiles.Values)
            {
                pinnedObject?.Dispose();
            }

            _sourceFiles.Clear();

            base.Dispose(disposing);
        }

        public Result LoadSource(string fileName, out IDxcBlob includeSource)
        {
            if (fileName.StartsWith("./"))
                fileName = fileName.Substring(2);

            string? includeFile = GetFilePath(fileName);

            if (string.IsNullOrEmpty(includeFile))
            {
                includeSource = default;

                return Result.Fail;
            }

            if (!_sourceFiles.TryGetValue(includeFile, out SourceCodeBlob? sourceCodeBlob))
            {
                byte[]? data = File.ReadAllBytes(includeFile);

                sourceCodeBlob = new SourceCodeBlob(data);

                _sourceFiles.Add(includeFile, sourceCodeBlob);
            }

            includeSource = sourceCodeBlob.Blob;

            return Result.Ok;
        }

        private string? GetFilePath(string fileName)
        {
            for (int i = 0; i < _includeDirectories.Length; i++)
            {
                string? filePath = Path.GetFullPath(Path.Combine(_includeDirectories[i], fileName));

                if (File.Exists(filePath))
                {
                    return filePath;
                }
            }

            return null;
        }


        private class SourceCodeBlob : IDisposable
        {
            private byte[] _data;
            private GCHandle _dataPointer;
            private IDxcBlobEncoding? _blob;

            internal IDxcBlob? Blob => _blob;

            public SourceCodeBlob(byte[] data)
            {
                _data = data;

                _dataPointer = GCHandle.Alloc(data, GCHandleType.Pinned);

                _blob = DxcCompiler.Utils?.CreateBlob(_dataPointer.AddrOfPinnedObject(), data.Length, Dxc.DXC_CP_UTF8);
            }

            public void Dispose()
            {
                //_blob?.Dispose();
                _blob = null;

                if (_dataPointer.IsAllocated)
                {
                    _dataPointer.Free();
                }

                _dataPointer = default;
            }
        }
    }
}