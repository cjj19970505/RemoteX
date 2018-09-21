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
    public partial class GattServer : IGattServer
    {
        public partial class GattServerService:IGattService
        {
            public partial class GattServerCharacteristic : IGattCharacteristic
            {
                public Android.Bluetooth.BluetoothGattCharacteristic DroidCharacteristic { get; private set; }

                private List<GattServerDescriptor> _Descritptor;

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

                /// <summary>
                /// Only avalible when added to a service
                /// </summary>
                public IGattService Service { get; private set; }

                public GattServerCharacteristic(Guid uuid, GattCharacteristicProperties properties, GattPermissions permission)
                {
                    DroidCharacteristic = new Android.Bluetooth.BluetoothGattCharacteristic(uuid.ToJavaUuid(), properties.ToDroidGattProperty(), permission.ToDroidGattPermission());
                    _Descritptor = new List<GattServerDescriptor>();
                }

                public void AddDescriptor(GattServerDescriptor descriptor)
                {
                    descriptor.AddToCharacteristic(this);
                }

                public void AddToService(GattServerService service)
                {
                    Service = service;
                    service.GattCharacteristics.Add(this);
                    service.DroidService.AddCharacteristic(DroidCharacteristic);
                }

                public virtual void OnCharacteristicRead(Android.Bluetooth.BluetoothDevice device, int requestId, int offset)
                {

                }


            }
        }
    }
    
}