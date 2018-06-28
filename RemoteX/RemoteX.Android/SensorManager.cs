using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RemoteX.Sensor;
using RemoteXDataLibary.Mathf;

[assembly: Xamarin.Forms.Dependency(typeof(RemoteX.Droid.SensorManager))]
namespace RemoteX.Droid
{
    /// <summary>
    /// 这个SensorManager不是Android内的SensorManager，而是实现了ISensorManager接口的一个Manager
    /// 以后统一一下表述
    /// Android的东西（重名的）前面都加上Droid，比如DroidSensor
    /// </summary>
    class SensorManager : ISensorManager
    {
        /// <summary>
        /// 安卓的SensorManager
        /// </summary>
        private Android.Hardware.SensorManager _DroidSensorManager;

        /// <summary>
        /// 被获取过的Sensor列表
        /// 若同一个Sensor在多处被获取多次，则确保获取的Sensor都是同一个实例
        /// </summary>
        private List<ISensor> _Sensors;
        public SensorManager()
        {
            _DroidSensorManager = (Android.Hardware.SensorManager)Application.Context.GetSystemService(Context.SensorService);
            _Sensors = new List<ISensor>();
        }

        public ISensor this[SensorType sensorType]
        {
            get
            {
                for (int i = 0; i < _Sensors.Count; i++)
                {
                    if (_Sensors[i].SensorType == sensorType)
                    {
                        return _Sensors[i];
                    }
                }
                if (sensorType == SensorType.Velocity)
                {
                    VelocitySensor veloctiySensor = new VelocitySensor(this);
                    _Sensors.Add(veloctiySensor);
                    return veloctiySensor;
                }
                else
                {
                    Android.Hardware.SensorType? droidTypeNullable = sensorTypeToDroidSensorType(sensorType);

                    if (droidTypeNullable == null)
                    {
                        return null;
                    }
                    Android.Hardware.SensorType droidSensorType = (Android.Hardware.SensorType)droidTypeNullable;
                    Android.Hardware.Sensor droidSensor = _DroidSensorManager.GetDefaultSensor(droidSensorType);
                    //检查一下这个Sensor是不是已经被创建过了
                    
                    SingleSensor sensor = new SingleSensor(this, droidSensor);
                    _Sensors.Add(sensor);
                    return sensor;
                }

                
            }
        }

        /// <summary>
        /// 将RemoteX项目的SensorType转换成AndroidAPI里的SensorType
        /// </summary>
        /// <param name="sensorType"></param>
        /// <returns>如果为null则转化不存在</returns>
        private Android.Hardware.SensorType? sensorTypeToDroidSensorType(SensorType sensorType)
        {
            switch (sensorType)
            {
                case SensorType.Gyroscope:
                    return Android.Hardware.SensorType.Gyroscope;
                case SensorType.Accelerometer:
                    return Android.Hardware.SensorType.Accelerometer;
                case SensorType.MagneticField:
                    return Android.Hardware.SensorType.MagneticField;
                case SensorType.RotationVector:
                    return Android.Hardware.SensorType.GameRotationVector;
            }
            return null;
        }

        /// <summary>
        /// 获取设备方向
        /// </summary>
        /// <param name="accelerometerReading"></param>
        /// <param name="magnetometerReading"></param>
        /// <returns></returns>
        public float[] GetOrientation(float[] accelerometerReading, float[] magnetometerReading)
        {
            float[] rotationMatrix = new float[9];
            float[] orientationAngles = new float[3];
            Android.Hardware.SensorManager.GetRotationMatrix(rotationMatrix, null, accelerometerReading, magnetometerReading);
            
            
            Android.Hardware.SensorManager.GetOrientation(rotationMatrix, orientationAngles);
            return orientationAngles;
        }
        /// <summary>
        /// RemoteX种ISensor的具体实现
        /// </summary>
        private class SingleSensor : Java.Lang.Object, ISensor, Android.Hardware.ISensorEventListener
        {
            public SensorType SensorType
            {
                get
                {
                    return (SensorType)droidSensorTypeToSensorType(DroidSensor.Type);
                }
            }
            public Android.Hardware.SensorType DroidSensorType
            {
                get
                {
                    return DroidSensor.Type;
                }
            }
            public Android.Hardware.Sensor DroidSensor { get; set; }
            public event SensorDataHandler OnSensorDataUpdated;
            private SensorManager _SensorManager;
            private bool _Registered;
            public double UpdateTimestep { get; set; }
            /// <summary>
            /// 最后更新数据时的时间
            /// </summary>
            private DateTime _LatestUpdatedDataDateTime;

            public SingleSensor(SensorManager sensorManager, Android.Hardware.Sensor droidSensor)
            {
                this.DroidSensor = droidSensor;
                this._SensorManager = sensorManager;
                _Registered = false;
                _LatestUpdatedDataDateTime = DateTime.Now;
                UpdateTimestep = 15;
            }

            private SensorType? droidSensorTypeToSensorType(Android.Hardware.SensorType droidSensorType)
            {
                switch (droidSensorType)
                {
                    case Android.Hardware.SensorType.Gyroscope:
                        return SensorType.Gyroscope;
                    case Android.Hardware.SensorType.Accelerometer:
                        return SensorType.Accelerometer;
                    case Android.Hardware.SensorType.MagneticField:
                        return SensorType.MagneticField;
                    case Android.Hardware.SensorType.GameRotationVector:
                        return SensorType.RotationVector;
                }
                return null;
            }

            public void OnAccuracyChanged(Android.Hardware.Sensor sensor, [GeneratedEnum] Android.Hardware.SensorStatus accuracy)
            {

            }
            public void OnSensorChanged(Android.Hardware.SensorEvent e)
            {
                if (e.Sensor.Type != DroidSensorType)
                {
                    return;
                }
                if ((DateTime.Now - _LatestUpdatedDataDateTime).TotalMilliseconds >= UpdateTimestep)
                {
                    _LatestUpdatedDataDateTime = DateTime.Now;

                    if(DroidSensorType == Android.Hardware.SensorType.GameRotationVector)
                    {
                        float[] quaternion = new float[4];
                        Android.Hardware.SensorManager.GetQuaternionFromVector(quaternion, e.Values.ToArray<float>());
                        OnSensorDataUpdated?.Invoke(this, quaternion);
                    }
                    else
                    {
                        OnSensorDataUpdated?.Invoke(this, e.Values.ToArray<float>());
                    }
                    
                }

            }
            public void Activate()
            {
                if (_Registered)
                {
                    return;
                }
                _SensorManager._DroidSensorManager.RegisterListener(this, DroidSensor, Android.Hardware.SensorDelay.Fastest);
                _Registered = true;
            }
            public void Deactivate()
            {
                if (_Registered)
                {
                    _SensorManager._DroidSensorManager.UnregisterListener(this);
                }
            }
        }

        private class VelocitySensor : Java.Lang.Object, IVelocitySensor, Android.Hardware.ISensorEventListener
        {
            public SensorType SensorType
            {
                get { return SensorType.Velocity; }
            }
            public double UpdateTimestep { get; set; }
            public event SensorDataHandler OnSensorDataUpdated;
            bool Activated { get; set; }
            Android.Hardware.Sensor linearAcceleration;
            SensorManager sensorManager;
            DateTime _LatestUpdatedDataDateTime;
            public VelocitySensor(SensorManager sensorManager)
            {
                this.sensorManager = sensorManager;
                Activated = false;
                _LatestUpdatedDataDateTime = DateTime.Now;
                linearAcceleration = sensorManager._DroidSensorManager.GetDefaultSensor(Android.Hardware.SensorType.LinearAcceleration);
                UpdateTimestep = 15;
                previousProcessDataTime = DateTime.Now;
            }
            public void Activate()
            {
                if(Activated)
                {
                    return;
                }
                sensorManager._DroidSensorManager.RegisterListener(this, linearAcceleration, Android.Hardware.SensorDelay.Fastest);
                Activated = true;
            }

            public void Deactivate()
            {
                if (Activated)
                {
                    return;
                }
            }

            public void OnAccuracyChanged(Android.Hardware.Sensor sensor, [GeneratedEnum] Android.Hardware.SensorStatus accuracy)
            {

            }
            public void OnSensorChanged(Android.Hardware.SensorEvent e)
            {
                if (e.Sensor.Type != Android.Hardware.SensorType.LinearAcceleration)
                {
                    return;
                }
                processData(e.Values.ToArray<float>());

                if ((DateTime.Now - _LatestUpdatedDataDateTime).TotalMilliseconds >= UpdateTimestep)
                {
                    _LatestUpdatedDataDateTime = DateTime.Now;
                    OnSensorDataUpdated?.Invoke(this, velocity.Array);
                }

            }

            DateTime previousProcessDataTime;
            Vector3 velocity = new Vector3(0, 0, 0);
            private void processData(float[] data)
            {
                DateTime now = DateTime.Now;
                TimeSpan deltaTime = now - previousProcessDataTime;
                Vector3 accelerate = new Vector3(data);
                velocity += (float)deltaTime.TotalSeconds * accelerate;
                previousProcessDataTime = now;
            }

            public void Reset()
            {
                previousProcessDataTime = DateTime.Now;
                velocity = new Vector3(0, 0, 0);
            }
        }
    }
}