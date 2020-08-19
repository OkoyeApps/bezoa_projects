using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
            using (var client = new WebClient())
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
            using (var client = new WebClient())
            {
                string content = await client.DownloadStringTaskAsync(url);
                Console.WriteLine(content.Substring(0, 100));
                Console.WriteLine("End of string");
            }
            Console.WriteLine("At the Bottom");
            //Console.ReadLine();
        }

        private static void TraceThreadAndTask(string infoData)
        {
            string taskInfo = Task.CurrentId == null ? "no task" : $"task {Task.CurrentId}";
            Console.WriteLine($"{infoData} in the thread {Thread.CurrentThread.ManagedThreadId} and {taskInfo}");
        }

        private static string Greeting(string name)
        {
            TraceThreadAndTask($"currently running {nameof(Greeting)}");

            Task.Delay(3000).Wait();
            return $"Hello, {name}";
        }

        private static Task<string> GreetingAsyc(string name)
        {
            return Task.Run(() =>
            {
                TraceThreadAndTask($"currently running {nameof(Greeting)}");
                throw new Exception("An error occured");
                return Greeting(name);
            });
        }

        private async static Task callWithAsyc(string name)
        {
            TraceThreadAndTask($"currently running {nameof(Greeting)}");
            string result = await GreetingAsyc(name);
            Console.WriteLine(result);
            TraceThreadAndTask($"ended, {nameof(callWithAsyc)}");
        }

        private static async Task simulateErrorHandling()
        {
            try
            {
                await callWithAsyc("Emmanuel");
                Console.WriteLine("after the await \n \n \n");

                var result = callWithAsyc("Emmanuel").GetAwaiter();
                Console.WriteLine($"is completed? {result.IsCompleted}");
                result.OnCompleted(() =>
                {
                    Console.WriteLine($"Is completed finally? {result.IsCompleted}");

                });
            }catch(Exception err)
            {
                Console.WriteLine("Something went wrong");
            }
        }

        static async Task Main(string[] args)
        {
            //SyncApi();
            //AsyncApi();
            //EventBasedAsynApi();
            //await TaskBasedAsyncApi();
            //SyncApi();


            simulateErrorHandling();


            Console.ReadLine();
        }
    }
}
