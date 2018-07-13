using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RemoteX.Core;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Diagnostics;
using System.Linq;

namespace RemoteX.PC.Core
{
    partial class BluetoothManager
    {
        class BluetoothConnection : IConnection
        {
            protected StreamSocket Socket;
            protected DataWriter SendDataWriter;
            Queue<MessagePack> packMessageBuffer;

            public ConnectionType connectionType
            {
                get
                {
                    return ConnectionType.Bluetooth;
                }
            }

            public ConnectionEstablishState ConnectionEstablishState { get; protected set; }

            public event MessageHandler onReceiveMessage;
            public event ConnectionHandler onConnectionEstalblishResult;

            public Task SendAsync(byte[] message)
            {
                throw new NotImplementedException();
            }

            protected async Task ReceiveAsync()
            {
                DataReader reader = new DataReader(Socket.InputStream);
                packMessageBuffer = new Queue<MessagePack>();
                MessagePack[] messagePacks;
                while (true)
                {
                    reader.InputStreamOptions = InputStreamOptions.Partial;
                    //byte[] dataBytes = new byte[0];
                    messagePacks = await _GetBytes(reader);
                    if (Socket == null)
                    {
                        Debug.WriteLine("接口关闭");
                        ConnectionEstablishState = ConnectionEstablishState.Disconnect;
                        Disconnect();
                        break;
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

            private async Task<MessagePack[]> _GetBytes(DataReader reader)
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
                    if (reader.UnconsumedBufferLength == 990)
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
                while (currIndex < packedMessages.Length)
                {
                    byte[] msgControlCodeBytes = new byte[sizeof(int)];
                    for (int i = 0; i < sizeof(int); i++)
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
                    for (int i = 0; i < msgLength; i++)
                    {
                        message[i] = packedMessages[currIndex + i];
                    }
                    currIndex += msgLength;
                    MessagePack msgPack = new MessagePack(controlCode, message);
                    unpackedMessages.Add(msgPack);
                }
                return unpackedMessages;
            }

            protected virtual void Disconnect() { }

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
        }
    }
}
