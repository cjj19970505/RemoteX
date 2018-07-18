using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using RemoteX.Core;

namespace RemoteX.WifiDirect
{
	public class WifiDirectDeviceListPage : ContentPage
	{
        IWifiDirectManager _WifiDirectManager;
        ObservableCollection<IWifiDirectDevice> _WifiDirectDeviceList;
        Button _ScanDevicesButton;
        Button _ConnectButton;
        IWifiDirectDevice _SelectedDevice;
        public WifiDirectDeviceListPage ()
		{
            _WifiDirectManager = DependencyService.Get<IWifiDirectManager>();
            _WifiDirectDeviceList = new ObservableCollection<IWifiDirectDevice>();

            ListView wifiDirectDeviceListView = new ListView();
            wifiDirectDeviceListView.ItemsSource = _WifiDirectDeviceList;
            wifiDirectDeviceListView.ItemSelected += _OnDeviceSelected;
            wifiDirectDeviceListView.ItemTemplate = new DataTemplate(typeof(WifiDirectDeviceCell));

            _WifiDirectManager.OnPeersFound += _OnPeersFound;
            _ScanDevicesButton = new Button();
            _ScanDevicesButton.Clicked+= (object sender, EventArgs e) =>
            {
                resetDeviceList();
                _WifiDirectManager.SearchForPeers();

            };
            _ConnectButton = new Button()
            {
                Text = "Connect"
            };
            _ConnectButton.Clicked += _OnConnectButtonClicked;
            Content = new StackLayout
            {
                Children =
                {
                    _ScanDevicesButton,
                    wifiDirectDeviceListView,
                    _ConnectButton
                }
            };


        }

        private async void _OnConnectButtonClicked(object sender, EventArgs e)
        {
            if(_SelectedDevice != null)
            {
                IClientConnection connection = _WifiDirectManager.CreateClientConnection(_SelectedDevice);
                await connection.ConnectAsync();
            }
        }

        private void resetDeviceList()
        {
            _WifiDirectDeviceList.Clear();
        }

        private async void _OnDeviceSelected(object sender, SelectedItemChangedEventArgs e)
        {

            if (e.SelectedItem == null)
            {
                return;
            }
            /*
            IBluetoothDevice device = e.SelectedItem as IBluetoothDevice;
            await Navigation.PushAsync(new BluetoothDeviceInfoPage(device));*/
            IWifiDirectDevice device = e.SelectedItem as IWifiDirectDevice;
            _SelectedDevice = device;
            
        }

        private void _OnPeersFound(IWifiDirectManager wifiDirectManager, IWifiDirectDevice[] devices)
        {
            foreach(IWifiDirectDevice device in devices)
            {
                this._WifiDirectDeviceList.Add(device);
            }
        }
    }
}