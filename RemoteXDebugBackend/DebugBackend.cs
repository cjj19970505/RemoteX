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
        private Dictionary<int, RemoteXControlMessage> latestControlMessage;

        /// <summary>
        /// 当被获取后这些数据就从后端删除
        /// 避免在调试中出现按下按键弹不开的那种情况
        /// </summary>
        private Queue<RCMWithEnqueueTime> unfetchedControlMessageBuffer;
        private Object unfetchedControlMessageBufferLock;
        /// <summary>
        /// 当超过这个时间就判超时
        /// </summary>
        public TimeSpan RemoveBufferTimeout
        {
            get
            {
                return new TimeSpan(0, 0, 0, 1, 0);
            }
        }

        public DebugBackend()
        {
            unfetchedControlMessageBuffer = new Queue<RCMWithEnqueueTime>();
            unfetchedControlMessageBufferLock = new object();
        }
        public void StartBackend(int port)
        {
            latestControlMessage = new Dictionary<int, RemoteXControlMessage>();
            Running = true;
            Port = port;
            Task startServerTask = startServerAsync();
            //Task removeTimeoutBufferTask = removeTimeoutBufferAsync();
        }

        
        
        public void Set(RemoteXControlMessage controlMessage)
        {
            if (!Running)
            {
                return;
            }
            if (latestControlMessage.ContainsKey(controlMessage.DataType))
            {
                latestControlMessage[controlMessage.DataType] = controlMessage;
            }
            else
            {
                latestControlMessage.Add(controlMessage.DataType, controlMessage);
            }

            RCMWithEnqueueTime rcmWithEnqueueTime = new RCMWithEnqueueTime(DateTime.Now, controlMessage);
            lock (unfetchedControlMessageBufferLock)
            {
                unfetchedControlMessageBuffer.Enqueue(rcmWithEnqueueTime);
                DateTime nowDateTime = DateTime.Now;
                DateTime earliestBufferEnqueueTime = unfetchedControlMessageBuffer.Peek().EnqueueDateTime;
                while((nowDateTime - earliestBufferEnqueueTime) > RemoveBufferTimeout && unfetchedControlMessageBuffer.Count>0)
                {
                    unfetchedControlMessageBuffer.Dequeue();
                    System.Diagnostics.Debug.WriteLine("MESSAGE DELETED");
                    earliestBufferEnqueueTime = unfetchedControlMessageBuffer.Peek().EnqueueDateTime;
                }
            }
            

        }
        public int Port { get; private set; }
        private async Task startServerAsync()
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
                        bool hasDataType = false;
                        foreach(string segment in segments)
                        {
                            Console.Write("(" + segment + ")");
                        }
                        if (segments[1] == "get/")
                        {
                            hasDataType = int.TryParse(segments[2], out dataTypeInt);
                        }
                        response = context.Response;
                        byte[] responseBuffer = null;
                        if (hasDataType)
                        {
                            if (latestControlMessage.ContainsKey(dataTypeInt))
                            {
                                responseBuffer = Encoding.Default.GetBytes(latestControlMessage[dataTypeInt].ToString());
                            }
                            else
                            {
                                responseBuffer = Encoding.Default.GetBytes("null");
                            }
                        }
                        else
                        {
                            RCMWithEnqueueTime[] bufferArray;
                            lock (unfetchedControlMessageBufferLock)
                            {
                                bufferArray = unfetchedControlMessageBuffer.ToArray();
                                unfetchedControlMessageBuffer.Clear();
                            }
                            
                            string s = "";
                            foreach(var buffer in bufferArray)
                            {
                                s += buffer.RemoteXControlMessage.ToString();
                                s += '/';
                            }
                            responseBuffer = Encoding.Default.GetBytes(s);
                            
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

        private Task removeTimeoutBufferAsync()
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    lock (unfetchedControlMessageBufferLock)
                    {
                        if (unfetchedControlMessageBuffer.Count > 0)
                        {
                            RCMWithEnqueueTime rcmWithEnqueueTime = unfetchedControlMessageBuffer.Peek();
                            if ((DateTime.Now - rcmWithEnqueueTime.EnqueueDateTime) > RemoveBufferTimeout)
                            {
                                unfetchedControlMessageBuffer.Dequeue();
                            }
                        }
                        
                    }
                }
            });
            
        }

        /// <summary>
        /// 全称：RemoteX Control Message With EnqueueTime
        /// </summary>
        struct RCMWithEnqueueTime
        {
            public RemoteXControlMessage RemoteXControlMessage;
            public DateTime EnqueueDateTime;

            public RCMWithEnqueueTime(DateTime enqueueTime, RemoteXControlMessage controlMessage)
            {
                this.RemoteXControlMessage = controlMessage;
                this.EnqueueDateTime = enqueueTime;
            }
        }
        
    }
}
