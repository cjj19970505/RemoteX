using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using RemoteX.Bluetooth;
using System.Diagnostics;
using RemoteXDataLibary;
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
            IConnectionManager connectionManager = DependencyService.Get<IConnectionManager>();
            IConnection connection = connectionManager.ControllerConnection;
            if(connection==null)
            {
                Debug.WriteLine("No Connection");
                return;
            }
            Data data = new Data((int)DataType.MouseRightDown, new float[] { 1.0f });
            Debug.WriteLine("SENDINGDATA::" + data);
            await connection.SendAsync(Data.encodeSensorData(data));
            Debug.WriteLine("SUCCFULLY SENDED::" + data);
            data = new Data((int)DataType.MouseRightUp, new float[] { 1.0f });
            Debug.WriteLine("SENDINGDATA::" + data);
            await connection.SendAsync(Data.encodeSensorData(data));
            Debug.WriteLine("SUCCFULLY SENDED::"+data);

        }
	}
}