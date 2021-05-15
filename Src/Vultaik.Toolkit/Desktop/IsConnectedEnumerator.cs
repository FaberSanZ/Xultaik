// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	IsConnectedEnumerator<T>.cs
=============================================================================*/

using System.Collections;
using System.Collections.Generic;

namespace Vultaik.Desktop
{
    internal struct IsConnectedEnumerator<T> : IEnumerator<T> where T : IInputDevice
    {
        private IEnumerator<T> _base;

        public IsConnectedEnumerator(IEnumerator<T> @base)
        {
            _base = @base;
            Current = default!;
        }

        public bool MoveNext()
        {
            while (_base.MoveNext())
            {
                if (_base.Current?.IsConnected ?? false)
                {
                    Current = _base.Current;
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            _base.Reset();
        }

        public T Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }
}
