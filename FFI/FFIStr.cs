
using System.Runtime.InteropServices;
using System;
using System.Text;

namespace Kataru
{
    /// <summary>
    /// Simple struct for receiving strings over FFI.
    /// </summary>
    struct FFIStr
    {
        public IntPtr strptr;
        public UIntPtr length;

        public override string ToString()
        {
            var buffer = new byte[(int)length];
            Marshal.Copy(strptr, buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

        public void ThrowIfError()
        {
            if (length != UIntPtr.Zero)
            {
                string errorMsg = ToString();
                throw new Exception($"'{errorMsg}'");
            }
        }
    }
}
