using System.Diagnostics;
using System.IO;

namespace ImgUp.Clipboard
{
    public class LinuxClipboard : AbstractClipboard
    {
        public override void Set(string text)
        {
            var tmpFilePath = Path.GetTempFileName();
            File.WriteAllText(tmpFilePath, text);
            try
            {
                var arguments = $"-c \"cat {tmpFilePath} | xclip -selection clipboard\"";
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "bash",
                        Arguments = arguments,
                        UseShellExecute = false,
                        CreateNoWindow = false,
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
    }
}