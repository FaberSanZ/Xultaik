// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

using System.Collections;
using System.Collections.Generic;

namespace Xultaik.Desktop
{
    internal struct IsConnectedWrapper<T> : IReadOnlyList<T> where T : IInputDevice
    {
        private readonly IReadOnlyList<T> _list;

        public IsConnectedWrapper(IReadOnlyList<T> list) => _list = list;
        public IEnumerator<T> GetEnumerator() => new IsConnectedEnumerator<T>(_list.GetEnumerator());

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int Count => _list.Count;

        public T this[int index] => _list[index];
    }
}
