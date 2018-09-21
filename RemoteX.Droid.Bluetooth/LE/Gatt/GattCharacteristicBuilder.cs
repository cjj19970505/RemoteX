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
            throw new NotImplementedException();
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