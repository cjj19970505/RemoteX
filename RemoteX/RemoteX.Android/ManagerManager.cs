using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RemoteX;
using RemoteX.Bluetooth;
[assembly: Xamarin.Forms.Dependency(typeof(RemoteX.Droid.ManagerManager))]
namespace RemoteX.Droid
{
    public class ManagerManager : IManagerManager
    {
        private IBluetoothManager _BluetoothManager;
        public IBluetoothManager BluetoothManager
        {
            get
            {
                if(_BluetoothManager == null)
                {
                    _BluetoothManager = new BluetoothManager();
                }
                return _BluetoothManager;
            }
        }
    }
}