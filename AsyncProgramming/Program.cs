using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace AsyncProgramming
{
    class Program
    {
        private const string url = "http://www.cninnovation.com";

      

        private static void AsyncApi()
        {
            Console.WriteLine(nameof(AsyncApi));
            WebRequest request = WebRequest.Create(url);
            IAsyncResult result = request.BeginGetResponse(ReadResponse, null);

            void ReadResponse(IAsyncResult ar)
            {
                using (WebResponse response = request.EndGetResponse(ar))
                {
                    Stream stream = response.GetResponseStream();
                    var reader = new StreamReader(stream);
                    string content = reader.ReadToEnd();
                    Console.WriteLine(content.Substring(0, 100));
                    Console.WriteLine("End of string");
                }
            Console.WriteLine("At the Bottom");
            }
            Console.ReadLine();
        }

        private static void EventBasedAsynApi()
        {
            Console.WriteLine(nameof(EventBasedAsynApi));
            using(var client =  new WebClient())
            {
                client.DownloadStringCompleted += Client_DownloadStringCompleted;
                client.DownloadStringAsync(new Uri(url));
                Console.WriteLine("Fired event");
                Console.WriteLine("Waiting for event");
            }
            Console.WriteLine("Before async code fired");
            Console.ReadLine();
        }

        private static void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            Console.WriteLine("Async code Download complete");
            Console.WriteLine(e.Result.Substring(0, 100));
            Console.WriteLine("Completed async download");
        }

        private static void SyncApi()
        {
            Console.WriteLine(nameof(SyncApi));
            using (var client = new WebClient())
            {
                string content = client.DownloadString(url);
                Console.WriteLine(content.Substring(0, 100));
            }
            Console.WriteLine("end of string");
            Console.ReadLine();
        }


        private static async Task TaskBasedAsyncApi()
        {
            Console.WriteLine(nameof(TaskBasedAsyncApi));
            using(var client = new WebClient())
            {
                string content = await client.DownloadStringTaskAsync(url);
                Console.WriteLine(content.Substring(0, 100));
                Console.WriteLine("End of string");
            }
            Console.WriteLine("At the Bottom");
            //Console.ReadLine();
        }

        static async Task Main(string[] args)
        {
            //SyncApi();
            //AsyncApi();
            //EventBasedAsynApi();
            await TaskBasedAsyncApi();
            SyncApi();

        }
    }
}
