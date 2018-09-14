using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Bluetooth
{
    public class BluetoothUtils
    {
        public const string UUID_LONG_STYLE_PREFIX = "0000";
        public const string UUID_LONG_STYLE_POSTFIX = "-0000-1000-8000-00805F9B34FB";

        public static Guid ShortValueUuid(int uuidShortValue)
        {
            return Guid.Parse(UUID_LONG_STYLE_PREFIX + string.Format("{0:x4}", uuidShortValue & 0xffff) + UUID_LONG_STYLE_POSTFIX);
        }
    }
}
