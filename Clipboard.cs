using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace ImgUp
{
    public static class Clipboard
    {
        public static void Set(string text)
        {
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                SetLinux(text);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                SetWindows(text);
            else
            Console.WriteLine("IDK HOW TO SET CLIPBOARD ON " +OSPlatform.OSX);
        }

        private static void SetWindows(string text)
        {
            try
            {
                var bytes = ((uint)text.Length + 1) * 2;
                var hGlobal = GlobalAlloc(GMEM_MOVABLE, (UIntPtr)bytes);
                
                try
                {
                    var source = Marshal.StringToHGlobalUni(text);

                    try
                    {
                        var target = GlobalLock(hGlobal);
                        
                        try
                        {
                            CopyMemory(target, source, bytes);
                        }
                        finally
                        {
                            GlobalUnlock(target);
                        }

                        hGlobal = IntPtr.Zero;
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(source);
                    }
                }
                finally
                {
                    if (hGlobal != IntPtr.Zero)
                        GlobalFree(hGlobal);
                }
            }
            finally
            {
                CloseClipboard();
            }
        }

        private static void SetLinux(string text)
        {
            var tmpFilePath = Path.GetTempFileName();
            File.WriteAllText(tmpFilePath, text);
            try
            {
                var arguments = $"-c \"cat {tmpFilePath} | xclip -i -selection clipboard\"";
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "bash",
                        Arguments = arguments,
                        UseShellExecute = false,
                    }
                };
                process.Start();
                process.WaitForExit(1000 * 5);
            }
            finally
            {
                File.Delete(tmpFilePath);
            }
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalFree(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
        
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseClipboard();

        private const uint GMEM_MOVABLE = 0x0002;
    }
}