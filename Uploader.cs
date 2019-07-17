using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ImgUp
{
    public static class Uploader
    {
        // First we will set our root address for the following requests
        private const string FTP_IMG_ROOT = "ftp://h.img.alumni.re/images/";
        // Next we set our Id file's public HTTP url, we will download this and parse it to set the current Id.
        private const string HTTP_IMG_ID_FILE = "https://h.img.alumni.re/images/Id.txt";
        // We use the curId as something like a counter, so we don't overwrite old files. I decided to do this on the client since *I'm* the only client.
        // Don't be an idiot.
        private static int _nextId;
        private static int _curId;

        // Since this is a static class and its initialized only if arguments are passed, its ok to block in the constructor.
        // Don't do this is bigger applications. 
        static Uploader()
        {

            // as stated above, here we download and parse the Id file so we know what the last Id on the server is
            // (it gets worse)
            using (var client = new WebClient())
                client.DownloadFile(HTTP_IMG_ID_FILE, "Id.txt");

            if (File.Exists("Id.txt") && int.TryParse(File.ReadAllText("Id.txt"), out _curId))
                _nextId = _curId + 1;
        }

        // Saves a couple of lines of code :D
        private static FtpWebRequest CreateUploadRequest(string file)
        {
            var request = (FtpWebRequest)WebRequest.Create(FTP_IMG_ROOT + $"{file}");
            request.Credentials = new NetworkCredential("ftp", "root");
            request.EnableSsl = true; // this is the reason we can't use WebClient. It won't work with ssl.
            request.Method = WebRequestMethods.Ftp.UploadFile;
            return request;
        }

        // not sure why i ended up using tasks... i bet they just slow everything down tbh..
        // you test that and email me the results. blog@her.st ;D
        public static async Task<string> UploadAsync(string path)
        {
            // further attemt at creating a more unique path but still giving it some readability.
            // this would turn File.txt into File_3948.txt
            // I don't even check if a file with the same name exists and just assume so.
            // Asking the server for a file list, looking for it and THEN starting to upload
            // takse too much time. This is single user anyways, I won't run 20 instances of this shit.
            var request = CreateUploadRequest(Path.GetFileNameWithoutExtension(path) + "_" + _curId + Path.GetExtension(path));

            using (var fileStream = File.OpenRead(path)) // doing streams like a good boi in case file is biiig
            using (var ftpStream = request.GetRequestStream())
                await fileStream.CopyToAsync(ftpStream); // but in the end I take the lazy route.

            await UpdateId(); // Told you it'd get worse.
            return request.RequestUri.AbsoluteUri.Replace("ftp", "https");
        }

        // did you think the server would keep track of the counter? 
        private static async Task UpdateId()
        {
            Interlocked.Increment(ref _curId); // atomicly incrementing our counters because by now i have no idea where our methods execute
            Interlocked.Increment(ref _nextId); // doing this seems to calm me down, no idea if its snakeoil

            await File.WriteAllTextAsync("Id.txt", $"{_curId}"); // we write it so we can read it ...

            var request = CreateUploadRequest("Id.txt"); // another request

            using (var fileStream = File.OpenRead("Id.txt")) // this is a file with a fucking number in it. Number might get big, lets use a stream XDDDDDDD
            using (var ftpStream = request.GetRequestStream())
                await fileStream.CopyToAsync(ftpStream); // another lazy way out
        }
    }
}