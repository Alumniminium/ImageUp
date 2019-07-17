using System;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceClient
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length == 0) // no args? no bueno.
                return;// seppuku

            var builder = new StringBuilder();
            for (var i = 0; i < args.Length; i++)
            {
                // upload image, get direct link bacl
                var url = await Uploader.UploadAsync(args[i]);
                // if this is the last file, don't add a new line at the end.
                if (i == args.Length - 1)
                    builder.Append(url); // add to url list
                else
                    builder.AppendLine(url); // add line to url list

                Console.WriteLine(url); // optional
            }
            // set clipboard to the url list 
            Clipboard.Set(builder.ToString());
        }// another kind of seppuku
    }
}