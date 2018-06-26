using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Globalization;
using System.IO;

namespace RemoteXDebugServer
{
    class Program
    {
        static int port = 8081;
        static Dictionary<int, string> dataDict;

        static void Main(string[] args)
        {
            dataDict = new Dictionary<int, string>();
            string[] prefixes = new string[1];
            prefixes[0] = "http://*:" + port + "/";
            ProcessRequests(prefixes);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setOrGet">FALSE::SET  TRUE::GET</param>
        /// <param name="data"></param>
        private static void processData(bool setOrGet, string data)
        {

        }
        private static void ProcessRequests(string[] prefixes)
        {
            HttpListener listener = new HttpListener();
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
            try
            {
                listener.Start();
                Console.WriteLine("Listener...");

                while (true)
                {
                    HttpListenerResponse response = null;
                    try
                    {
                        HttpListenerContext context = listener.GetContext();
                        Console.WriteLine(context.Request.RawUrl);
                        Console.WriteLine(context.Request.ContentType);

                        //byte[] bytes = context.Request.InputStream.ReadToEnd();
                        //Console.WriteLine(BitConverter.ToString(bytes));
                        byte[] bytes;
                        using (StreamReader streamReader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                        {
                            bytes = Encoding.Default.GetBytes(streamReader.ReadToEnd());
                        }

                        for(int i = 0; i < bytes.Length; i++)
                        {
                            Console.Write(bytes[i] + " ");
                        }
                        Console.WriteLine();
                        DateTime dateTime = DateTime.Now;
                        Console.WriteLine(dateTime + ":" + dateTime.Millisecond);

                        response = context.Response;
                        

                        string responseString =
                            "<HTML><BODY>The time is currently " +
                            DateTime.Now.ToString(
                            DateTimeFormatInfo.CurrentInfo) +
                            "</BODY></HTML>";
                        
                        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                        response.ContentLength64 = buffer.Length;
                        Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                    }
                    catch (HttpListenerException ex)
                    {
                        Console.WriteLine("FUCK??"+ex.Message);
                    }
                    finally
                    {
                        if (response != null)
                        {
                            response.Close();
                        }
                    }
                }
            }
            catch (HttpListenerException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Stop listening for requests.
                listener.Close();
                Console.WriteLine("Done Listening.");
            }
        }

        
    }


}
