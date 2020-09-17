// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ

/*=============================================================================
	ILog.cs
=============================================================================*/

namespace Zeckoxe.Core
{
    public interface ILog
    {
        void Info(string message);

        void Warn(string message);

        void Error(string type, string message);
    }
}



