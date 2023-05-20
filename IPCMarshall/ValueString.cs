using System.Runtime.InteropServices;
using System.Text;

namespace IPCMarshall;

public unsafe struct ValueString
{
    public const int MaxLen = 255;
    public fixed char CharPtrBacking[MaxLen];

    public ValueString(string @string)
    {
        if (@string == null) throw new ArgumentNullException(nameof(@string));
        if (@string.Length > MaxLen) throw new ArgumentException("String too long", nameof(@string));
        fixed (char* ptr = CharPtrBacking)
        {
            var bytes = Encoding.UTF8.GetBytes(@string);
            Marshal.Copy(bytes, 0, (IntPtr)ptr, bytes.Length);
        }
    }

    public string String
    {
        get
        {
            fixed (char* ptr = CharPtrBacking)
            {
                return new string((sbyte*)ptr);
            }
        }
    }

    public override string ToString() => String;
}