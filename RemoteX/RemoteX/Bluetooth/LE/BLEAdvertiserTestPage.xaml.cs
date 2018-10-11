using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RemoteX.Bluetooth.LE.Gatt;
using RemoteX.Input;
using RemoteX.Data.Mathf;
using RemoteX.Core;
using RemoteX.Data;

namespace RemoteX.Bluetooth.LE
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BLEAdvertiserTestPage : ContentPage
    {
        BatteryServiceWrapper BatteryServiceWrapper;
        TestServiceWrapper TestServiceWrapper;
        public BLEAdvertiserTestPage()
        {

            InitializeComponent();
        }

        private void StartAdvertisingButton_Clicked(object sender, EventArgs e)
        {
            var bluetoothManager = DependencyService.Get<IManagerManager>().BluetoothManager;
            bluetoothManager.GattSever.AddService(new DeviceInfomationServiceBuilder(bluetoothManager).Build());
            BatteryServiceWrapper = new BatteryServiceWrapper(bluetoothManager);
            bluetoothManager.GattSever.AddService(BatteryServiceWrapper.GattServerService);
            TestServiceWrapper = new TestServiceWrapper(bluetoothManager);
            TestServiceWrapper.KeepNotifyingCharacteristicWrapper.NotifyLength = 8;
            bluetoothManager.GattSever.AddService(TestServiceWrapper.GattServerService);
            bluetoothManager.GattSever.StartAdvertising();

        }

        private void SendNotifyButton_Clicked(object sender, EventArgs e)
        {
            if (TestServiceWrapper.KeepNotifyingCharacteristicWrapper.Notifying)
            {
                TestServiceWrapper.KeepNotifyingCharacteristicWrapper.StopNotify();
            }
            else
            {
                TestServiceWrapper.KeepNotifyingCharacteristicWrapper.KeepNotifyAsync();
            }
        }

        private void NotifyTestSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            BatteryServiceWrapper.BatteryLevelCharacteristicWrapper.BatteryLevel = (int)e.NewValue;
            BatteryServiceWrapper.BatteryLevelCharacteristicWrapper.NotifyAll();
            IntervalLabel.Text = e.NewValue.ToString();
            TestServiceWrapper.KeepNotifyingCharacteristicWrapper.Interval = TimeSpan.FromMilliseconds(e.NewValue);

        }

        private ITouch _FirstTouch;
        private Vector2 previousTouchPos;
        private float _SpeedFactor;
        private float _RotateRadiusFactor;
        private void onTouchAction(ITouch touch, TouchMotionAction action)
        {
            
            if (_FirstTouch == null && action == TouchMotionAction.Down)
            {
                _FirstTouch = touch;
                previousTouchPos = touch.Position;
            }
            if (touch != _FirstTouch)
            {
                return;
            }
            if (action == TouchMotionAction.Move)
            {
                Vector2 speed = (touch.Position - previousTouchPos) * (float)NotifyTestSlider.Value;
                previousTouchPos = touch.Position;
                TestServiceWrapper.TestCharacteristicWrapper.SetMouseSpeed(speed);
                System.Diagnostics.Debug.WriteLine("XJ" + speed);
                /*
                IConnectionManager controllerManager = DependencyService.Get<IConnectionManager>();
                IConnection connection = controllerManager.ControllerConnection;
                if (connection != null && connection.ConnectionEstablishState == ConnectionEstablishState.Succeeded)
                {
                    RemoteXControlMessage data = new RemoteXControlMessage((int)DataType.TouchMouseSpeed, new float[] { speed.x, speed.y });
                    await connection.SendAsync(data.Bytes);
                }
                */
            }
            else if (action == TouchMotionAction.Up)
            {
                if (touch == _FirstTouch)
                {
                    _FirstTouch = null;
                    TestServiceWrapper.TestCharacteristicWrapper.SetMouseSpeed(Vector2.Zero);
                }
            }
        }

        private void NotifyLengthSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            TestServiceWrapper.KeepNotifyingCharacteristicWrapper.NotifyLength = (int)e.NewValue;
            LengthLabel.Text = TestServiceWrapper.KeepNotifyingCharacteristicWrapper.NotifyLength.ToString();
        }
    }
}