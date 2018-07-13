using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Data
{
    public class Connection
    {
        public struct BluetoothConnectionInfo
        {
            public ulong DeviceAddress { get; set; }
            public Guid Guid { get; set; }
            public override string ToString()
            {
                string s = "MAC:" + DeviceAddress + '\n';
                s += "UUID:" + Guid;
                return s;
            }
        }
        public static string EncodeBluetoothConnection(ulong deviceAddress, Guid guid)
        {
            string s = "MAC:" + deviceAddress + "|";
            s += "UUID:" + guid;
            return s;
        }
        public static BluetoothConnectionInfo DecodeBluetoothConnection(string encodedBluetoothConnection)
        {
            string[] arrMacAndUuid = encodedBluetoothConnection.Split('|');
            string sMac = arrMacAndUuid[0].Remove(0, 4);
            string sGuid = arrMacAndUuid[1].Remove(0, 5);
            ulong mac = ulong.Parse(sMac);
            Guid guid = Guid.Parse(sGuid);
            BluetoothConnectionInfo info = new BluetoothConnectionInfo()
            {
                DeviceAddress = mac,
                Guid = guid
            };
            return info;
        }


    }
}
