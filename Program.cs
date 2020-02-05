using System;
using System.Diagnostics;
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
        public static StreamWriter Logger = new StreamWriter("/home/alumni/imgup.log", true);
        public static async Task Main(string[] args)
        {

            if(Debugger.IsAttached)
            {
                args = new string[] { "-r" , "-c","--height=240", "/tmp/upload.png" };
            }
            if (args.Length == 0) // no args? no bueno.
                return;// seppuku
            Logger.WriteLine("Starting ImgUp...");
            resize = args.Contains("-r");
            Logger.WriteLine("Resize: "+resize);
            convert = args.Contains("-c");
            Logger.WriteLine("Convert: "+convert);
            if (resize)
                sizeh = int.Parse(args.First(a => a.Contains("--height=")).Split('=')[1]);
            
            Logger.WriteLine("Size H: "+sizeh);
            var builder = new StringBuilder();
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-"))
                    continue;
                
            Logger.WriteLine("uploading "+args[i]);
                var url = await Uploader.UploadAsync(WebPConverter.Convert(args[i]));

                if (i == args.Length - 1 && !resize && !convert)
                    builder.Append("image: " + url); // add to url list
                else
                    builder.AppendLine("image: " + url);

                if (resize)
                    args[i] = Processor.Resize(args[i], sizeh);

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
            Logger.WriteLine(builder.ToString());
            Logger.Dispose();
        }// another kind of seppuku
    }
}