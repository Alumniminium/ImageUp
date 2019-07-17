using System.Diagnostics;
using System.IO;

namespace ImgUp.Clipboard
{
    public class LinuxAbstractClipboard : AbstractClipboard
    {
        public override void Set(string text)
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
    }
}