// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved.

/*=============================================================================
	DataBox.cs
=============================================================================*/



using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Vultaik
{

    [StructLayout(LayoutKind.Sequential)]
    public struct DataBox : IEquatable<DataBox>
    {
        public IntPtr DataPointer;


        public int RowPitch;


        public int SlicePitch;


        public bool IsEmpty => EqualsByRef(ref empty);


        private static DataBox empty;


        public DataBox(IntPtr datapointer, int rowPitch, int slicePitch)
        {
            DataPointer = datapointer;
            RowPitch = rowPitch;
            SlicePitch = slicePitch;
        }









        public bool Equals(DataBox other) => EqualsByRef(ref other);
        

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is DataBox && Equals((DataBox)obj);
        }

        public override int GetHashCode()
        {

            int hashCode = DataPointer.GetHashCode();
            hashCode = (hashCode * 397) ^ RowPitch;
            hashCode = (hashCode * 397) ^ SlicePitch;

            return unchecked(hashCode);
        }


        public static bool operator ==(DataBox left, DataBox right) => left.Equals(right);
        


        public static bool operator !=(DataBox left, DataBox right) => !left.Equals(right);
        

        private bool EqualsByRef(ref DataBox other) => 
            DataPointer.Equals(other.DataPointer) && RowPitch == other.RowPitch && SlicePitch == other.SlicePitch;
        
    }
}
