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
using RemoteX.Droid.Bluetooth.LE;

namespace RemoteX.Droid.Bluetooth.LE.Gatt
{
    internal class GattServerService : IGattService
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

        public Guid Uuid
        {
            get
            {
                return DroidService.Uuid.ToGuid();
            }
        }

        public IGattServer Server { get; private set; }

        public List<GattServerCharacteristic> GattCharacteristics;

        public GattServerService(GattServer server, Guid uuid)
        {

            GattCharacteristics = new List<GattServerCharacteristic>();
            Server = server;
            DroidService = new Android.Bluetooth.BluetoothGattService(Java.Util.UUID.FromString(uuid.ToString()), Android.Bluetooth.GattServiceType.Primary);
        }

        public void AddCharacteristic(GattServerCharacteristic characteristic)
        {
            GattCharacteristics.Add(characteristic);
            DroidService.AddCharacteristic(characteristic.DroidCharacteristic);
        }
    }
}