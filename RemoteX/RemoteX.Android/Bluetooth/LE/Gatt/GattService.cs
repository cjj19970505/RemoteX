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
    internal class GattService : IGattService
    {
        public Android.Bluetooth.BluetoothGattService DroidService { get; private set; }

        public GattServiceType ServiceType
        {
            get
            {
                if(DroidService.Type == Android.Bluetooth.GattServiceType.Primary)
                {
                    return GattServiceType.Primary;
                }
                else
                {
                    return GattServiceType.Secondary;
                }
            }
        }

        public IGattCharacteristic[] MandatoryCharacteristics => throw new NotImplementedException();

        public IGattCharacteristic[] OptionalCharacteristics => throw new NotImplementedException();

        public GattService(Guid uuid)
        {
            DroidService = new Android.Bluetooth.BluetoothGattService(Java.Util.UUID.FromString(uuid.ToString()), Android.Bluetooth.GattServiceType.Primary);
        }
    }
}