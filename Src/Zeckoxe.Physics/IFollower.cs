// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. Faber.reach@gmail.com

/*===================================================================================
	Camera.cs
====================================================================================*/



using System.Numerics;

namespace Zeckoxe.Physics
{

    public interface IFollower
    {
        Vector3 Position { get; }

        Vector3 Interest { get; }
    }
}
