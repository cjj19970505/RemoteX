using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Xamarin.Forms;
using RemoteX.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using RemoteX.Mathf;
using RemoteX.Bluetooth;
using RemoteX.Data;

namespace RemoteX.Controller
{
	public class TouchMousePage : ControllerPage
	{
        private ITouch _FirstTouch;
        private Vector2 previousTouchPos;
        private float _SpeedFactor;
        private float _RotateRadiusFactor;
		public TouchMousePage ():base()
		{
            IInputManager inputManager = DependencyService.Get<IInputManager>();
            inputManager.OnTouchAction += onTouchAction;
            Title = "Touch Mouse Page";
            previousTouchPos = Vector2.Zero;
            Slider speedFactorSlider = new Slider
            {
                Minimum = 0,
                Maximum = 10,
                Value = 1
            };
            Slider radiusFactorSlider = new Slider
            {
                Minimum = 0,
                Maximum = 5,
                Value = 0.5
            };
            speedFactorSlider.ValueChanged += onSpeedValueChanged;
            radiusFactorSlider.ValueChanged += onRadiusValueChanged;
            Content = new StackLayout {
                Orientation = StackOrientation.Vertical,
				Children = {
                    speedFactorSlider,
                    radiusFactorSlider
                }
			};
            initialTime = DateTime.Now;
            previousTime = initialTime;
            Device.StartTimer(TimeSpan.FromSeconds(1f/60), _Update);
            stopUpdate = false;

        }
        private DateTime initialTime;
        private DateTime previousTime;
        private bool stopUpdate;
        private bool _Update()
        {
            if(stopUpdate)
            {
                return false;
            }
            DateTime currTime = DateTime.Now;
            float deltaTime = (float)(currTime - previousTime).TotalSeconds;
            float t = (float)(currTime - initialTime).TotalSeconds;
            float x = (float)Math.Sin(_SpeedFactor*t);
            float y = (float)Math.Cos(_SpeedFactor*t);
            Vector2 speed = new Vector2(x, y) * _RotateRadiusFactor * 10;
            IConnectionManager connectionManager = DependencyService.Get<IConnectionManager>();
            IConnection connection = connectionManager.ControllerConnection;
            if (connection != null)
            {
                Data.Data data = new Data.Data((int)DataType.TouchMouseSpeed, new float[] { speed.x, speed.y });
                connection.sendAsync(Data.Data.encodeSensorData(data));
            }
            previousTime = currTime;
            return true;
        }
        private void onSpeedValueChanged(object sender, ValueChangedEventArgs e)
        {
            _SpeedFactor = (float)e.NewValue;
        }

        private void onRadiusValueChanged(object sender, ValueChangedEventArgs e)
        {
            _RotateRadiusFactor = (float)e.NewValue;
        }


        private async void onTouchAction(ITouch touch, TouchMotionAction action)
        {
            if(_FirstTouch == null && action == TouchMotionAction.Down)
            {
                _FirstTouch = touch;
                previousTouchPos = touch.Position;
            }
            if(touch != _FirstTouch)
            {
                return;
            }
            if(action == TouchMotionAction.Move)
            {
                Vector2 speed = (touch.Position - previousTouchPos)*_SpeedFactor;
                previousTouchPos = touch.Position;
                IConnectionManager controllerManager = DependencyService.Get<IConnectionManager>();
                IConnection connection = controllerManager.ControllerConnection;
                if(connection != null)
                {
                    Data.Data data = new Data.Data((int)DataType.TouchMouseSpeed, new float[] { speed.x, speed.y });
                    await connection.sendAsync(Data.Data.encodeSensorData(data));
                }
            }
            else if(action == TouchMotionAction.Up)
            {
                if(touch == _FirstTouch)
                {
                    _FirstTouch = null;
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            stopUpdate = true;
            IInputManager inputManager = DependencyService.Get<IInputManager>();
            inputManager.OnTouchAction -= onTouchAction;
        }
    }
}