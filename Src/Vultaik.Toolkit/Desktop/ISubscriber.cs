// Copyright (c) 2019-2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	ISubscriber.cs
=============================================================================*/

namespace Vultaik.Desktop
{
    public interface ISubscriber
    {
        void Subscribe(Events events);
        void Unsubscribe(Events events);
    }
}
