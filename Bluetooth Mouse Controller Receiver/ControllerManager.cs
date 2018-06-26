using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RemoteXDataLibary;

namespace Bluetooth_Mouse_Controller_Receiver
{
    
    class ControllerManager
    {
        public struct MouseBoundary
        {
            public Quaternion qUp;
            public Quaternion qDown;
            public Quaternion qLeft;
            public Quaternion qRight;
            public Quaternion qPointer;
            public Vector3 fixAxis;
            public MouseBoundary(Quaternion leftUp, Quaternion rightUp, Quaternion rightDown, Quaternion leftDown)
            {
                this.qUp = leftUp;
                this.qDown = rightUp;
                this.qLeft = rightDown;
                this.qRight = leftDown;
                fixAxis = new Vector3(0, 1, 0);
                this.qPointer = new Quaternion();
                
            }
            public Vector3 quaternionToVector(Quaternion quaternion)
            {
                return quaternionToMatrix(quaternion).MultiplyVector(fixAxis).normalized;
            }
            public Vector3 vPointer
            {
                get
                {
                    return quaternionToMatrix(qPointer).MultiplyVector(fixAxis).normalized;
                }
            }
            Quaternion this[int index]
            {
                get
                {
                    if(index == 0)
                    {
                        return qUp;
                    }
                    else if(index == 1)
                    {
                        return qDown;
                    }
                    else if(index == 2)
                    {
                        return qLeft;
                    }
                    else if(index == 3)
                    {
                        return qRight;
                    }
                    else
                    {
                        throw new StackOverflowException("Only 4 value in this");
                    }
                }
            }
            public static Matrix4x4 quaternionToMatrix(Quaternion rot)
            {
                float x = rot.x;
                float y = rot.y;
                float z = rot.z;
                float w = rot.w;
                Vector4 c1 = new Vector4(1 - 2 * y * y - 2 * z * z, 2 * x * y + 2 * w * z, 2 * x * z - 2 * w * y, 0);
                Vector4 c2 = new Vector4(2 * x * y - 2 * w * z, 1 - 2 * x * x - 2 * z * z, 2 * y * z + 2 * w * x, 0);
                Vector4 c3 = new Vector4(2 * x * z + 2 * w * y, 2 * y * z - 2 * w * x, 1 - 2 * x * x - 2 * y * y, 0);
                Vector4 c4 = new Vector4(0, 0, 0, 1);
                Matrix4x4 mat = new Matrix4x4(c1, c2, c3, c4);
                return mat;
            }
            /// <summary>
            /// 获取设备坐标x:0--1 y:0--1
            /// </summary>
            public Vector2 devicePosition
            {
                get
                {
                    Vector3 vUp = quaternionToMatrix(qUp).MultiplyVector(fixAxis).normalized;
                    Vector3 vDown = quaternionToMatrix(qDown).MultiplyVector(fixAxis).normalized;
                    Vector3 vLeft = quaternionToMatrix(qLeft).MultiplyVector(fixAxis).normalized;
                    Vector3 vRight = quaternionToMatrix(qRight).MultiplyVector(fixAxis).normalized;
                    Vector3 vPointer = quaternionToMatrix(qPointer).MultiplyVector(fixAxis).normalized;
                    Vector3 projHor = Vector3.ProjectOnPlane(vPointer, Vector3.Cross(vLeft, vRight));
                    float dpX = inverseLerp(vLeft, vRight, projHor);
                    Vector3 projVer = Vector3.ProjectOnPlane(vPointer, Vector3.Cross(vUp, vDown));
                    float dpY = inverseLerp(vUp, vDown, projVer);
                    return new Vector2(dpX, dpY);
                }
            }
            public Vector3 getDevicePositionByVPointer(Vector3 vPointer)
            {
                Vector3 vUp = quaternionToMatrix(qUp).MultiplyVector(fixAxis).normalized;
                Vector3 vDown = quaternionToMatrix(qDown).MultiplyVector(fixAxis).normalized;
                Vector3 vLeft = quaternionToMatrix(qLeft).MultiplyVector(fixAxis).normalized;
                Vector3 vRight = quaternionToMatrix(qRight).MultiplyVector(fixAxis).normalized;
                Vector3 projHor = Vector3.ProjectOnPlane(vPointer, Vector3.Cross(vLeft, vRight));
                float dpX = inverseLerp(vLeft, vRight, projHor);
                Vector3 projVer = Vector3.ProjectOnPlane(vPointer, Vector3.Cross(vUp, vDown));
                float dpY = inverseLerp(vUp, vDown, projVer);
                return new Vector2(dpX, dpY);
            }
            private float inverseLerp(Vector3 a, Vector3 b, Vector3 value)
            {
                float totalDeg = Mathf.Abs(Vector3.Angle(a, b));
                float aDeg = Mathf.Abs(Vector3.Angle(a, value));
                float bDeg = Mathf.Abs(Vector3.Angle(value, b));

                if(aDeg >= totalDeg)
                {
                    return 1 + bDeg / totalDeg;
                }
                else if(bDeg >= totalDeg)
                {
                    return 0 - aDeg / totalDeg;
                }
                else
                {
                    return aDeg / totalDeg;
                }
            }
            
        }

        public MouseBoundary mouseBoundary;
        public class MouseSimulateManager
        {
            MouseBoundary mouseBoundary;
            public float mousePointerThreshold = 0.0005f;
            List<Vector3> lastMousePointers = new List<Vector3>();
            Vector3 lastAverMousePointer = Vector3.zero;
            Vector3 lastMousePointer = Vector3.zero;
            int averDataCount = 11;

            public void processData(RemoteXDataLibary.Data data)
            {
                Quaternion rot = new Quaternion(data.data[0], data.data[1], data.data[2], data.data[3]);
                Vector3 readyPointer = mouseBoundary.quaternionToVector(rot);
                lastMousePointers.Add(readyPointer);
                if (lastMousePointers.Count > averDataCount)
                {
                    lastMousePointers.RemoveAt(0);
                }
                Vector3 sumVec = Vector3.zero;
                foreach (Vector3 v in lastMousePointers)
                {
                    sumVec += v;
                }
                if (lastMousePointers.Count >= averDataCount)
                {
                    Vector3 currAverMousepointer = (sumVec / averDataCount).normalized;

                    if ((lastAverMousePointer - readyPointer).magnitude > mousePointerThreshold)
                    {
                        this.mouseBoundary.qPointer = rot;
                        Vector2 devicePosition = this.mouseBoundary.getDevicePositionByVPointer(currAverMousepointer);
                        devicePosition.x *= 66000;
                        devicePosition.y *= 66000;
                        MoveCursor moveCursor = new MoveCursor();
                        System.Drawing.Point point = new System.Drawing.Point();
                        point.X = (int)devicePosition.x;
                        point.Y = (int)devicePosition.y;
                        moveCursor.MoveTo(point);
                        lastAverMousePointer = sumVec / averDataCount;
                    }

                }
                else
                {
                    if (lastMousePointers.Count != 0)
                    {
                        lastAverMousePointer = (sumVec / lastMousePointers.Count).normalized;
                    }
                }
                lastMousePointer = readyPointer;
            }
        }

        public class TouchMouseManager
        {
            /// <summary>
            ///
            /// </summary>
            /// <param name="delta">这个是手机中传过来的移动速度</param>
            public void ProcessData(RemoteXDataLibary.Data data)
            {

                System.Drawing.Point point = new System.Drawing.Point((int)data.data[0], (int)data.data[1]);
                MoveCursor moveCursor = new MoveCursor(false);
                moveCursor.MoveTo(point);
            }
        }
        
        public class GyroscopeMouseManager
        {
            public Vector2 mouseSpeedFactor = new Vector2(10, 10);
            public void processData(RemoteXDataLibary.Data data)
            {
                Vector3 gyroData = new Vector3(data.data[0], data.data[1], data.data[2]);
                MoveCursor moveCursor = new MoveCursor(false);
                System.Drawing.Point point = new System.Drawing.Point();
                point.X = (int)(-gyroData.x * mouseSpeedFactor.x);
                point.Y = (int)(gyroData.y * mouseSpeedFactor.y);
                moveCursor.MoveTo(point);
            }
        }

        public class MouseButtonManager
        {
            public void processData(RemoteXDataLibary.Data data)
            {
                MoveCursor moveCursor = new MoveCursor(false);
                if (data.dataType == (int)RemoteXDataLibary.DataType.MouseLeftUp)
                {
                    moveCursor.LeftUp();
                }
                if(data.dataType == (int)RemoteXDataLibary.DataType.MouseLeftDown)
                {
                    moveCursor.LeftDown();
                }
                if(data.dataType == (int)RemoteXDataLibary.DataType.MouseRightUp)
                {
                    moveCursor.RightUp();
                }
                if (data.dataType == (int)RemoteXDataLibary.DataType.MouseRightDown)
                {
                    moveCursor.RightDown();
                    //moveCursor.scrollVerticle();
                    //moveCursor.MouseWheel(120);
                }
                if(data.dataType == (int)RemoteXDataLibary.DataType.MouseScrollVerticle)
                {
                    moveCursor.scrollVerticle(data.data[0]*2500);
                }
                if (data.dataType == (int)RemoteXDataLibary.DataType.MouseScrollHorizontal)
                {
                    moveCursor.scrollHorizontal(data.data[0] * 2500);
                }
            }
        }

        public class KeyboardController
        {
            public void processData(RemoteXDataLibary.Data data)
            {
                ushort keyCode = dataTypeToKeyCode(data.dataType);
                PressKeyboard pressKeyboard = new PressKeyboard();
                if (data.data[0] > 0)
                {
                    pressKeyboard.HoldKey(keyCode);
                }
                else
                {
                    pressKeyboard.ReleaseKey(keyCode);
                }
                
            }
            public ushort dataTypeToKeyCode(int dataType)
            {
                ushort ascii = (ushort)(dataType - 1000);
                return (ushort)ascii;
            }
        }
        public class GunController
        {
            public Vector2 mouseSpeedFactor = new Vector2(10, 10);
            public void processData(RemoteXDataLibary.Data data)
            {
                Vector3 gyroData = new Vector3(data.data[0], data.data[1], data.data[2]);
                MoveCursor moveCursor = new MoveCursor(false);
                System.Drawing.Point point = new System.Drawing.Point();
                point.X = (int)(-gyroData.x * mouseSpeedFactor.x);
                point.Y = (int)(gyroData.z * mouseSpeedFactor.y);
                moveCursor.MoveTo(point);
            }
        }
        MouseSimulateManager mouseSimulateManager;
        public GyroscopeMouseManager gyroscopeMouseManager;
        MouseButtonManager mouseButtonManager;
        KeyboardController keyboardController;
        TouchMouseManager touchMouseManager;

        public ControllerManager()
        {
            mouseSimulateManager = new MouseSimulateManager();
            gyroscopeMouseManager = new GyroscopeMouseManager();
            mouseButtonManager = new MouseButtonManager();
            keyboardController = new KeyboardController();
            touchMouseManager = new TouchMouseManager();
        }

        public void addData(RemoteXDataLibary.Data data)
        {
            if(data.dataType == (int)(RemoteXDataLibary.DataType.SetMouseBoundary))
            {
                Quaternion leftUp = new Quaternion(data.data[0], data.data[1], data.data[2], data.data[3]);
                Quaternion rightUp = new Quaternion(data.data[4], data.data[5], data.data[6], data.data[7]);
                Quaternion rightDown = new Quaternion(data.data[8], data.data[9], data.data[10], data.data[11]);
                Quaternion leftDown = new Quaternion(data.data[12], data.data[13], data.data[14], data.data[15]);
                this.mouseBoundary = new MouseBoundary(leftUp, rightUp, rightDown, leftDown);
            }
            if(data.dataType == (int)RemoteXDataLibary.DataType.MouseRotationVector)
            {
                mouseSimulateManager.processData(data);
            }
            if(data.dataType == (int)RemoteXDataLibary.DataType.SensorGyroscope)
            {
                gyroscopeMouseManager.processData(data);
            }
            if(data.dataType<600 && data.dataType >= 300)
            {
                mouseButtonManager.processData(data);
            }
            if(data.dataType>=1000 && data.dataType < 2000)
            {
                keyboardController.processData(data);
            }
            if(data.dataType == (int)RemoteXDataLibary.DataType.TouchMouseSpeed)
            {
                touchMouseManager.ProcessData(data);
            }
            
            //if(data.dataType==(int)BluetoothUtility.DataType.)
        }
        public Matrix4x4 quaternionToMatrix(Quaternion rot)
        {
            float x = rot.x;
            float y = rot.y;
            float z = rot.z;
            float w = rot.w;
            Vector4 c1 = new Vector4(1 - 2 * y * y - 2 * z * z, 2 * x * y + 2 * w * z, 2 * x * z - 2 * w * y, 0);
            Vector4 c2 = new Vector4(2 * x * y - 2 * w * z, 1 - 2 * x * x - 2 * z * z, 2 * y * z + 2 * w * x, 0);
            Vector4 c3 = new Vector4(2 * x * z + 2 * w * y, 2 * y * z - 2 * w * x, 1 - 2 * x * x - 2 * y * y, 0);
            Vector4 c4 = new Vector4(0, 0, 0, 1);
            Matrix4x4 mat = new Matrix4x4(c1, c2, c3, c4);
            return mat;
        }

    }
}
