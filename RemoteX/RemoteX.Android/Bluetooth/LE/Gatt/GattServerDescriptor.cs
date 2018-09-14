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
using RemoteX.Bluetooth.LE.Gatt;

namespace RemoteX.Droid.Bluetooth.LE.Gatt
{
    class GattServerDescriptor : IGattDescriptor
    {
        public Guid Uuid
        {
            get
            {
                return DroidDescriptor.Uuid.ToGuid();
            }
        }

        [Obsolete("Not Finished Yet")]
        public GattPermissions Permissions
        {
            get
            {
                return new GattPermissions();
            }
        }

        public Android.Bluetooth.BluetoothGattDescriptor DroidDescriptor { get; private set; }

        public IGattCharacteristic Characteristic { get; private set; }

        public GattServerDescriptor(GattServerCharacteristic characteristic, Guid uuid, GattPermissions permissions)
        {
            Characteristic = characteristic;
            DroidDescriptor = new Android.Bluetooth.BluetoothGattDescriptor(uuid.ToJavaUuid(), permissions.ToDroidGattDescriptorPermission());
        }

        public virtual void OnWriteRequest(BluetoothDevice device, int requestId, bool preparedWrite, bool responseNeeded, int offset, byte[] value)
        {

        }

        public virtual void OnReadRequest(BluetoothDevice device, int requestId, int offset)
        {

        }

    }
}