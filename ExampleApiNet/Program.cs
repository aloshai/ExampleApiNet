using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleApiNet
{
    public class Program
    {
        static HttpListener httpListener;
        static async Task Main(string[] args)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("This machine not supported HttpListener.");
                return;
            }

            httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://*:80/");
            httpListener.Prefixes.Add("http://*:8080/");

            CreateServer();
            await Task.Delay(-1);
        }

        static void CreateServer()
        {
            httpListener.Start();

            while (true)
            {
                Console.WriteLine("Waiting a new Request...");
                ThreadPool.QueueUserWorkItem(Process, httpListener.GetContext());
            }
        }

        static void Process(object o)
        {
            var context = o as HttpListenerContext;

            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            response.Cookies.Add(new Cookie("content", "Where is my cookie?"));

            string responseString = "Hello World!";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;

            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            response.Close();
        }
    }
}
