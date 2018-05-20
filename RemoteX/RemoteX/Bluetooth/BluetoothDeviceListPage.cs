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
        Button scanDevicesButton;
        public BluetoothDeviceListPage()
        {

            bluetoothManager = DependencyService.Get<IBluetoothManager>();
            bluetoothDeviceList = new ObservableCollection<IBluetoothDevice>();
            resetDeviceList();
            ListView bluetoothDeviceListView = new ListView();
            bluetoothDeviceListView.ItemSelected += onDeviceSelected;
            bluetoothDeviceListView.ItemsSource = bluetoothDeviceList;
            bluetoothDeviceListView.ItemTemplate = new DataTemplate(typeof(BluetoothDeviceCell));
            scanDevicesButton = new Button();

            scanDevicesButton.Clicked += (object sender, EventArgs e) =>
            {
                resetDeviceList();
                bluetoothManager.SearchForBlutoothDevices();

            };
            bluetoothManager.onDiscoveryStarted += onDiscoveryStarted;
            bluetoothManager.onDevicesFound += onDevicesFound;
            bluetoothManager.onDiscoveryFinished += onDiscoveryFinished;
            scanDevicesButton.Text = "Scan Devices";
            Content = new StackLayout
            {
                Children = {
                    scanDevicesButton,
                    bluetoothDeviceListView
                }
            };

        }
        private void onDevicesFound(IBluetoothManager IBluetoothManager, IBluetoothDevice[] devices)
        {
            foreach (IBluetoothDevice device in devices)
            {
                bool isFounded = false;
                foreach (IBluetoothDevice foundedDevice in this.bluetoothDeviceList)
                {
                    if (foundedDevice.Address == device.Address)
                    {
                        isFounded = true;
                        break;
                    }
                }
                if (isFounded)
                {
                    break;
                }
                this.bluetoothDeviceList.Add(device);
            }
        }
        private void resetDeviceList()
        {
            bluetoothDeviceList.Clear();
            IBluetoothDevice[] pairedDevices = bluetoothManager.PairedDevices;
            foreach (IBluetoothDevice device in pairedDevices)
            {
                bluetoothDeviceList.Add(device);
            }
        }
        private void onDiscoveryStarted(IBluetoothManager bluetoothManager) { scanDevicesButton.IsEnabled = false; scanDevicesButton.Text = "Discovering"; }
        private void onDiscoveryFinished(IBluetoothManager bluetoothManager)
        {
            scanDevicesButton.IsEnabled = true;
            scanDevicesButton.Text = "Scan Devices";
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
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            bluetoothManager.onDiscoveryStarted -= onDiscoveryStarted;
            bluetoothManager.onDevicesFound -= onDevicesFound;
            bluetoothManager.onDiscoveryFinished -= onDiscoveryFinished;
        }
    }
}