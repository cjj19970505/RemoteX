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
using RemoteX.Bluetooth;
using RemoteX.Bluetooth.LE.Gatt;

namespace RemoteX.Droid.Bluetooth.LE.Gatt
{
    internal class BatteryService:GattServerService
    {
        public static Guid BATTERY_SERVICE_UUID = BluetoothUtils.ShortValueUuid(0x180F);
        public BatteryService(GattServer server):base(server, BATTERY_SERVICE_UUID)
        {
            AddCharacteristic(new BatteryLevelCharacteristic(this));
        }

        public class BatteryLevelCharacteristic : GattServerCharacteristic
        {
            public static Guid BATTERY_LEVEL_UUID = BluetoothUtils.ShortValueUuid(0x2A19);
            private static GattCharacteristicProperties PROPERTIES = new GattCharacteristicProperties
            {
                Read = true,
                Notify = true,
                WriteWithoutResponse = true,
                //Write = true
            };
            private static GattPermissions PERMISSIONS = new GattPermissions
            {
                //Read = true,
                Write = true

            };
            public BatteryLevelCharacteristic(BatteryService service):base(service, BATTERY_LEVEL_UUID, PROPERTIES, PERMISSIONS)
            {
                _BatteryLevel = 89;
                AddDescriptor(new ClientCharacteristicConfigurationDescriptor(this));
                DroidCharacteristic.SetValue(BitConverter.GetBytes(BatteryLevel));
            }
            
            int _BatteryLevel;
            int BatteryLevel
            {
                get
                {
                    return _BatteryLevel;
                }
                set
                {
                    _BatteryLevel = value;
                }
            }
            internal override void OnCharacteristicRead(BluetoothDevice device, int requestId, int offset)
            {
                base.OnCharacteristicRead(device, requestId, offset);
                (Service.Server as GattServer).DroidGattServer.SendResponse(device, requestId, GattStatus.Success, offset, new byte[] { BitConverter.GetBytes(BatteryLevel)[0] });
            }
        }
    }
}