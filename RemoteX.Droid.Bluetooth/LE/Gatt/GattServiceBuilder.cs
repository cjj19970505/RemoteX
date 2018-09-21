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
using static RemoteX.Droid.Bluetooth.LE.Gatt.GattServer;
using static RemoteX.Droid.Bluetooth.LE.Gatt.GattServer.GattServerService;

namespace RemoteX.Droid.Bluetooth.LE.Gatt
{
    class GattServiceBuilder : IGattServiceBuilder
    {
        public GattServiceType ServiceType { get; set; }
        public Guid Uuid { get; set; }
        private List<IGattCharacteristic> _CharacteristicsList;
        public GattServiceBuilder(GattServer gattServer)
        {
            _CharacteristicsList = new List<IGattCharacteristic>();
        }

        public IGattServiceBuilder AddCharacteristics(params IGattCharacteristic[] characteristics)
        {
            AddCharacteristics(characteristics as IEnumerable<IGattCharacteristic>);
            return this;
        }

        public IGattServiceBuilder AddCharacteristics(IEnumerable<IGattCharacteristic> characteristics)
        {
            _CharacteristicsList.AddRange(characteristics);
            return this;
        }

        public IGattService Build()
        {
            GattServerService gattServerService = new GattServerService(Uuid);
            
            foreach (var characteristic in _CharacteristicsList)
            {
                gattServerService.AddCharacteristic(characteristic as GattServerCharacteristic);
            }
            return gattServerService;

        }

        public IGattServiceBuilder SetServiceType(GattServiceType gattServiceType)
        {
            ServiceType = gattServiceType;
            return this;
        }

        public IGattServiceBuilder SetUuid(Guid uuid)
        {
            Uuid = uuid;
            return this;
        }
    }
}