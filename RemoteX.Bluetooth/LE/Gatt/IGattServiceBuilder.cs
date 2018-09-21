using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Bluetooth.LE.Gatt
{


    public interface IGattServiceBuilder
    {
        GattServiceType ServiceType { get; set; }
        Guid Uuid { get; set; }
        
        IGattServiceBuilder SetServiceType(GattServiceType gattServiceType);
        IGattServiceBuilder SetUuid(Guid uuid);

        /// <summary>
        /// 这个的实现就直接调用下面的参数为IEnumerable版本就好了
        /// </summary>
        /// <param name="characteristics"></param>
        /// <returns></returns>
        IGattServiceBuilder AddCharacteristics(params IGattCharacteristic[] characteristics);
        IGattServiceBuilder AddCharacteristics(IEnumerable<IGattCharacteristic> characteristics);
        IGattService Build();
    }

    public interface IGattCharacteristicBuilder
    {
        GattCharacteristicProperties Properties { get; set; }
        Guid Uuid { get; set; }
        IGattCharacteristicBuilder SetUuid(Guid uuid);
        IGattCharacteristicBuilder SetProperties(GattCharacteristicProperties properties);
        IGattCharacteristicBuilder AddDescriptors(params IGattDescriptor[] gattDescriptors);
        IGattCharacteristicBuilder AddDescriptors(IEnumerable<IGattDescriptor> gattDescriptors);
        IGattCharacteristic Build();
    }

    public interface IGattDescriptorBuilder
    {
        IGattDescriptorBuilder SetUuid(Guid uuid);
        IGattDescriptor Build();
    }
}
