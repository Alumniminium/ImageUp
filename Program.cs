using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ImgUp.Clipboard;

namespace ImgUp
{
    public static class Program
    {
        public static bool resize, convert;
        public static int sizeh;
        public static AbstractClipboard Clipboard;
        static StreamWriter Logger = new StreamWriter("/home/alumni/imgup.log", true);
        public static async Task Main(string[] args)
        {
            if (args.Length == 0) // no args? no bueno.
                return;// seppuku

            resize = args.Contains("-r");
            convert = args.Contains("-c");
            if (resize)
                sizeh = int.Parse(args.First(a => a.Contains("--height=")).Split('=')[1]);

            var builder = new StringBuilder();
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-"))
                    continue;

                var url = await Uploader.UploadAsync(WebPConverter.Convert(args[i]));

                if (i == args.Length - 1 && !resize && !convert)
                    builder.Append("image: " + url); // add to url list
                else
                    builder.AppendLine("image: " + url);

                if (resize)
                    args[i] = ImageResizer.Resize(args[i], sizeh);

                if (convert)
                    args[i] = WebPConverter.Convert(args[i]);

                // upload image, get direct link back
                if (resize || convert)
                    url = await Uploader.UploadAsync(args[i]);
                // if this is the last file, don't add a new line at the end.
                if (i == args.Length - 1)
                    builder.Append("thumbnail: " + url); // add to url list
                else
                    builder.AppendLine("thumbnail: " + url);
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Clipboard = new LinuxClipboard();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Clipboard = new WindowsClipboard();

            // set clipboard to the url list 
            Clipboard.Set(builder.ToString());
            Console.WriteLine(builder.ToString());
        }// another kind of seppuku
    }
}