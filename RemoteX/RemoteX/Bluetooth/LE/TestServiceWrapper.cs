using RemoteX.Bluetooth.LE.Gatt;
using System;
using System.Collections.Generic;
using System.Text;
using RemoteX.Data;
using RemoteX.Data.Mathf;

namespace RemoteX.Bluetooth.LE
{
    class TestServiceWrapper
    {
        public static Guid SERVICE_UUID = BluetoothUtils.ShortValueUuid(0x110A);
        public IGattServerService GattServerService { get; }
        public TestCharacteristicWrapper TestCharacteristicWrapper;

        public KeepNotifyingCharacteristicWrapper KeepNotifyingCharacteristicWrapper { get; }

        public TestServiceWrapper(IBluetoothManager bluetoothManager)
        {
            IGattServiceBuilder builder = bluetoothManager.NewGattServiceBuilder();
            builder.SetUuid(SERVICE_UUID).SetServiceType(GattServiceType.Primary);
            TestCharacteristicWrapper = new TestCharacteristicWrapper(bluetoothManager);
            KeepNotifyingCharacteristicWrapper = new KeepNotifyingCharacteristicWrapper(bluetoothManager);
            builder.AddCharacteristics(TestCharacteristicWrapper.GattServerCharacteristic);
            builder.AddCharacteristics(KeepNotifyingCharacteristicWrapper.GattServerCharacteristic);
            GattServerService = builder.Build();
        }

        
    }
    public class TestCharacteristicWrapper
    {
        public static Guid UUID = BluetoothUtils.ShortValueUuid(0x3454);
        private static GattCharacteristicProperties PROPERTIES = new GattCharacteristicProperties
        {
            Read = true,
            Notify = true,
            Write = true
        };
        private static GattPermissions PERMISSIONS = new GattPermissions
        {
            Read = true,
            Write = true
        };
        public IGattServerCharacteristic GattServerCharacteristic { get; private set; }
        public ClientCharacteristicConfigurationDescriptorWrapper ClientCharacteristicConfigurationDescriptorWrapper { get; }

        public TestCharacteristicWrapper(IBluetoothManager bluetoothManager)
        {
            ClientCharacteristicConfigurationDescriptorWrapper = new ClientCharacteristicConfigurationDescriptorWrapper(bluetoothManager);

            IGattCharacteristicBuilder builder = bluetoothManager.NewGattCharacteristicBuilder();
            builder.SetUuid(UUID).SetPermissions(PERMISSIONS).SetProperties(PROPERTIES);
            builder.AddDescriptors(ClientCharacteristicConfigurationDescriptorWrapper.GattServerDescriptor);
            GattServerCharacteristic = builder.Build();
            GattServerCharacteristic.OnRead += _OnRead;
        }
        private DateTime _LastNotifyTime;
        public void SetMouseSpeed(Vector2 mouseVelocity)
        {
            List<byte> bytesList = new List<byte>();
            bytesList.AddRange(BitConverter.GetBytes(mouseVelocity.x));
            bytesList.AddRange(BitConverter.GetBytes(mouseVelocity.y));
            GattServerCharacteristic.Value = bytesList.ToArray();
            NotifyAll();
            System.Diagnostics.Debug.WriteLine("XJNotifyDeltaTime:" + (DateTime.Now - _LastNotifyTime).TotalMilliseconds);
            _LastNotifyTime = DateTime.Now;

            
        }

        private void _OnRead(object sender, CharacteristicReadRequest e)
        {
            var device = e.Device;
            GattServerCharacteristic.Service.Server.SendResponse(e.Device, e.RequestId, Encoding.Default.GetBytes("How long can you mother fucker read"));
        }

        public void NotifyAll()
        {
            var clientConfigurations = ClientCharacteristicConfigurationDescriptorWrapper.ClientConfigurations;
            foreach (var pair in clientConfigurations)
            {
                if (pair.Value.Notifications)
                {
                    GattServerCharacteristic.NotifyValueChanged(pair.Key, false);
                }
            }
        }

        ///private void 
    }

}
