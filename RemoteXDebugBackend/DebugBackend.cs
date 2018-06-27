using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RemoteXDataLibary;
/// <summary>
/// 调试后端
/// 因为Unity项目在Editorm模式中没办法使用win32中程序通信间的办法
/// 所以搞出这个调试后端，将控制器的输入都储存到后端上
/// Unity中使用Editor模式时，用WebRequest获取这些东西
/// </summary>
namespace RemoteXDebugBackend
{
    public class DebugBackend
    {
        private static DebugBackend _Instance;
        
        public static DebugBackend Instance
        {
            get
            {
                
                if(_Instance == null)
                {
                    _Instance = new DebugBackend();
                }
                return _Instance;
            }
        }

        public bool Running { get; private set; }

        public event EventHandler OnServerStart;
        public event EventHandler OnServerFailed;
        public event EventHandler OnServerClosed;
        
        /// <summary>
        /// 储存最新的输入数据
        /// </summary>
        private Dictionary<int, Data> latestData;

        public async Task StartAsync(int port)
        {
            latestData = new Dictionary<int, Data>();
            Running = true;
            Port = port;
            await startServer();
        }
        public void Set(Data data)
        {
            
            if (latestData.ContainsKey(data.dataType))
            {
                latestData[data.dataType] = data;
            }
            else
            {
                latestData.Add(data.dataType, data);
            }
        }

        
        public int Port { get; private set; }
        private async Task startServer()
        {
            
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://*:" + Port + "/");
            try
            {
                httpListener.Start();
                HttpListenerResponse response = null;
                System.Diagnostics.Debug.WriteLine("DEBUGBACKEND::Start Listening...");
                OnServerStart?.Invoke(this, null);
                while (true)
                {
                    try
                    {
                        //HttpListenerContext context = httpListener.GetContext();
                        HttpListenerContext context = await httpListener.GetContextAsync();
                        HttpListenerRequest request = context.Request;
                        string[] segments = request.Url.Segments;
                        int dataTypeInt = 0;
                        foreach(string segment in segments)
                        {
                            Console.Write("(" + segment + ")");
                        }
                        if (segments[1] == "get/")
                        {
                            dataTypeInt = int.Parse(segments[2]);
                            
                        }

                        Console.WriteLine();
                        response = context.Response;
                        byte[] responseBuffer = null;
                        if (latestData.ContainsKey(dataTypeInt))
                        {
                            //responseBuffer = Data.encodeSensorData(latestData[dataTypeInt]);
                            responseBuffer = Encoding.Default.GetBytes(latestData[dataTypeInt].ToString());
                        }
                        else
                        {
                            responseBuffer = Encoding.Default.GetBytes("null");
                        }
                        response.ContentLength64 = responseBuffer.Length;
                        Stream output = response.OutputStream;
                        output.Write(responseBuffer, 0, responseBuffer.Length);
                    }
                    catch(HttpListenerException ex)
                    {
                        Console.WriteLine("ERROR IN RESPONSE AND REQUEST: " + ex.Message);
                    }
                    finally
                    {
                        if(response != null)
                        {
                            response.Close();
                        }
                    }
                }
            }
            catch(HttpListenerException ex)
            {
                Console.WriteLine("ERROR IN FUCKING CONNECTION: " + ex.Message);
                OnServerFailed?.Invoke(this, null);
            }
            finally
            {
                httpListener.Close();
            }
        }
        
    }
}
