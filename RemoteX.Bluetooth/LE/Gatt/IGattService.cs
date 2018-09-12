using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteX.Bluetooth.LE.Gatt
{
    /// <summary>
    /// Primary services can be discovered using Primary Service Discovery procedures.
    /// A sencondary service is a service that is only intended to be referenced from a primary service or another secondary service or other higher layer specification
    /// From Core_v5.0.pdf Page2230
    /// </summary>
    public enum GattServiceType { Primary, Secondary}
    public interface IGattClient
    {
        /// <summary>
        /// 4.8.1
        /// This sub-procedure is used to read a Characteristic Value from a server when the client knows the Characteristic Value Handle. The Attribute Protocol Read Request is used with the Attribute Handle parameter set to the Characteristic Value Handle. The Read Response returns the Characteristic Value in the Attribute Value parameter.
        /// </summary>
        /// <param name="characteristicValueHandle"></param>
        /// <returns></returns>
        Task<byte[]> ReadCharacteristicValue(int characteristicValueHandle);

        /// <summary>
        /// 4.8.2
        /// This sub-procedure is used to read a Characteristic Value from a server when the client only knows the characteristic UUID and does not know the handle of the characteristic. 
        /// </summary>
        /// <param name="CharacteristicUuid"></param>
        /// <returns></returns>
        Task<byte[]> ReadCharacteristicValue(Guid characteristicUuid);
    }

    public interface IGattService
    {
        GattServiceType ServiceType { get; }
        IGattCharacteristic[] MandatoryCharacteristics { get; }
        IGattCharacteristic[] OptionalCharacteristics { get; }
    }

    public interface IGattCharacteristic
    {
        IGattService[] ReferencedServices { get; }
        IGattDescriptor[] Descriptors { get; }

        //3.3.1 Characteristic declaration
        //Attribute Value Field
        /// <summary>
        /// 3.3.1.1
        /// The Characteristic Properties bit field determines how the Characteristic Value can be used, or how the characteristic descriptors (see Section 3.3.3) can be accessed. If the bits defined in Table 3 5 are set, the action described is permitted. Multiple Characteristic Properties can be set.
        /// </summary>
        GattCharacteristicProperties CharacteristicProperties { get; }
        /// <summary>
        /// 3.3.1.2
        /// The Characteristic Value Attribute Handle field is the Attribute Handle of the Attribute that contains the Characteristic Value.
        /// </summary>
        int CharacteristicValueHandle { get; }
        /// <summary>
        /// 3.3.1.3
        /// The Characteristic UUID field is a 16-bit Bluetooth UUID or 128-bit UUID that describes the type of Characteristic Value
        /// </summary>
        Guid Uuid { get; }

        //3.3.2 Characteristic Value Declaration
        byte[] Value { get; }

        //3.3.3 Characteristic Descriptor Declarations


    }

    public interface IGattDescriptor
    {
        Guid Uuid { get; }
    }

    /// <summary>
    /// 3.3.1.1
    /// The Characteristic Properties bit field determines how the Characteristic Value can be used, or how the characteristic descriptors (see Section 3.3.3) can be accessed. If the bits defined in Table 3 5 are set, the action described is permitted. Multiple Characteristic Properties can be set.
    /// </summary>
    public struct GattCharacteristicProperties
    {
        public bool Broadcast;
        public bool Read;
        public bool WriteWithoutResponse;
        public bool Write;
        public bool Notify;
        public bool Indicate;
        public bool AuthenticatedSignedWrites;
        public bool ExtendedProperties;
    }

    
}
