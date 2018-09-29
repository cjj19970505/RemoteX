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
        BatteryServiceWrapper BatteryServiceWrapper;
        public BLEAdvertiserTestPage()
        {
            InitializeComponent();
        }

        private void StartAdvertisingButton_Clicked(object sender, EventArgs e)
        {
            var bluetoothManager = DependencyService.Get<IManagerManager>().BluetoothManager;
            bluetoothManager.GattSever.AddService(new DeviceInfomationServiceBuilder(bluetoothManager).Build());
            BatteryServiceWrapper = new BatteryServiceWrapper(bluetoothManager);
            bluetoothManager.GattSever.AddService(BatteryServiceWrapper.GattServerService);
            bluetoothManager.GattSever.StartAdvertising();
        }

        private void SendNotifyButton_Clicked(object sender, EventArgs e)
        {
            var bluetoothManager = DependencyService.Get<IManagerManager>().BluetoothManager;
            bluetoothManager.GattSever.NotifyTest();
        }

        private void NotifyTestSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            BatteryServiceWrapper.BatteryLevelCharacteristicWrapper.BatteryLevel = (int)e.NewValue;
            BatteryServiceWrapper.BatteryLevelCharacteristicWrapper.NotifyAll();
        }
    }
}