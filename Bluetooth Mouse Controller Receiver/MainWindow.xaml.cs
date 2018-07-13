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

using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using RemoteXDataLibary;
using RemoteX.PC.Core;
using RemoteX.Core;
using RemoteX.DebugBackend;

namespace Bluetooth_Mouse_Controller_Receiver
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        ControllerManager controllerManager;
        BTTask btTask;
        private Dictionary<IConnection, ControllerManager> controllerManagers;
        public MainWindow()
        {
            controllerManagers = new Dictionary<IConnection, ControllerManager>();
            InitializeComponent();
        }

        //这个站且用来测试鼠标移动，以下就是他妈的移动方法。
        private void Button_BluetoothInitialize_Click(object sender, RoutedEventArgs e)
        {
            /*
            BTTaskManager btTaskManager = BTTaskManager.instance;
            btTask = btTaskManager.newTask();
            btTask.onReceiveMessage += onReceiveData;
            Debug.WriteLine("UI::" + Thread.CurrentThread.ManagedThreadId);
            btTask.startAdvertising();
            ControllerManager controllerManager = new ControllerManager();
            
            ImageSource is_QRCode = BitmapToBitmapImage(btTask.QRCode);
            img_QRCode.Source = is_QRCode;
            controllerManager = new ControllerManager();*/

            ControllerManager controllerManager = new ControllerManager();
            BluetoothManager bluetoothManager = BluetoothManager.Instance;
            var bluetoothServerConnection = bluetoothManager.CreateRfcommServerConnection(Guid.Parse("14c5449a-6267-4c7e-bd10-63dd79740e5" + 0));
            bluetoothServerConnection.OnConnectionEstalblishResult += OnConnectionEstalblishResult;
            bluetoothServerConnection.onReceiveMessage += _OnReceiveMessage;
            bluetoothServerConnection.StartServer();
            controllerManagers.Add(bluetoothServerConnection, controllerManager);



        }

        private void OnConnectionEstalblishResult(IConnection connection, ConnectionEstablishState connectionEstablishState)
        {

        }

        private void _OnReceiveMessage(IConnection connection, byte[] message)
        {
            RemoteXDataLibary.RemoteXControlMessage[] datas = null;
            try
            {
                datas = RemoteXDataLibary.RemoteXControlMessage.FromBytes(message);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine("XJ2::" + exception.Message);
            }
            ControllerManager controllerManager = controllerManagers[connection];
            if (datas != null)
            {
                foreach (var data in datas)
                {
                    controllerManager.addData(data);
                    DebugBackend.Instance.Set(data);
                    Debug.WriteLine(data);
                }
            }
        }

        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png); // 坑点：格式选Bmp时，不带透明度

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }
        /*
        private void onReceiveData(BTTask btTask, byte[] message)
        {
            RemoteXDataLibary.RemoteXControlMessage[] datas = null;
            try
            {
                datas = RemoteXDataLibary.RemoteXControlMessage.FromBytes(message);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine("XJ2::" + exception.Message);
            }
            ControllerManager controllerManager = controllerManagers[btTask.taskId];
            if (datas != null)
            {
                foreach (var data in datas)
                {
                    controllerManager.addData(data);
                    RemoteXDebugBackend.DebugBackend.Instance.Set(data);
                    Debug.WriteLine(data);
                }
            }
        }*/
        private void Button_MoveCursor_Click(object sender, RoutedEventArgs e)
        {
            System.Drawing.Point point = new System.Drawing.Point();
            point.X = Convert.ToInt32(TextBox_SetMousePositionX.Text);
            point.Y = Convert.ToInt32(TextBox_SetMousePositionY.Text);
            MoveCursor moveCursor = new MoveCursor();
            moveCursor.MoveTo(point);
        }

        private void tbMouseRateX_TextChanged(object sender, TextChangedEventArgs e)
        {
            foreach (var pair in controllerManagers)
            {
                var controllerManager = pair.Value;
                float tryRate;
                if (float.TryParse(tbMouseRateX.Text, out tryRate))
                {
                    controllerManager.gyroscopeMouseManager.mouseSpeedFactor.x = tryRate;
                }

            }
        }

        private void tbMouseRateY_TextChanged(object sender, TextChangedEventArgs e)
        {
            foreach (var pair in controllerManagers)
            {
                var controllerManager = pair.Value;
                float tryRate;
                if (float.TryParse(tbMouseRateY.Text, out tryRate))
                {
                    controllerManager.gyroscopeMouseManager.mouseSpeedFactor.y = tryRate;
                }

            }
        }

        private async void btn_StartDebugBackend_Click(object sender, RoutedEventArgs e)
        {
            if (RemoteX.DebugBackend.DebugBackend.Instance.Running)
            {
                return;
            }
            RemoteX.DebugBackend.DebugBackend.Instance.StartBackend(8081);
        }

        private async void btnSendTestData_Click(object sender, RoutedEventArgs e)
        {
            DateTime currTime = DateTime.Now;
            while (true)
            {
                if (btTask != null)
                {
                    if ((DateTime.Now - currTime).TotalMilliseconds > 20)
                    {
                        currTime = DateTime.Now;
                        await btTask.Send(2, new RemoteXControlMessage(1, new float[] { 56, 23, 12, 1.23f }).Bytes);
                        await btTask.Send(1, new RemoteXControlMessage(2, new float[] { 56, 23, 12, 1.23f }).Bytes);
                        await btTask.Send(6, new RemoteXControlMessage(3, new float[] { 56, 23, 12, 1.23f }).Bytes);
                        await btTask.Send(0, new RemoteXControlMessage(4, new float[] { 56, 23, 12, 1.23f }).Bytes);
                        await btTask.Send(5, new RemoteXControlMessage(87, new float[] { 56, 23, 12, 1.23f }).Bytes);
                        await btTask.Send(0, new RemoteXControlMessage(5, new float[] { 56, 23, 12, 1.23f }).Bytes);
                        await btTask.Send(8, new RemoteXControlMessage(6, new float[] { 56, 23, 12, 1.23f }).Bytes);
                    }

                }
            }


        }
    }
}
