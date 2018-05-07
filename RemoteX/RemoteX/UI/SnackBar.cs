using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace RemoteX.UI
{
    public enum BarType { Warning, Error, Info };
    /// <summary>
    /// 只用在Absolute Layout
    /// </summary>
	public class SnackBar : Frame
	{
        /// <summary>
        /// 文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///  Bar的高度(Absolute)
        /// </summary>
        public double BarHeight { get; set; }
        private BarType _BarType;
        private Image _TypeIcon;
        public BarType BarType
        {
            set
            {
                _BarType = value;
                if (_BarType == BarType.Warning)
                {
                    BackgroundColor = new Color(1,218/255.0,218/255.0);
                    _TypeIcon.Source = ImageSource.FromResource("RemoteX.UI.Icon.RedWarning.png");
                }
                else if(_BarType == BarType.Info)
                {
                    BackgroundColor = new Color(1, 253 / 255.0, 199 / 255.0);
                    _TypeIcon.Source = ImageSource.FromResource("RemoteX.UI.Icon.InfoIcon.png");
                }
                
            }
        }
		public SnackBar ()
		{
            _TypeIcon = new Image() { Margin = new Thickness(5, 0, 5, 0), VerticalOptions = LayoutOptions.Center };

            BarType = BarType.Info;
            BarHeight = 30;
            Text = "Really?? No Message??sdfasdfasdfasdf";
            HasShadow = true;
            Padding = new Thickness(0, 5, 0, 5);
            AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);
            AbsoluteLayout.SetLayoutBounds(this, new Rectangle(0.5, 0.95, 0.95, BarHeight));
            Button b;
            TapGestureRecognizer btnTap = new TapGestureRecognizer();
            btnTap.Tapped += onBtnTapped;
            Label label = new Label() { Text = "Cancel", FontSize = 12, HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(5, 0, 5, 0) };
            label.GestureRecognizers.Add(btnTap);
            Content = new StackLayout {
                Orientation = StackOrientation.Horizontal,
                //BackgroundColor = Color.Purple,
                Padding = 0,
                Margin = 0,
                VerticalOptions = LayoutOptions.Center,
                Children = {
                    _TypeIcon,
                    new Label(){Text = Text, FontSize = 12, VerticalOptions = LayoutOptions.Center},
                    label
                }
			};
		}
        private void onBtnTapped(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("FUCK YOU");
        }
    }
}