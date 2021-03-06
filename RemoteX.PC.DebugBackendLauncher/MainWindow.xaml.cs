﻿using RemoteX.Core;
using RemoteX.PC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RemoteX.DebugBackend;
using RemoteX.Data;
using Windows.System;
using System.Timers;

namespace RemoteX.PC.DebugBackendLauncher
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        IServerConnection bluetoothServerConnection;
        Timer startServerTimer;
        public MainWindow()
        {
            InitializeComponent();
            
            ConnectionManager.Instance.onControllerConnectionEstalblishResult += OnControllerConnectionEstablishResult;
            ConnectionManager.Instance.onControllerConnectionReceiveMessage += OnControllerConnectionReceiveMessage;
            DebugBackend.DebugBackend.Instance.OnServerFailed += OnDebugServerFailed;
            DebugBackend.DebugBackend.Instance.OnServerStart += OnDebugServerStart;
            DebugBackend.DebugBackend.Instance.OnReceiveSendRequest += OnReceiveSendRequest;
            BluetoothManager bluetoothManager = BluetoothManager.Instance;
            bluetoothServerConnection = bluetoothManager.CreateRfcommServerConnection(Guid.Parse("14c5449a-6267-4c7e-bd10-63dd79740e5" + 0));
            ConnectionManager.Instance.ControllerConnection = bluetoothServerConnection;
            Timer startServerTimer = new Timer(5000);
            startServerTimer.Elapsed += OnStartTimerElapsed;
            startServerTimer.Start();



            DebugBackend.DebugBackend.Instance.StartBackend(8083);
            img_QR.Source = bluetoothServerConnection.GetQRCode().ToBitmapImage();
            tb_Mac.Text = bluetoothServerConnection.ConnectCode;
            

        }

        private void OnStartTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if(bluetoothServerConnection.ConnectionEstablishState != ConnectionEstablishState.Succeeded)
                {
                    bluetoothServerConnection.StartServer();
                }
                else
                {
                    startServerTimer.Stop();
                }
                
            }
            catch (Exception)
            {

            }
            
        }

        /*
private Task StartServerTask()
{
   return Task.Run(() =>
   {
       while (true)
       {
           try
           {
               BluetoothManager bluetoothManager = BluetoothManager.Instance;
               bluetoothServerConnection.StartServer();
           }
           catch (Exception e)
           {

           }
       }
   });
}*/

        private async void OnReceiveSendRequest(object sender, RemoteXControlMessage e)
        {
            if(ConnectionManager.Instance.ControllerConnection != null && ConnectionManager.Instance.ControllerConnection.ConnectionEstablishState == ConnectionEstablishState.Succeeded)
            {
                await ConnectionManager.Instance.ControllerConnection.SendAsync(e.Bytes);
            }
        }

        private void OnControllerConnectionReceiveMessage(IConnection connection, byte[] message)
        {
            RemoteXControlMessage[] controlMessages = RemoteXControlMessage.FromBytes(message);
            for (int i = 0; i < controlMessages.Length; i++)
            {
                DebugBackend.DebugBackend.Instance.Set(controlMessages[i]);
            }
            this.Dispatcher.Invoke(() =>
            {
                string s = "";
                for (int i = 0;i< controlMessages.Length; i++)
                {
                    s += controlMessages[i].ToString();
                    s += '\n';
                }
                tbox_ReceivedData.Text = s;
            });
        }

        private void OnDebugServerStart(object sender, EventArgs e)
        {
            DebugBackend.DebugBackend.Instance.ServerInfoQRCode = (ConnectionManager.Instance.ControllerConnection as IServerConnection).GetQRCode();
            this.Dispatcher.Invoke(() =>
            {
                tb_Port.Text = DebugBackend.DebugBackend.Instance.Port.ToString();
            });
            
        }

        private void OnDebugServerFailed(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                tb_Port.Text = "服务器开启失败";
            });
            
        }


        private void OnControllerConnectionEstablishResult(IConnection connection, ConnectionEstablishState connectionEstablishState)
        {
            this.Dispatcher.Invoke(() =>
            {
                tb_ConnectionState.Text = connectionEstablishState.ToString();
            });
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ulong targetMac = ulong.Parse(tbox_TargetMac.Text);
                
            }
            finally
            {
                //var bluetoothServerConnection = BluetoothManager.Instance.CreateRfcommServerConnection(Guid.Parse("14c5449a-6267-4c7e-bd10-63dd79740e5" + 0));
                //ConnectionManager.Instance.ControllerConnection = bluetoothServerConnection;
            }
            

        }

        private async void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int dataType = int.Parse(tbox_DataType.Text);
                string sDataValues = tbox_DataValue.Text;
                string[] dataValuesStringArray = sDataValues.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                List<float> valueList = new List<float>();
                foreach (var sData in dataValuesStringArray)
                {
                    valueList.Add(float.Parse(sData));
                }
                RemoteXControlMessage remoteXControlMessage = new RemoteXControlMessage(dataType, valueList.ToArray());
                System.Diagnostics.Debug.WriteLine(remoteXControlMessage);
                if(ConnectionManager.Instance.ControllerConnection != null && ConnectionManager.Instance.ControllerConnection.ConnectionEstablishState == ConnectionEstablishState.Succeeded)
                {
                    await ConnectionManager.Instance.ControllerConnection.SendAsync(remoteXControlMessage.Bytes);
                    label_SendState.Content = "Successful";
                }
            }
            catch (Exception)
            {
                label_SendState.Content = "Failed";
            }
        }
    }
}
