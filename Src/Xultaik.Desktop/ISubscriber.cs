// Copyright (c) Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)


namespace Xultaik.Desktop
{
    public interface ISubscriber
    {
        void Subscribe(Events events);
        void Unsubscribe(Events events);
    }
}
