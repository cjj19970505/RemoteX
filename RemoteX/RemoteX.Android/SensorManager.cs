﻿using System;
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
        private List<Sensor> _Sensors;
        public SensorManager()
        {
            _DroidSensorManager = (Android.Hardware.SensorManager)Application.Context.GetSystemService(Context.SensorService);
            _Sensors = new List<Sensor>();
        }

        public ISensor this[SensorType sensorType]
        {
            get
            {
                Android.Hardware.SensorType? droidTypeNullable = sensorTypeToDroidSensorType(sensorType);

                if (droidTypeNullable == null)
                {
                    return null;
                }
                Android.Hardware.SensorType droidSensorType = (Android.Hardware.SensorType)droidTypeNullable;
                Android.Hardware.Sensor droidSensor = _DroidSensorManager.GetDefaultSensor(droidSensorType);
                //检查一下这个Sensor是不是已经被创建过了
                for (int i = 0; i < _Sensors.Count; i++)
                {
                    if (_Sensors[i].SensorType == sensorType)
                    {
                        return _Sensors[i];
                    }
                }
                Sensor sensor = new Sensor(this, droidSensor);
                _Sensors.Add(sensor);
                return sensor;
            }
        }

        /// <summary>
        /// 将RemoteX项目的SensorType转换成AndroidAPI里的SensorType
        /// </summary>
        /// <param name="sensorType"></param>
        /// <returns>如果为null则转化不存在</returns>
        private Android.Hardware.SensorType? sensorTypeToDroidSensorType(SensorType sensorType)
        {
            switch(sensorType)
            {
                case SensorType.Gyroscope:
                    return Android.Hardware.SensorType.Gyroscope;
            }
            return null;
        }

        /// <summary>
        /// 将AndroidAPI项目的SensorType转换成RemoteX里的SensorType
        /// </summary>
        /// <param name="droidSensorType"></param>
        /// <returns>如果为null则转化不存在</returns>
        private SensorType? droidSensorTypeToSensorType(Android.Hardware.SensorType droidSensorType)
        {
            switch (droidSensorType)
            {
                case Android.Hardware.SensorType.Gyroscope:
                    return SensorType.Gyroscope;
            }
            return null;
        }


        /// <summary>
        /// RemoteX种ISensor的具体实现
        /// </summary>
        private class Sensor:Java.Lang.Object, ISensor, Android.Hardware.ISensorEventListener
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
            public Sensor(SensorManager sensorManager ,Android.Hardware.Sensor droidSensor)
            {
                this.DroidSensor = droidSensor;
                this._SensorManager = sensorManager;
                _Registered = false;
            }

            private SensorType? droidSensorTypeToSensorType(Android.Hardware.SensorType droidSensorType)
            {
                switch (droidSensorType)
                {
                    case Android.Hardware.SensorType.Gyroscope:
                        return SensorType.Gyroscope;
                }
                return null;
            }

            public void OnAccuracyChanged(Android.Hardware.Sensor sensor, [GeneratedEnum] Android.Hardware.SensorStatus accuracy)
            {

            }
            public void OnSensorChanged(Android.Hardware.SensorEvent e)
            {
                if(e.Sensor.Type != DroidSensorType)
                {
                    return;
                }
                OnSensorDataUpdated?.Invoke(this, e.Values.ToArray<float>());
            }

            public void Activate()
            {
                if(_Registered)
                {
                    return;
                }
                _SensorManager._DroidSensorManager.RegisterListener(this, DroidSensor, Android.Hardware.SensorDelay.Fastest);
                _Registered = true;
            }
            public void Deactivate()
            {
                if(_Registered)
                {
                    _SensorManager._DroidSensorManager.UnregisterListener(this);
                }
            }
        }

    }
}