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
using RemoteX.Droid.Bluetooth;
using RemoteX.Droid;

namespace RemoteX.Droid.Bluetooth.LE.Gatt
{
    public partial class GattServer : IGattServer
    {
        public partial class GattServerService : IGattServerService
        {
            public partial class GattServerCharacteristic : IGattServerCharacteristic
            {
                public Android.Bluetooth.BluetoothGattCharacteristic DroidCharacteristic { get; private set; }

                private List<GattServerDescriptor> _Descritptor;

                public event EventHandler<ReadRequest> OnRead;
                public event EventHandler<WriteRequest> OnWrite;

                public IGattServerDescriptor[] Descriptors
                {
                    get
                    {
                        return _Descritptor.ToArray();
                    }
                }

                public GattPermissions Permissions
                {
                    get
                    {
                        return DroidCharacteristic.Permissions.ToGattPermissions();
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
                public IGattServerService Service { get; private set; }

                public GattServerCharacteristic(Guid uuid, GattCharacteristicProperties properties, GattPermissions permission)
                {
                    DroidCharacteristic = new Android.Bluetooth.BluetoothGattCharacteristic(uuid.ToJavaUuid(), properties.ToDroidGattProperty(), permission.ToDroidGattPermission());
                    _Descritptor = new List<GattServerDescriptor>();
                    AddDescriptor(new ClientCharacteristicConfigurationDescriptor());
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
                    ReadRequest readRequest = new ReadRequest
                    {
                        Device = BluetoothManager.BluetoothDeviceWrapper.GetBluetoothDeviceFromDroidDevice((Service.Server as GattServer).BluetoothManager, device),
                        Offset = offset,
                        RequestId = requestId,
                    };
                    OnRead?.Invoke(this, readRequest);
                    //Service.Server.SendResponse(BluetoothManager.BluetoothDeviceWrapper.GetBluetoothDeviceFromDroidDevice((Service.Server as GattServer).BluetoothManager, device), requestId, null);
                }

                public virtual void OnCharacteristicWrite(Android.Bluetooth.BluetoothDevice droidDevice, int requestId, Android.Bluetooth.BluetoothGattCharacteristic characteristic, bool preparedWrite, bool responseNeeded, int offset, byte[] value)
                {
                    var device = BluetoothManager.BluetoothDeviceWrapper.GetBluetoothDeviceFromDroidDevice((Service.Server as GattServer).BluetoothManager, droidDevice);
                    WriteRequest writeRequest = new WriteRequest
                    {
                        Device = device,
                        Offset = offset,
                        ResponseNeeded = responseNeeded,
                        RequestId = requestId,
                        Value = value
                    };
                    OnWrite?.Invoke(this, writeRequest);
                }


            }
        }
    }
    
}