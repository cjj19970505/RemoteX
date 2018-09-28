using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RemoteX.Bluetooth.LE.Gatt;

namespace RemoteX.Bluetooth.LE
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BLEAdvertiserTestPage : ContentPage
    {
        public BLEAdvertiserTestPage()
        {
            InitializeComponent();
        }

        private void StartAdvertisingButton_Clicked(object sender, EventArgs e)
        {
            var bluetoothManager = DependencyService.Get<IManagerManager>().BluetoothManager;
            bluetoothManager.GattSever.AddService(new DeviceInfomationServiceBuilder(bluetoothManager).Build());
            bluetoothManager.GattSever.StartAdvertising();
        }

        private void SendNotifyButton_Clicked(object sender, EventArgs e)
        {
            var bluetoothManager = DependencyService.Get<IManagerManager>().BluetoothManager;
            bluetoothManager.GattSever.NotifyTest();
        }
    }
}