using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;

namespace RemoteX.Bluetooth.Win10
{
    public partial class BluetoothManager:IBluetoothManager
    {
        
        public class BluetoothDevice : IBluetoothDevice
        {
            public BluetoothLEDevice UwpBleDevice { get; }

            public string Name => throw new NotImplementedException();

            public string Address => throw new NotImplementedException();

            public Guid[] LastestFetchedUuids => throw new NotImplementedException();

            public bool IsFetchingUuids => throw new NotImplementedException();

            public event BluetoothDeviceGetUuidsHanlder OnUuidsFetched;

            public void FetchUuidsWithSdp()
            {
                throw new NotImplementedException();
            }

            public void stopFetchingUuidsWithSdp()
            {
                throw new NotImplementedException();
            }
        }
    }
    
}
