using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;

namespace RemoteX.Droid
{
    /// <summary>
    /// 针对Xamarin.Form中IBluetoothDevice对Android的BluetoothDevice进行的一层包装
    /// 目前不足：在FetchUuid的时候要是遇到蓝牙或者Discovery被中途退出，或者一直获取不到uuid，IsFetchingUuids就会一直为true
    /// </summary>
    class BluetoothDeviceWrapper : RemoteX.Bluetooth.IBluetoothDevice
    {
        Receiver _Receiver;

        public string Name { get; private set; }

        public string Address { get; private set; }

        public Guid[] LastestFetchedUuids { get; private set; }

        public bool IsFetchingUuids { get; private set; }

        public event RemoteX.Bluetooth.BluetoothDeviceGetUuidsHanlder OnUuidsFetched;

        public BluetoothDevice BluetoothDevice { get; private set; }

        public BluetoothDeviceWrapper(BluetoothDevice bluetoothDevice)
        {
            this.BluetoothDevice = bluetoothDevice;
            IsFetchingUuids = false;
            _Receiver = new Receiver(this);
            this.Name = bluetoothDevice.Name;
            this.Address = bluetoothDevice.Address;
            ParcelUuid[] uuids = bluetoothDevice.GetUuids();
            if (uuids != null)
            {
                Guid[] guids = new Guid[uuids.Length];
                for (int i = 0; i < uuids.Length; i++)
                {
                    Guid guid = Guid.Parse(uuids[i].Uuid.ToString());
                    guids[i] = guid;
                }
                LastestFetchedUuids = guids;
            }
        }

        public void FetchUuidsWithSdp()
        {
            if (IsFetchingUuids)
            {
                return;
            }
            IsFetchingUuids = true;
            IntentFilter intentFilter = new IntentFilter(BluetoothDevice.ActionUuid);
            Application.Context.RegisterReceiver(_Receiver, intentFilter);
            BluetoothDevice.FetchUuidsWithSdp();
        }

        public void stopFetchingUuidsWithSdp()
        {
            if(!IsFetchingUuids)
            {
                return;
            }
            IsFetchingUuids = false;
            BluetoothAdapter.DefaultAdapter.CancelDiscovery();
            Application.Context.UnregisterReceiver(_Receiver);
        }



        private class Receiver : BroadcastReceiver
        {
            private BluetoothDeviceWrapper _DeviceWrapper;
            public Receiver(BluetoothDeviceWrapper deviceWrapper)
            {
                this._DeviceWrapper = deviceWrapper;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                string action = intent.Action;
                if (BluetoothDevice.ActionUuid == action)
                {
                    IParcelable[] parcelUuids = intent.GetParcelableArrayExtra(BluetoothDevice.ExtraUuid);
                    List<Guid> guids = new List<Guid>();
                    if (parcelUuids != null && parcelUuids.Length > 0)
                    {
                        foreach (IParcelable parcel in parcelUuids)
                        {
                            ParcelUuid parcelUuid = parcel as ParcelUuid;
                            if (parcelUuid != null)
                            {
                                guids.Add(Guid.Parse(parcelUuid.Uuid.ToString()));
                            }
                        }
                    }
                    Application.Context.UnregisterReceiver(this);
                    _DeviceWrapper.IsFetchingUuids = false;
                    _DeviceWrapper.LastestFetchedUuids = guids.ToArray();
                    _DeviceWrapper.OnUuidsFetched?.Invoke(_DeviceWrapper, guids.ToArray());
                }
            }
        }

    }
}