using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace RemoteX.Bluetooth
{
    public class BluetoothDeviceCell : ViewCell
    {

        public BluetoothDeviceCell()
        {
            var deviceNameLabel = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = "Unknown"
            };
            deviceNameLabel.SetBinding(Label.TextProperty, "Name");
            
            var deviceMacLabel = new Label
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                FontSize = 10,
                Text = "Unknown"
            };
            deviceMacLabel.SetBinding(Label.TextProperty, "Address");

            var viewLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Children = { deviceNameLabel, deviceMacLabel }
            };
            this.View = viewLayout;
        }
    }
}