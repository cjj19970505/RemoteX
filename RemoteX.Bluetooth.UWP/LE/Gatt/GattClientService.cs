using RemoteX.Bluetooth.LE.Gatt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using RemoteX.Bluetooth;

namespace RemoteX.Bluetooth.Win10.LE.Gatt
{
    public partial class GattClient : IGattClient
    {
        public partial class GattClientService : IGattClientService
        {
            public GattDeviceService UwpService { get; }
            public GattClient GattClient { get; }
            private List<GattClientCharacteristic> _Characteristics;
            public GattServiceType ServiceType => throw new NotImplementedException();
            public Guid Uuid
            {
                get
                {
                    return UwpService.Uuid;
                }
            }
            

            private GattClientService(GattClient gattClient, GattDeviceService uwpService)
            {
                GattClient = GattClient;
                UwpService = uwpService;
                _Characteristics = new List<GattClientCharacteristic>();
            }

            public static GattClientService FromUwpGattService(GattClient gattClient, GattDeviceService uwpService)
            {
                var service = gattClient._ClientServices.GetFromUwpGattService(uwpService);
                if(service == null)
                {
                    service = new GattClientService(gattClient, uwpService);
                }
                return service;

            }

            public async Task<IGattClientCharacteristic[]> DiscoverAllCharacteristicsAsync()
            {
                //throw new NotImplementedException();
                var result = await UwpService.GetCharacteristicsAsync();
                List<IGattClientCharacteristic> characteristicList = new List<IGattClientCharacteristic>();
                foreach(var uwpCharacteristic in result.Characteristics)
                {
                    characteristicList.Add(GattClientCharacteristic.FromUwpCharacteristic(this, uwpCharacteristic));
                }
                return characteristicList.ToArray();
            }

            
        }
    }
    
}
