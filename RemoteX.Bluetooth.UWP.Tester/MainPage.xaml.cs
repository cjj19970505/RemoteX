using RemoteX.Bluetooth.LE.Gatt;
using RemoteX.Bluetooth.Win10;
using RemoteX.Bluetooth.Win10.LE.Gatt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace RemoteX.Bluetooth.UWP.Tester
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            BluetoothManager bluetoothManager = BluetoothManager.Instance;
            GattScanner gattScanner = bluetoothManager.GattScanner;
            gattScanner.OnReceived += GattScanner_OnReceived;
            gattScanner.StartScan();
        }

        private async void GattScanner_OnReceived(object sender, BluetoothManager.BluetoothDevice e)
        {
            System.Diagnostics.Debug.WriteLine("(Name: " + e.Name+" Address: "+BluetoothUtils.AddressInt64ToString(e.Address)+")");
            if (e.Name.Contains("xj"))
            {
                System.Diagnostics.Debug.WriteLine("Got you");
                BluetoothManager.Instance.GattScanner.StopScan();
            }
            else
            {
                return;
            }
            var gattClient = await e.ConnectToGattServerAsync();
            if(gattClient != null)
            {
                System.Diagnostics.Debug.WriteLine("Connected");
                int serviceCount = 0;
                IGattClientService[] services = null;
                while (serviceCount == 0)
                {
                    System.Diagnostics.Debug.WriteLine("No Service");
                    services = await gattClient.DiscoverAllPrimaryServiceAsync();
                    serviceCount = services.Length;
                }
                foreach(var service in services)
                {
                    System.Diagnostics.Debug.WriteLine("SERVICE:" + service.Uuid);
                    var characteristics = await service.DiscoverAllCharacteristicsAsync();
                    foreach(var characteristic in characteristics)
                    {
                        System.Diagnostics.Debug.WriteLine("CHARACTERISTIC:" + characteristic.Uuid);
                        byte[] value = (await characteristic.ReadCharacteristicValueAsync()).Value;
                        if(value == null)
                        {
                            System.Diagnostics.Debug.WriteLine("CHARACTERISTIC VALUE: NULL");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("CHARACTERISTIC VALUE: "+ Encoding.ASCII.GetString(value));
                        }
                    }
                }
            }
            
        }
    }
}
