using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Xamarin.Forms;
using RemoteX.Sensor;

namespace RemoteX.Controller
{
	public class SensorTestPage : ControllerPage
	{
		public SensorTestPage ():base()
		{
            ISensorManager sensorManager = DependencyService.Get<ISensorManager>();
            ISensor gyroSensor = sensorManager[SensorType.Gyroscope];
            gyroSensor.Activate();
            gyroSensor.OnSensorDataUpdated += _SensorDataHandler;
            Content = new StackLayout {
				Children = {
					new Label { Text = "Sensor Testttt" }
				}
			};
		}
        void _SensorDataHandler(ISensor sensor, float[] data)
        {
            if(sensor.SensorType == SensorType.Gyroscope)
            {
                string s = "";
                for(int i = 0;i<data.Length;i++)
                {
                    s += data[i] + ", ";
                }
            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DependencyService.Get<ISensorManager>()[SensorType.Gyroscope].OnSensorDataUpdated -= _SensorDataHandler;
        }

    }
}