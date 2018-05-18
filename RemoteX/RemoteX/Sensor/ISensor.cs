using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.Sensor
{
    public delegate void SensorDataHandler(ISensor sensor, float[] data);

    /// <summary>
    /// 若同一种类型的Sensor在不同的地方通过不同的手段被获取多次，要确保获取的Sensor一直都是同一个实例
    /// </summary>
    public interface ISensor
    {
        SensorType SensorType { get; }

        double UpdateTimestep { get; set; }

        /// <summary>
        /// 传感器数据更新时触发
        /// </summary>
        event SensorDataHandler OnSensorDataUpdated;
        void Activate();
        void Deactivate();
        
    }
}
