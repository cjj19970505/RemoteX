using RemoteX.Bluetooth.Win10.LE.Gatt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace RemoteX.Bluetooth.Win10
{
    static class Extensions
    {
        public static BluetoothManager.BluetoothDevice GetFromAddress(this IEnumerable<BluetoothManager.BluetoothDevice> self, ulong address)
        {
            foreach(var device in self)
            {
                if(device.Address == address)
                {
                    return device;
                }
            }
            return null;
        }

        public static BluetoothManager.BluetoothDevice GetFromUwpLEDevice(this IEnumerable<BluetoothManager.BluetoothDevice> self, BluetoothLEDevice uwpLeDevice)
        {
            foreach(var device in self)
            {
                if(device.Address == uwpLeDevice.BluetoothAddress)
                {
                    return device;
                }
            }
            return null;
        }

        public static GattClient.GattClientService GetFromUwpGattService(this IEnumerable<GattClient.GattClientService> self, GattDeviceService uwpGattService)
        {
            foreach(var service in self)
            {
                if(service.UwpService.AttributeHandle == uwpGattService.AttributeHandle)
                {
                    return service;
                }
            }
            return null;
        }

        public static GattClient.GattClientService.GattClientCharacteristic GetFromUwpGattCharacteristic(this IEnumerable<GattClient.GattClientService.GattClientCharacteristic> self, GattCharacteristic uwpGattCharacteristic)
        {
            foreach(var characteristic in self)
            {
                if(characteristic.UwpGattCharacteristic.AttributeHandle == uwpGattCharacteristic.AttributeHandle)
                {
                    return characteristic;
                }
            }
            return null;
        }
    }
}
