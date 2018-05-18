using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Xamarin.Forms;
using RemoteX.Sensor;
using RemoteXDataLibary;

namespace RemoteX.Controller
{
	public class SensorTestPage : ControllerPage
	{
        IConnectionManager _ConnectionManager;
		public SensorTestPage ():base()
		{
            ISensorManager sensorManager = DependencyService.Get<ISensorManager>();
            _ConnectionManager = DependencyService.Get<IConnectionManager>();
            ISensor gyroSensor = sensorManager[SensorType.Gyroscope];
            gyroSensor.Activate();
            gyroSensor.OnSensorDataUpdated += _SensorDataHandler;
            ControllerContentView = new StackLayout {
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
                IConnection connection = _ConnectionManager.ControllerConnection;
                if (connection != null)
                {
                    connection.SendAsync(Data.encodeSensorData(new Data((int)DataType.SensorGyroscope, data)));
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