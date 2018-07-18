using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace RemoteX.WifiDirect
{
	public class WifiDirectDeviceListPage : ContentPage
	{
        IWifiDirectManager _WifiDirectManager;
        ObservableCollection<IWifiDirectDevice> _WifiDirectDeviceList;
        Button _ScanDevicesButton;

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
            Content = new StackLayout
            {
                Children =
                {
                    _ScanDevicesButton,
                    wifiDirectDeviceListView
                }
            };


        }
        private void resetDeviceList()
        {
            _WifiDirectDeviceList.Clear();
        }

        private void _OnDeviceSelected(object sender, SelectedItemChangedEventArgs e)
        {
            throw new NotImplementedException();
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