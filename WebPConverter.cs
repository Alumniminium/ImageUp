using System;
using System.Diagnostics;
using System.IO;

namespace ImgUp
{
    public static class WebPConverter
    {
        //Print + shift
        // maim -s ~/upload.png; imgup ~/upload.png && play ~/.config/.ding.wav
        public static string Convert(string inputPath)
        {
            var arguments = $"-c \"cwebp {inputPath} -o {Path.ChangeExtension(inputPath, ".webp")}\"";
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
            process.WaitForExit(1000 * 15);
            return Path.ChangeExtension(inputPath, ".webp");
        }
    }
}