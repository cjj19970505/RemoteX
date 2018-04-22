using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Data
{
    public enum DataType
    {
        SensorRotationVector = 11, SensorGyroscope = 4, SetMouseBoundary = 102, MouseRotationVector = 103,
        MouseLeftDown = 302, MouseLeftUp = 304, MouseRightDown = 308, MouseRightUp = 316, MouseScrollVerticle = 428, MouseScrollHorizontal = 556
    };
    public struct Data
    {
        private static char BEGIN_CHAR = '(';
        private static char END_CHAR = ')';
        public int dataType;
        public float[] data;
        public Data(int dataType, float[] data)
        {
            this.dataType = dataType;
            this.data = (float[])data.Clone();
        }

        /// <summary>
        /// 将byte[]转化为Data[]
        /// </summary>
        /// <param name="sensorDataBytes"></param>
        /// <returns></returns>
        public static Data[] fromBytes(byte[] sensorDataBytes)
        {

            int currPos = 0;
            List<Data> sensorDataPacks = new List<Data>();
            int beginPos = 0;

            int loopCount = 0;
            while (beginPos < sensorDataBytes.Length)
            {
                currPos = beginPos;
                char possibleBeginChar = BitConverter.ToChar(sensorDataBytes, currPos);
                if (possibleBeginChar != BEGIN_CHAR)
                {
                    beginPos++;
                    continue;
                }
                currPos += sizeof(char);
                //===============================================================
                int sensorType = BitConverter.ToInt32(sensorDataBytes, currPos);
                currPos += sizeof(int);
                //================================================================
                int dataCount = typeToCount(sensorType);
                if (dataCount < 0)
                {
                    return null;
                }
                float[] sensorData = new float[dataCount];
                for (int i = 0; i < dataCount; i++)
                {
                    sensorData[i] = BitConverter.ToSingle(sensorDataBytes, currPos + sizeof(float) * i);
                }
                currPos += dataCount * sizeof(float);
                //===============================================================
                char possibleEndChar = BitConverter.ToChar(sensorDataBytes, currPos);
                if (possibleEndChar != END_CHAR)
                {
                    beginPos++;
                    continue;
                }
                currPos += sizeof(char);
                beginPos = currPos;
                Data sensorDataPack = new Data(sensorType, sensorData);
                sensorDataPacks.Add(sensorDataPack);
                loopCount++;
            }
            if (loopCount > 1)
            {
                System.Diagnostics.Debug.WriteLine("LOOP COUNT: " + loopCount);
            }
            return sensorDataPacks.ToArray();
        }

        public static byte[] encodeSensorData(int sensorType, float[] sensorData)
        {
            byte[] beginCharBytes = BitConverter.GetBytes(BEGIN_CHAR);
            byte[] sensorTypeBytes = BitConverter.GetBytes(sensorType);
            byte[][] sensorDataBytesArray = new byte[sensorData.Length][];
            for (int i = 0; i < sensorData.Length; i++)
            {
                byte[] singleSensorDataBytes = BitConverter.GetBytes(sensorData[i]);
                sensorDataBytesArray[i] = (byte[])singleSensorDataBytes.Clone();
            }
            byte[] sensorDataBytes = mergeBytes(sensorDataBytesArray);
            byte[] endCharBytes = BitConverter.GetBytes(END_CHAR);
            byte[] dataBytes = mergeBytes(beginCharBytes, sensorTypeBytes, sensorDataBytes, endCharBytes);
            return dataBytes;
        }
        public static byte[] encodeSensorData(Data data)
        {
            return encodeSensorData(data.dataType, data.data);
        }

        /// <summary>
        /// 每个DataType决定了数据的数量，这个函数用来决定这些数据类型与数据数量的对应关系
        /// 例如，旋转向量传感器数据 的数据数量是4
        /// </summary>
        /// <param name="sensorType"></param>
        /// <returns></returns>
        private static int typeToCount(int sensorType)
        {
            switch (sensorType)
            {
                case (int)DataType.SensorRotationVector:
                    return 4;
                case (int)DataType.SetMouseBoundary:
                    return 16;
                case (int)DataType.MouseRotationVector:
                    return 4;
                case (int)DataType.SensorGyroscope:
                    return 3;
                case (int)DataType.MouseLeftDown:
                    return 1;
                case (int)DataType.MouseLeftUp:
                    return 1;
                case (int)DataType.MouseRightDown:
                    return 1;
                case (int)DataType.MouseRightUp:
                    return 1;
                case (int)DataType.MouseScrollVerticle:
                    return 1;
                case (int)DataType.MouseScrollHorizontal:
                    return 1;

            }
            if (sensorType >= 1000 && sensorType < 2000)
            {
                return 1;
            }
            return -1;
        }
        private static byte[] mergeBytes(params byte[][] dataBytesArray)
        {
            int totalLength = 0;
            for (int i = 0; i < dataBytesArray.Length; i++)
            {
                totalLength += dataBytesArray[i].Length;
            }
            byte[] finalBytes = new byte[totalLength];
            int copyedByteCount = 0;
            for (int i = 0; i < dataBytesArray.Length; i++)
            {
                Array.Copy(dataBytesArray[i], 0, finalBytes, copyedByteCount, dataBytesArray[i].Length);
                copyedByteCount += dataBytesArray[i].Length;
            }
            return finalBytes;
        }
        private static byte[] getBytesRange(byte[] bytes, int pos, int length)
        {
            byte[] rangeBytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                rangeBytes[i] = bytes[pos + i];
            }
            return rangeBytes;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(TYPE: " + dataType + " | DATA: ");
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i]);
                if (i != data.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
