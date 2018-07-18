using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using RemoteX.Bluetooth;
using System.Diagnostics;
using RemoteX.Data;
using RemoteX.Sensor;
using ZXing.Mobile;
using RemoteX.Core;

namespace RemoteX.Controller
{
    public class WordsTalkerPage : ControllerPage
    {
        private Editor sendEditor;
        public WordsTalkerPage() : base()
        {
            sendEditor = new Editor();
            Button sendButton = new Button()
            {
                Text = "Send",
            };
            sendButton.Clicked += onSendBtnClicked;

            Button scanQRCode = new Button();
            sendButton.Clicked += async (sender, e) =>
            {
                try
                {
	                //MobileBarcodeScanner.Initialize (Application);
                    var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                    var result = await scanner.Scan();

                    if (result != null)
                        Console.WriteLine("Scanned Barcode: " + result.Text);
                }
                catch (Exception exce)
                {

                }
            };

            StackLayout senderLayout = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Children =
                {
                    sendEditor,
                    sendButton
                }
            };
            Content = new StackLayout
            {
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
            if (connection == null)
            {
                Debug.WriteLine("No Connection");
                return;
            }
            RemoteXControlMessage data = new RemoteXControlMessage((int)DataType.MouseRightDown, new float[] { 1.0f });
            Debug.WriteLine("SENDINGDATA::" + data);
            await connection.SendAsync(data.Bytes);
            Debug.WriteLine("SUCCFULLY SENDED::" + data);
            data = new RemoteXControlMessage((int)DataType.MouseRightUp, new float[] { 1.0f });
            Debug.WriteLine("SENDINGDATA::" + data);
            await connection.SendAsync(data.Bytes);
            Debug.WriteLine("SUCCFULLY SENDED::" + data);



        }
    }
}