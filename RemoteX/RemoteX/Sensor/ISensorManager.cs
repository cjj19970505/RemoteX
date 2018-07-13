using System;
using System.Collections.Generic;
using System.Text;
using RemoteX.Data.Mathf;

namespace RemoteX.Sensor
{
    public enum SensorType { Gyroscope, Accelerometer, MagneticField, RotationVector, Velocity }
    
    public interface ISensorManager
    {
        
        /// <summary>
        /// 根据SensorType获取Sensor实例
        /// </summary>
        /// <param name="sensorType"></param>
        /// <returns></returns>
        ISensor this[SensorType sensorType] { get; }

        float[] GetOrientation(float[] accelerometerReading, float[] magnetometerReading);
    }
}
