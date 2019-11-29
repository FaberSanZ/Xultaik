// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	GraphicsResourceUsage.cs
=============================================================================*/


namespace Zeckoxe.Graphics
{
    public enum GraphicsResourceUsage
    {
        Default = unchecked(0),

        Immutable = unchecked(1),

        Dynamic = unchecked(2),

        Staging = unchecked(3),
    }
}