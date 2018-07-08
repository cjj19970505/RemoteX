using RemoteX.Sensor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RemoteXDataLibary;

using Xamarin.Forms;

namespace RemoteX.Controller
{
	public class NextIdeaController : ControllerPage
    {
        IConnectionManager _ConnectionManager;
        ISensor rotationVectorSensor;
        public NextIdeaController ()
		{
            ISensorManager sensorManager = DependencyService.Get<ISensorManager>();
            _ConnectionManager = DependencyService.Get<IConnectionManager>();
            rotationVectorSensor = sensorManager[SensorType.RotationVector];
            rotationVectorSensor.Activate();
            rotationVectorSensor.OnSensorDataUpdated += _SensorDataHandler;


            Content = new StackLayout {
				Children = {
					new Label { Text = "NextIdea DEBUG Controller" }
				}
			};
		}

        private void _SensorDataHandler(ISensor sensor, float[] data)
        {
            IConnection connection = _ConnectionManager.ControllerConnection;
            if (sensor.SensorType == SensorType.RotationVector)
            {
                if (connection != null)
                {
                    connection.SendAsync(new RemoteXControlMessage((int)DataType.SensorRotationVector, data).Bytes);
                }
            }
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DependencyService.Get<ISensorManager>()[SensorType.RotationVector].OnSensorDataUpdated -= _SensorDataHandler;
        }
    }
}