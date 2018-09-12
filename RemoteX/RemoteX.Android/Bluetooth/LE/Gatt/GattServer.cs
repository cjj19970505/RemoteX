using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using RemoteX.Bluetooth.LE.Gatt;

namespace RemoteX.Droid.Bluetooth.LE.Gatt
{
    internal class GattServer : IGattServer
    {
        public ulong Address => throw new NotImplementedException();

        internal Android.Bluetooth.BluetoothGattServer DroidGattServer { get; private set; }
        internal BluetoothManager BluetoothManager { get; private set; }
        private ServerCallback _ServerCallback;
        private AdvertiserCallback _AdvertiserCallback;

        internal GattServer(BluetoothManager bluetoothManager)
        {
            _ServerCallback = new ServerCallback();
            _AdvertiserCallback = new AdvertiserCallback();
            BluetoothManager = bluetoothManager;
            DroidGattServer = BluetoothManager.DroidBluetoothManager.OpenGattServer(Application.Context, _ServerCallback);

        }

        public void StartAdvertising()
        {
            Guid testGuid = Guid.NewGuid();
            Android.Bluetooth.LE.AdvertiseSettings advertiseSettings = new Android.Bluetooth.LE.AdvertiseSettings.Builder()
                .SetTxPowerLevel(Android.Bluetooth.LE.AdvertiseTx.PowerHigh)
                .SetConnectable(true)
                .SetTimeout(0)
                .SetAdvertiseMode(Android.Bluetooth.LE.AdvertiseMode.LowLatency)
                .Build();
            Android.Bluetooth.LE.AdvertiseData advertiseData = new Android.Bluetooth.LE.AdvertiseData.Builder()
                .SetIncludeTxPowerLevel(false)
                .SetIncludeDeviceName(true)
                .AddServiceUuid(ParcelUuid.FromString(testGuid.ToString()))
                .Build();
            Android.Bluetooth.LE.AdvertiseData scanResult = new Android.Bluetooth.LE.AdvertiseData.Builder()
                .AddServiceUuid(ParcelUuid.FromString(testGuid.ToString()))
                .Build();
            var advertisier = BluetoothManager.BluetoothAdapter.BluetoothLeAdvertiser;
            advertisier.StartAdvertising(advertiseSettings, advertiseData, _AdvertiserCallback);
            Log.Info("BLEAdver", "Ithinkitworks");
            

        }


        public void AddService(IGattService service)
        {
            DroidGattServer.AddService((service as GattService).DroidService);
        }

        private class ServerCallback : Android.Bluetooth.BluetoothGattServerCallback
        {
            public override void OnCharacteristicReadRequest(BluetoothDevice device, int requestId, int offset, BluetoothGattCharacteristic characteristic)
            {
                base.OnCharacteristicReadRequest(device, requestId, offset, characteristic);
            }
        }

        private class AdvertiserCallback : Android.Bluetooth.LE.AdvertiseCallback
        {
            public override void OnStartSuccess(AdvertiseSettings settingsInEffect)
            {
                base.OnStartSuccess(settingsInEffect);
            }
            public override void OnStartFailure([GeneratedEnum] AdvertiseFailure errorCode)
            {
                base.OnStartFailure(errorCode);
            }

        }
    }
}