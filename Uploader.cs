using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ImgUp
{
    public static class Uploader
    {
        // First we will set our root address for the following requests
        private const string FTP_IMG_ROOT = "ftp://cdn.her.st/images/";
        private static readonly string User;
        private static readonly string Pass;
        // Since this is a static class and its initialized only if arguments are passed, its ok to block in the constructor.
        // Don't do this is bigger applications. 
        static Uploader()
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/imgup");
            var tokenFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/imgup/login.token";
            if (!File.Exists(tokenFilePath))
                Program.Logger.WriteLine("Need a `login.token` file in `.config/imgup` with ftp `user:pass` content");
            var tokenFile = File.ReadAllText(tokenFilePath);
            User = tokenFile.Split(':')[0];
            Pass = tokenFile.Split(':')[1];
        }

        // Saves a couple of lines of code :D
        private static FtpWebRequest CreateUploadRequest(string file)
        {
            var request = (FtpWebRequest)WebRequest.Create(FTP_IMG_ROOT + $"{file}");
            request.Credentials = new NetworkCredential(User, Pass);
            request.EnableSsl = true; // this is the reason we can't use WebClient. It won't work with ssl.
            request.Method = WebRequestMethods.Ftp.UploadFile;
            return request;
        }

        // not sure why i ended up using tasks... i bet they just slow everything down tbh..
        // you test that and email me the results. trrbl@her.st ;D
        public static async Task<string> UploadAsync(string path)
        {
            // most simple way to generate unique filenames? using guid's!
            var request = CreateUploadRequest(Guid.NewGuid() + Path.GetExtension(path));

            using (var fileStream = File.OpenRead(path)) // doing streams like a good boi in case file is biiig
            using (var ftpStream = request.GetRequestStream())
                await fileStream.CopyToAsync(ftpStream); // but in the end I take the lazy route.

            return request.RequestUri.AbsoluteUri.Replace("ftp", "https");
        }
    }
}