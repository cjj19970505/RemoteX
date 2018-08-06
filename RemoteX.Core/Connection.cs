using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Core
{
    public interface IConnectionInfo
    {

    }
    public class Connection
    {
        public struct BluetoothConnectionInfo : IConnectionInfo
        {
            public ulong DeviceAddress { get; set; }
            public Guid Guid { get; set; }
            public override string ToString()
            {
                string s = "MAC:" + DeviceAddress + '\n';
                s += "UUID:" + Guid;
                return s;
            }
            public string DeviceAddressString
            {
                get
                {
                    byte[] decbyte = BitConverter.GetBytes(DeviceAddress);
                    string ans = "";
                    for (int i = 0; i < decbyte.Length - 2; i++)
                    {
                        ans += Convert.ToString(decbyte[decbyte.Length - 3 - i], 16);
                    }
                    for (int j = 2; j <= 14; j += 2)
                    {
                        ans = ans.Insert(j, ":");
                        j++;
                    }
                    ans = ans.ToUpper();
                    return ans;
                }
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
            Guid guid = new Guid(sGuid);
            BluetoothConnectionInfo info = new BluetoothConnectionInfo()
            {
                DeviceAddress = mac,
                Guid = guid
            };
            return info;
        }
    }
}
