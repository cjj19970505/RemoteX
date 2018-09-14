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
    static class Extensions
    {
        public static Java.Util.UUID ToJavaUuid(this Guid self)
        {
            return Java.Util.UUID.FromString(self.ToString());
        }

        public static Guid ToGuid(this Java.Util.UUID self)
        {
            return Guid.Parse(self.ToString());
        }

        public static ParcelUuid ToJavaParcelUuid(this Guid self)
        {
            return ParcelUuid.FromString(self.ToString());
        }
        public static Android.Bluetooth.GattProperty ToDroidGattProperty(this GattCharacteristicProperties self)
        {
            int propertiesCode = 0;
            if (self.Broadcast)
            {
                propertiesCode |= (int)Android.Bluetooth.GattProperty.Broadcast;
            }
            if (self.Read)
            {
                propertiesCode |= (int)Android.Bluetooth.GattProperty.Read;
            }
            if (self.WriteWithoutResponse)
            {
                propertiesCode |= (int)Android.Bluetooth.GattProperty.WriteNoResponse;
            }
            if (self.Write)
            {
                propertiesCode |= (int)Android.Bluetooth.GattProperty.Write;
            }
            if (self.Notify)
            {
                propertiesCode |= (int)Android.Bluetooth.GattProperty.Notify;
            }
            //....A lot undone

            return (Android.Bluetooth.GattProperty)propertiesCode;
        }

        public static Android.Bluetooth.GattPermission ToDroidGattPermission(this GattPermissions gattPermission)
        {
            int permissionCode = 0;
            if (gattPermission.Read)
            {
                permissionCode |= (int)Android.Bluetooth.GattPermission.Read;
            }
            if (gattPermission.Write)
            {
                permissionCode |= (int)Android.Bluetooth.GattPermission.Write;
            }
            return (Android.Bluetooth.GattPermission)permissionCode;
        }

        public static Android.Bluetooth.GattDescriptorPermission ToDroidGattDescriptorPermission(this GattPermissions gattPermission)
        {
            int permissionCode = 0;
            if (gattPermission.Read)
            {
                permissionCode |= (int)Android.Bluetooth.GattDescriptorPermission.Read;
            }
            if (gattPermission.Write)
            {
                permissionCode |= (int)Android.Bluetooth.GattDescriptorPermission.Write;
            }
            return (Android.Bluetooth.GattDescriptorPermission)permissionCode;
        }

        public static GattServerService GetFromUuid(this IEnumerable<GattServerService> self, Guid uuid)
        {
            foreach(var gattService in self)
            {
                if(gattService.Uuid == uuid)
                {
                    return gattService;
                }
            }
            return null;
        }

        public static GattServerCharacteristic GetFromUuid(this IEnumerable<GattServerCharacteristic> self, Guid uuid)
        {
            foreach (var gattCharacteristic in self)
            {
                if (gattCharacteristic.Uuid == uuid)
                {
                    return gattCharacteristic;
                }
            }
            return null;
        }

        public static GattServerDescriptor GetFromUuid(this IEnumerable<IGattDescriptor> self, Guid uuid)
        {
            foreach (var gattDescriptor in self)
            {
                if (gattDescriptor.Uuid == uuid)
                {
                    return gattDescriptor as GattServerDescriptor;
                }
            }
            return null;
        }

        public static GattServerDescriptor ToDescriptor(this Android.Bluetooth.BluetoothGattDescriptor self, GattServer server)
        {
            var service = server.GattServices.GetFromUuid(self.Characteristic.Service.Uuid.ToGuid());
            var characteristic = service.GattCharacteristics.GetFromUuid(self.Characteristic.Uuid.ToGuid());
            var descriptor = characteristic.Descriptors.GetFromUuid(self.Uuid.ToGuid());
            return descriptor;
            //var descriptor = characteristic.Descriptors.GetFromUuid(self.Uuid);

        }
    }
}