using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Data
{
    public enum DataType
    {
        SensorRotationVector = 11, SensorGyroscope = 4, OrientationAngle = 50, Velocity = 51, SetMouseBoundary = 102, MouseRotationVector = 103,
        MouseLeftDown = 302, MouseLeftUp = 304, MouseRightDown = 308, MouseRightUp = 316, MouseScrollVerticle = 428, MouseScrollHorizontal = 556,
        TouchMouseSpeed = 601
    };
    /// <summary>
    /// 控制器传送的控制信息遵守这个格式
    /// </summary>
    public struct RemoteXControlMessage
    {
        private static char BEGIN_CHAR = '(';
        private static char END_CHAR = ')';

        public int DataType { get; set; }
        private float[] _Values;
        public float[] Values
        {
            get
            {
                return (float[])_Values.Clone();
            }
        }
        public RemoteXControlMessage(int dataType, float[] values)
        {
            this.DataType = dataType;
            this._Values = (float[])values.Clone();
        }

        public byte[] Bytes
        {
            get
            {
                byte[] beginCharBytes = BitConverter.GetBytes(BEGIN_CHAR);
                byte[] dataTypeBytes = BitConverter.GetBytes(DataType);
                byte[] valueCountBytes = BitConverter.GetBytes(_Values.Length);
                byte[][] valuesBytesArray = new byte[_Values.Length][];
                for (int i = 0; i < _Values.Length; i++)
                {
                    byte[] singleValueDataBytes = BitConverter.GetBytes(_Values[i]);
                    valuesBytesArray[i] = (byte[])singleValueDataBytes.Clone();
                }
                byte[] valuesBytes = mergeBytes(valuesBytesArray);
                byte[] endCharBytes = BitConverter.GetBytes(END_CHAR);
                byte[] dataBytes = mergeBytes(beginCharBytes, dataTypeBytes, valueCountBytes, valuesBytes, endCharBytes);
                return dataBytes;
            }
        }

        public static RemoteXControlMessage[] FromBytes(byte[] controlMessagesBytes)
        {

            int currPos = 0;
            List<RemoteXControlMessage> controlMessages = new List<RemoteXControlMessage>();
            int beginPos = 0;

            int loopCount = 0;
            while (beginPos < controlMessagesBytes.Length)
            {
                currPos = beginPos;
                char possibleBeginChar = BitConverter.ToChar(controlMessagesBytes, currPos);
                if (possibleBeginChar != BEGIN_CHAR)
                {
                    beginPos++;
                    continue;
                }
                currPos += sizeof(char);
                //===============================================================
                int dataType = BitConverter.ToInt32(controlMessagesBytes, currPos);
                currPos += sizeof(int);
                //================================================================
                int valueCount = BitConverter.ToInt32(controlMessagesBytes, currPos);
                currPos += sizeof(int);
                //================================================================
                float[] sensorData = new float[valueCount];
                for (int i = 0; i < valueCount; i++)
                {
                    sensorData[i] = BitConverter.ToSingle(controlMessagesBytes, currPos + sizeof(float) * i);
                }
                currPos += valueCount * sizeof(float);
                //===============================================================
                char possibleEndChar = BitConverter.ToChar(controlMessagesBytes, currPos);
                if (possibleEndChar != END_CHAR)
                {
                    beginPos++;
                    continue;
                }
                currPos += sizeof(char);
                beginPos = currPos;
                RemoteXControlMessage controlMessage = new RemoteXControlMessage(dataType, sensorData);
                controlMessages.Add(controlMessage);
                loopCount++;
            }
            if (loopCount > 1)
            {
                System.Diagnostics.Debug.WriteLine("LOOP COUNT: " + loopCount);
            }
            return controlMessages.ToArray();
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(TYPE: " + DataType + " | DATA: ");
            for (int i = 0; i < _Values.Length; i++)
            {
                sb.Append(_Values[i]);
                if (i != _Values.Length - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(")");
            return sb.ToString();
        }

        public static RemoteXControlMessage FromString(string sControlMessage)
        {
            string[] tempSs = sControlMessage.Split('|');
            sControlMessage = sControlMessage.Remove(0, 1);
            sControlMessage = sControlMessage.Remove(sControlMessage.Length - 1, 1);
            string[] sTypeAndData = sControlMessage.Split('|');
            string sType = sTypeAndData[0];
            sType = sType.Remove(0, 5);
            sType = sType.Trim();
            int type = int.Parse(sType);
            System.Diagnostics.Debug.WriteLine(sType);
            string sDataOnly = sTypeAndData[1].Remove(0, 7);
            sDataOnly = sDataOnly.Replace(" ", "");
            string[] sFloatDatas = sDataOnly.Split(',');
            float[] values = new float[sFloatDatas.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = float.Parse(sFloatDatas[i]);
            }
            RemoteXControlMessage restoredData = new RemoteXControlMessage(type, values);
            return restoredData;
        }
    }
}
