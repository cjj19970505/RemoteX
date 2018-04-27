using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using RemoteX.Bluetooth;
using System.Diagnostics;
using RemoteX.Data;
using RemoteX.Sensor;

namespace RemoteX.Controller
{
	public class WordsTalkerPage : ControllerPage
	{
        private Editor sendEditor;
		public WordsTalkerPage ():base()
		{
            sendEditor = new Editor();
            Button sendButton = new Button()
            {
                Text = "Send",
            };
            sendButton.Clicked += onSendBtnClicked;

            StackLayout senderLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    sendEditor,
                    sendButton
                }
            };
            Content = new StackLayout {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.End,
                Children = {
                    senderLayout
                }
			};
            
		}

        private async void onSendBtnClicked(object sender, EventArgs e)
        {
            string text = sendEditor.Text;
            IBluetoothManager bluetoothManager = DependencyService.Get<IBluetoothManager>();
            IConnection connection = bluetoothManager.DefaultConnection;
            if(connection==null)
            {
                Debug.WriteLine("No Connection");
                return;
            }
            Data.Data data = new Data.Data((int)DataType.MouseRightDown, new float[] { 1.0f });
            Debug.WriteLine("SENDINGDATA::" + data);
            await connection.sendAsync(Data.Data.encodeSensorData(data));
            Debug.WriteLine("SUCCFULLY SENDED::" + data);
            data = new Data.Data((int)DataType.MouseRightUp, new float[] { 1.0f });
            Debug.WriteLine("SENDINGDATA::" + data);
            await connection.sendAsync(Data.Data.encodeSensorData(data));
            Debug.WriteLine("SUCCFULLY SENDED::"+data);

        }
	}
}