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
            Debug.WriteLine("MotherFucker"+ gyroSensor);
            Content = new StackLayout {
				Children = {
					new Label { Text = "Welcome to Xamarin.Forms!" }
				}
			};
		}

        void onSensorChanged()
        {

        }
	}
}