﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sensor
{
    public enum SensorType { Gyroscope}
    
    public interface ISensorManager
    {
        
        /// <summary>
        /// 根据SensorType获取Sensor实例
        /// </summary>
        /// <param name="sensorType"></param>
        /// <returns></returns>
        ISensor this[SensorType sensorType] { get; }
    }
}