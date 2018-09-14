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
using RemoteX.Bluetooth.LE.Gatt;

namespace RemoteX.Droid.Bluetooth.LE.Gatt
{
    class GattServerCharacteristic : IGattCharacteristic
    {
        public Android.Bluetooth.BluetoothGattCharacteristic DroidCharacteristic { get; private set; }

        public IGattService[] ReferencedServices => throw new NotImplementedException();

        private List<GattServerDescriptor> _Descritptor;
        private Guid bATTERY_LEVEL_UUID;

        public IGattDescriptor[] Descriptors
        {
            get
            {
                return _Descritptor.ToArray();
            }
        }

        public GattCharacteristicProperties CharacteristicProperties => throw new NotImplementedException();

        public int CharacteristicValueHandle
        {
            get
            {
                return DroidCharacteristic.InstanceId;
            }
        }

        public Guid Uuid
        {
            get
            {
                
                return Guid.Parse(DroidCharacteristic.Uuid.ToString());
            }
        }

        public byte[] Value => throw new NotImplementedException();

        public IGattService Service { get; private set; }

        public GattServerCharacteristic(GattServerService service, Guid uuid, GattCharacteristicProperties properties , GattPermissions permission)
        {
            Service = service;
            DroidCharacteristic = new Android.Bluetooth.BluetoothGattCharacteristic(uuid.ToJavaUuid(), properties.ToDroidGattProperty(), permission.ToDroidGattPermission());
            _Descritptor = new List<GattServerDescriptor>();
        }

        public GattServerCharacteristic(BatteryService service, Guid bATTERY_LEVEL_UUID)
        {
            Service = service;
            this.bATTERY_LEVEL_UUID = bATTERY_LEVEL_UUID;
        }

        public void AddDescriptor(GattServerDescriptor descriptor)
        {
            _Descritptor.Add(descriptor);
            DroidCharacteristic.AddDescriptor(descriptor.DroidDescriptor);
        }

        internal virtual void OnCharacteristicRead(Android.Bluetooth.BluetoothDevice device, int requestId, int offset)
        {

        }


    }
}