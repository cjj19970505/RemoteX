using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace RemoteX.Bluetooth
{
	public class BluetoothDeviceInfoPage : ContentPage
	{
        private IBluetoothDevice _BluetoothDevice;
        ObservableCollection<Guid> guidList;
        public BluetoothDeviceInfoPage (IBluetoothDevice bluetoothDevice)
		{
            this._BluetoothDevice = bluetoothDevice;
            this.Title = _BluetoothDevice.Name;
            guidList = new ObservableCollection<Guid>();
            
            Guid[] lastestUuids = _BluetoothDevice.LastestFetchedUuids;
            if(lastestUuids != null)
            {
                foreach (var guid in lastestUuids)
                {
                    guidList.Add(guid);
                }
            }
            
            Label nameLabel = new Label();
            nameLabel.BindingContext = _BluetoothDevice;
            nameLabel.SetBinding(Label.TextProperty, "Name");

            Label addressLabel = new Label();
            addressLabel.BindingContext = _BluetoothDevice;
            addressLabel.SetBinding(Label.TextProperty, "Address");

            ListView uuidListView = new ListView();
            uuidListView.ItemsSource = guidList;

            uuidListView.IsPullToRefreshEnabled = true;
            uuidListView.Refreshing += (object sender, EventArgs e) =>
            {
                if (_BluetoothDevice.IsFetchingUuids)
                {
                    uuidListView.EndRefresh();
                }
                _BluetoothDevice.FetchUuidsWithSdp();

            };

            _BluetoothDevice.OnUuidsFetched += (IBluetoothDevice device, Guid[] guids) =>
              {
                  System.Diagnostics.Debug.WriteLine("UUID GET");
                  if (uuidListView.IsRefreshing)
                  {
                      uuidListView.EndRefresh();
                  }
                  guidList.Clear();
                  if (guids != null)
                  {
                      foreach (var guid in guids)
                      {
                          guidList.Add(guid);
                      }
                  }
              };
            Content = new StackLayout {
                Orientation = StackOrientation.Vertical,
				Children = {
                    //nameLabel,
                    //addressLabel,
                    uuidListView
                }
			};
		}

        

        
	}
}