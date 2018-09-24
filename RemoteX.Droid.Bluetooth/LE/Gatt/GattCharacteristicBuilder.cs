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
using static RemoteX.Droid.Bluetooth.LE.Gatt.GattServer.GattServerService;
using static RemoteX.Droid.Bluetooth.LE.Gatt.GattServer.GattServerService.GattServerCharacteristic;
using static RemoteX.Droid.Bluetooth.LE.Gatt.GattServer.GattServerService.GattServerCharacteristic.GattServerDescriptor;

namespace RemoteX.Droid.Bluetooth.LE.Gatt
{
    class GattCharacteristicBuilder : IGattCharacteristicBuilder
    {
        private List<IGattDescriptor> _DescriptorsList;

        public GattCharacteristicBuilder()
        {
            _DescriptorsList = new List<IGattDescriptor>();
        }

        public GattCharacteristicProperties Properties { get; set; }
        public Guid Uuid { get; set; }

        public IGattCharacteristicBuilder AddDescriptors(params IGattDescriptor[] gattDescriptors)
        {
            AddDescriptors(gattDescriptors as IEnumerable<IGattDescriptor>);
            return this;
        }

        public IGattCharacteristicBuilder AddDescriptors(IEnumerable<IGattDescriptor> gattDescriptors)
        {
            _DescriptorsList.AddRange(gattDescriptors);
            return this;
        }

        public IGattCharacteristic Build()
        {
            GattPermissions permissions = new GattPermissions
            {
                Read = true,
                Write = true
            };
            GattServerCharacteristic characteristic = new GattServerCharacteristic(Uuid, Properties, permissions);
            foreach(var descriptor in _DescriptorsList)
            {
                characteristic.AddDescriptor(descriptor as GattServerDescriptor);
            }
            return characteristic;
        }

        public IGattCharacteristicBuilder SetProperties(GattCharacteristicProperties properties)
        {
            Properties = properties;
            return this;
        }

        public IGattCharacteristicBuilder SetUuid(Guid uuid)
        {
            Uuid = uuid;
            return this;
        }
    }
}