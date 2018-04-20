using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Xamarin.Forms;

namespace RemoteX.Bluetooth
{
    public class BluetoothDeviceListPage : ContentPage
    {
        IBluetoothManager bluetoothManager;
        ObservableCollection<IBluetoothDevice> bluetoothDeviceList;
        public BluetoothDeviceListPage()
        {
            bluetoothManager = DependencyService.Get<IBluetoothManager>();
            bluetoothDeviceList = new ObservableCollection<IBluetoothDevice>();
            ListView bluetoothDeviceListView = new ListView();
            bluetoothDeviceListView.ItemSelected += onDeviceSelected;
            bluetoothDeviceListView.ItemsSource = bluetoothDeviceList;
            bluetoothDeviceListView.ItemTemplate = new DataTemplate(typeof(BluetoothDeviceCell));
            Button scanDevicesButton = new Button();
            scanDevicesButton.Clicked += (object sender, EventArgs e) =>
            {
                bluetoothManager.searchForBlutoothDevices();
                bluetoothDeviceList.Clear();
            };
            bluetoothManager.onDiscoveryStarted += (IBluetoothManager bluetoothManager) => { scanDevicesButton.IsEnabled = false; scanDevicesButton.Text = "Discovering"; };
            bluetoothManager.onDevicesFound += (IBluetoothManager IBluetoothManager, IBluetoothDevice[] devices) =>
            {
                foreach (IBluetoothDevice device in devices)
                {
                    this.bluetoothDeviceList.Add(device);
                }
            };
            bluetoothManager.onDiscoveryFinished += (IBluetoothManager bluetoothManager) =>
            {
                scanDevicesButton.IsEnabled = true;
                scanDevicesButton.Text = "Scan Devices";
            };
            scanDevicesButton.Text = "Scan Devices";
            Content = new StackLayout
            {
                Children = {
                    scanDevicesButton,
                    bluetoothDeviceListView
                }
            };

        }

        private async void onDeviceSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }
            IBluetoothDevice device = e.SelectedItem as IBluetoothDevice;
            await Navigation.PushAsync(new BluetoothDeviceInfoPage(device));
        }


    }
}