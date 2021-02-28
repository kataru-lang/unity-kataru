using System.Runtime.InteropServices;
using System;

namespace Kataru
{
    /// <summary>
    /// Simple struct for receiving int arrays over FFI.
    /// </summary>
    struct FFIArray
    {
        public IntPtr vecptr;
        public UIntPtr length;

        public int[] ToArray()
        {
            var buffer = new int[(int)length];
            Marshal.Copy(vecptr, buffer, 0, buffer.Length);
            return buffer;
        }
    }
}
