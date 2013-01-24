using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Color20.Win32
{
    internal static class Win32Utils
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        public static Point CurrentMousePosition
        {
            get
            {
                Win32Point w32Mouse = new Win32Point();
                GetCursorPos(ref w32Mouse);
                return new Point(w32Mouse.X, w32Mouse.Y);
            }
        }
    }
}
