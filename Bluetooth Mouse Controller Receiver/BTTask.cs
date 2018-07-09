using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Networking.Sockets;
using System.Diagnostics;
using Windows.Storage.Streams;
using System.Threading;
using RemoteXDebugBackend;
using System.Drawing;
using ZXing;
using ZXing.QrCode;
using Windows.Devices.Bluetooth;

namespace Bluetooth_Mouse_Controller_Receiver
{
    class BTTask
    {
        private const uint SERVICE_VERSION_ATTRIBUTE_ID = 0x0300;
        private const byte SERVICE_VERSION_ATTRIBUTE_TYPE = 0x0A;
        private const uint SERVICE_VERSION = 200;
        /// <summary>
        /// 每个BTTask都有一个唯一标识码
        /// </summary>
        private Guid _taskId;
        private StreamSocket _socket;
        private RfcommServiceProvider _provider;
        //public async void startA
        public delegate void MessageHandler(BTTask btTask, byte[] message);
        public delegate void BTTaskHandler(BTTask btTask);
        public event MessageHandler onReceiveMessage;
        public event BTTaskHandler onConnectionEstablished;
        public BluetoothAdapter _BluetoothAdapter;
        private DataWriter sendtDataWriter;
        public BTTask(Guid taskId)
        {
            this._taskId = taskId;
            packMessageBuffer = new Queue<MessagePack>();
            Task<BluetoothAdapter> t = BluetoothAdapter.GetDefaultAsync().AsTask();
            while (!t.IsCompleted)
            {
                System.Diagnostics.Debug.WriteLine("NOT COMPLETED");
            }
            BluetoothAdapter bluetoothAdapter = t.Result;
            if(bluetoothAdapter != null)
            {
                Debug.WriteLine("MAC::" + bluetoothAdapter.BluetoothAddress);
                _BluetoothAdapter = bluetoothAdapter;
            }
            else
            {
                Debug.WriteLine("WTF");
            }
            //_LastMessageProcessedTime = DateTime.Now;
        }
        public Guid taskId
        {
            get
            {
                return _taskId;
            }
        }
        public Guid uuid
        {
            get
            {
                //return new Guid("14c5449a-6267-4c7e-bd10-63dd79740e5" + BTTaskManager.instance.getIndex(this));
                return new Guid("14c5449a-6267-4c7e-bd10-63dd79740e5" + 0);
            }
        }
        public async void startAdvertising()
        {
            int index = BTTaskManager.instance.getIndex(this);

            RfcommServiceId myId = RfcommServiceId.FromUuid(uuid);
            _provider = await RfcommServiceProvider.CreateAsync(myId);
            StreamSocketListener listener = new StreamSocketListener();
            listener.ConnectionReceived += onConnectionReceived;
            await listener.BindServiceNameAsync(_provider.ServiceId.AsString(), SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);
            InitializeServiceSdpAttributes(_provider);
            _provider.StartAdvertising(listener, true);
        }
        public async Task Send(int controlCode ,byte[] message)
        {
            byte[] packedBytes = _PackMessage(controlCode, message);
            sendtDataWriter.WriteBytes(packedBytes);
            await sendtDataWriter.StoreAsync();
        }

        private byte[] _PackMessage(int controlCode, byte[] message)
        {
            byte[] controlCodeBytes = BitConverter.GetBytes(controlCode);
            byte[] dataLengthBytes = BitConverter.GetBytes(message.Length);
            byte[] packedMsg = new byte[controlCodeBytes.Length + dataLengthBytes.Length + message.Length];
            controlCodeBytes.CopyTo(packedMsg, 0);
            dataLengthBytes.CopyTo(packedMsg, controlCodeBytes.Length);
            message.CopyTo(packedMsg, controlCodeBytes.Length + dataLengthBytes.Length);
            return packedMsg;
        }
        private void InitializeServiceSdpAttributes(RfcommServiceProvider provider)
        {
            DataWriter writer = new DataWriter();
            // First write the attribute type
            writer.WriteByte(SERVICE_VERSION_ATTRIBUTE_TYPE);
            // Then write the data
            writer.WriteUInt32(SERVICE_VERSION);
            IBuffer data = writer.DetachBuffer();
            provider.SdpRawAttributes.Add(SERVICE_VERSION_ATTRIBUTE_ID, data);

        }
        private void onConnectionReceived(StreamSocketListener listener, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            _provider.StopAdvertising();
            _socket = args.Socket;
            System.Diagnostics.Debug.WriteLine("OnConnect");

            //TextBlock_Log.Text = "成功";
            if (_socket != null)
            {
                sendtDataWriter = new DataWriter(_socket.OutputStream);
                System.Diagnostics.Debug.WriteLine("SUCCEES on Thread: " + Thread.CurrentThread.ManagedThreadId);
                onConnectionEstablished?.Invoke(this);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("FUCK NO");
            }
            _HandlePackMessageBuffer();
            receive();
        }
        async void receive()
        {
            DataReader reader = new DataReader(_socket.InputStream);
            MessagePack[] messagePacks;
            while (true)
            {

                reader.InputStreamOptions = InputStreamOptions.Partial;
                //byte[] dataBytes = new byte[0];
                messagePacks = await getBytes(reader);
                if (_socket == null)
                {
                    Debug.WriteLine("接口关闭");
                }
                lock (packMessageBuffer)
                {
                    for (int i = 0; i < messagePacks.Length; i++)
                    {
                        packMessageBuffer.Enqueue(messagePacks[i]);

                    }
                    messagePacks = null;
                }
            }
        }

        private async Task<MessagePack[]> getBytes(DataReader reader)
        {
            const int LOAD_BYTES_COUNT = 990;
            List<byte> finalDataBytes = new List<byte>();
            byte[] currentDataBytes;
            bool needNextLoad = true;
            int loadCount = 0;

            while (needNextLoad)
            {
                loadCount++;
                DateTime beforLoad = DateTime.Now;
                await reader.LoadAsync(LOAD_BYTES_COUNT);
                if (reader.UnconsumedBufferLength < LOAD_BYTES_COUNT)
                {
                    needNextLoad = false;
                }
                else
                {
                    needNextLoad = true;
                }
                if(reader.UnconsumedBufferLength == 990)
                {
                    System.Diagnostics.Debug.WriteLine("990");
                }
                currentDataBytes = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(currentDataBytes);
                finalDataBytes.AddRange(currentDataBytes);
            }

            MessagePack[] messagePacks = null;
            try
            {
                messagePacks = unpackMessages(finalDataBytes.ToArray()).ToArray();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return messagePacks;
        }

        

        private static List<MessagePack> unpackMessages(byte[] packedMessages)
        {
            List<MessagePack> unpackedMessages = new List<MessagePack>();
            int currIndex = 0;
            while(currIndex < packedMessages.Length)
            {
                byte[] msgControlCodeBytes = new byte[sizeof(int)];
                for(int i = 0; i < sizeof(int); i++)
                {
                    msgControlCodeBytes[i] = packedMessages[i + currIndex];
                }
                currIndex += sizeof(int);
                int controlCode = BitConverter.ToInt32(msgControlCodeBytes, 0);

                byte[] msgLengthBytes = new byte[sizeof(int)];
                for (int i = 0; i < sizeof(int); i++)
                {
                    msgLengthBytes[i] = packedMessages[i + currIndex];
                }
                currIndex += sizeof(int);
                int msgLength = BitConverter.ToInt32(msgLengthBytes, 0);

                byte[] message = new byte[msgLength];
                for(int i = 0; i < msgLength; i++)
                {
                    message[i] = packedMessages[currIndex + i];
                }
                currIndex += msgLength;
                MessagePack msgPack = new MessagePack(controlCode, message);
                unpackedMessages.Add(msgPack);
            }
            return unpackedMessages;
        }

        Queue<MessagePack> packMessageBuffer;
        /// <summary>
        /// 上一条Message被处理的时间
        /// </summary>
        private async void _HandlePackMessageBuffer()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    MessagePack messagePack;
                    lock (packMessageBuffer)
                    {

                        if (packMessageBuffer.Count <= 0)
                        {
                            continue;
                        }
                        messagePack = packMessageBuffer.Dequeue();

                    }
                    if(messagePack.ControlCode == 2)
                    {
                        onReceiveMessage?.Invoke(this, messagePack.Message);
                    }
                }
            });

        }

        
        public Bitmap QRCode
        {
            get
            {
                BarcodeWriter writer = new BarcodeWriter();
                writer.Format = BarcodeFormat.QR_CODE;
                QrCodeEncodingOptions options = new QrCodeEncodingOptions();
                options.DisableECI = true;
                options.CharacterSet = "UTF-8";
                options.Width = 500;
                options.Height = 500;
                options.Margin = 1;
                writer.Options = options;
                string encodedConnection = RemoteXDataLibary.Connection.EncodeBluetoothConnection(_BluetoothAdapter.BluetoothAddress, uuid);
                Bitmap map = writer.Write(encodedConnection);
                return map;
            }
        }

        class MessagePack
        {
            byte[] _Message;

            public byte[] Message
            {
                get
                {
                    return _Message.ToArray();
                }
                set
                {
                    _Message = value.ToArray();
                }
            }

            public int ControlCode { get; private set; }
            public MessagePack(int controlCode, byte[] message)
            {
                this.ControlCode = controlCode;
                this.Message = message;
            }
            
        }

        /*
        class MessagePack
        {
            byte[] _Message;
            public double TimeSpanInMs { get; private set; }
            public double LoadDateTimespanInMs { get; private set; }

            /// <summary>
            /// 在本机这个包被创建的时刻
            /// </summary>
            public DateTime GeneratedDateTime { get; set; }

            /// <summary>
            /// 这个包完成所有预处理，被使用的时刻
            /// </summary>
            public DateTime ConsumeDateTime { get; set; }

            /// <summary>
            /// 这个包从缓存队列被取下来的时刻
            /// </summary>
            public DateTime DequeueDateTime { get; set; }

            /// <summary>
            /// 期望被解包的时刻
            /// 一般就是上一个包处理完以后马上解包，所以这个应该差不多就是上一个包的消耗时刻
            /// </summary>
            public DateTime ExpectedDequeueDateTime { get; set; }

            public int ControlCode { get; private set; }

            public MessagePack(byte[] message, double timeSpanInMs)
            {
                this._Message = message.ToArray();
                this.TimeSpanInMs = timeSpanInMs;
                if (timeSpanInMs < 0)
                {
                    ControlCode = -(int)timeSpanInMs;
                }
                else
                {
                    ControlCode = -1;
                }
                this.GeneratedDateTime = DateTime.Now;

            }
            public double PreprocessTimespanInMs
            {
                get
                {
                    return (ConsumeDateTime - GeneratedDateTime).TotalMilliseconds;
                }
            }
            public byte[] Message
            {
                get
                {

                    return _Message.ToArray();
                }
            }

        }
        */
    }
}
