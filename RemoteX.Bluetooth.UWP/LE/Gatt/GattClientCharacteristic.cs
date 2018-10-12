using RemoteX.Bluetooth.LE.Gatt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace RemoteX.Bluetooth.Win10.LE.Gatt
{
    public partial class GattClient : IGattClient
    {
        public partial class GattClientService : IGattClientService
        {
            public class GattClientCharacteristic : IGattClientCharacteristic
            {
                public IGattServerDescriptor[] Descriptors => throw new NotImplementedException();

                public GattPermissions Permissions { get; }

                public RemoteX.Bluetooth.LE.Gatt.GattCharacteristicProperties CharacteristicProperties => throw new NotImplementedException();

                public int CharacteristicValueHandle => throw new NotImplementedException();



                public Guid Uuid
                {
                    get
                    {
                        return UwpGattCharacteristic.Uuid;
                    }
                }

                GattCharacteristic _UwpGattCharacteristic;

                public event EventHandler<byte[]> OnNotified;

                public GattCharacteristic UwpGattCharacteristic
                {
                    get
                    {
                        return _UwpGattCharacteristic;
                    }
                    set
                    {
                        if(_UwpGattCharacteristic != null)
                        {
                            _UwpGattCharacteristic.ValueChanged -= _UwpGattCharacteristic_ValueChanged;
                        }
                        value.ValueChanged += _UwpGattCharacteristic_ValueChanged;
                        _UwpGattCharacteristic = value;
                    }
                }

                public byte[] LatestValue { get; private set; }

                private void _UwpGattCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
                {
                    var reader = DataReader.FromBuffer(args.CharacteristicValue);
                    byte[] newValue = new byte[reader.UnconsumedBufferLength];
                    reader.ReadBytes(newValue);
                    LatestValue = newValue;
                    OnNotified?.Invoke(this, newValue);
                }

                public IGattClientService Service { get; }

                

                private GattClientCharacteristic(GattClientService service, GattCharacteristic uwpCharacteristic)
                {
                    UwpGattCharacteristic = uwpCharacteristic;
                    Service = service;
                }

                public static GattClientCharacteristic FromUwpCharacteristic(GattClientService service, GattCharacteristic uwpCharacteristic)
                {
                    var characteristic = service._Characteristics.GetFromUwpGattCharacteristic(uwpCharacteristic);
                    if(characteristic == null)
                    {
                        characteristic = new GattClientCharacteristic(service, uwpCharacteristic);
                        service._Characteristics.Add(characteristic);
                    }
                    return characteristic;
                }

                public async Task<ReadCharacteristicValueResult> ReadCharacteristicValueAsync()
                {
                    var uwpResult = await UwpGattCharacteristic.ReadValueAsync();
                    if(uwpResult.Status != GattCommunicationStatus.Success)
                    {
                        LatestValue = null;
                        return new ReadCharacteristicValueResult
                        {
                            Value = null
                        };
                    }
                    var valueBuffer = uwpResult.Value;
                    if(valueBuffer == null)
                    {
                        LatestValue = null;
                        return new ReadCharacteristicValueResult
                        {
                            Value = null
                        };
                    }
                    var reader = DataReader.FromBuffer(valueBuffer);
                    byte[] value = new byte[reader.UnconsumedBufferLength];
                    reader.ReadBytes(value);
                    LatestValue = value;
                    return new ReadCharacteristicValueResult
                    {

                        Value = value
                    };
                }
            }
        }
    }
    
}
