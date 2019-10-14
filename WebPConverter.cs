using System.Diagnostics;
using System.IO;

namespace ImgUp
{
    public static class WebPConverter
    {
        public static string Convert(string inputPath)
        {
            var arguments = $"-c \"cwebp -q 75 -alpha_q 10 {inputPath} -o {Path.ChangeExtension(inputPath, ".webp")}\"";
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