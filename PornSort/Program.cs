using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;

namespace PornSort
{
    class Program
    {
        static void Main(string[] args)
        {
            var sortedImagesDirectory = new DirectoryInfo("sorted");
            var unsortedImagesDirectory = new DirectoryInfo("images");
            var apiKey = "API KEY";
            var apiEndpoint = String.Format("https://api.haystack.ai/api/image/analyzeadult?output=json&apikey={0}", apiKey);

            using (var client = new WebClient())
            {
                foreach (var imagePath in unsortedImagesDirectory.EnumerateFiles())
                {
                    var image = File.ReadAllBytes(imagePath.FullName);
                    var responseData = client.UploadData(apiEndpoint, "POST", image);
                    var responseText = Encoding.UTF8.GetString(responseData);
                    dynamic response = JsonConvert.DeserializeObject(responseText);
                    var adultType = (String) response.adultContent.adultContentType;
                    string sortedType;

                    if (String.IsNullOrEmpty(adultType))
                        sortedType = "clean";
                    else
                        sortedType = adultType;

                    var sortedDirectory = Path.Combine(sortedImagesDirectory.FullName, sortedType);
                    var sortedPath = Path.Combine(sortedDirectory, imagePath.Name);

                    Directory.CreateDirectory(sortedDirectory);
                    File.Move(imagePath.FullName, sortedPath);
                    Thread.Sleep(50);
                }
            }
        }
    }
}
