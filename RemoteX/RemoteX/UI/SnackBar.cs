using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using RemoteXDataLibary.Mathf;

namespace RemoteX.UI
{
    public enum BarType { Warning, Error, Succeed };
    /// <summary>
    /// 只用在Absolute Layout
    /// </summary>
	public class SnackBar : Frame
	{
        private BarType _BarType;
        private Color _StartTypeColor;
        private Color _CurrentTypeColor;
        Animation _TypeTransitionAnimation;

        private Image _TypeIcon;

        public BarType BarType
        {
            get
            {
                return _BarType;
            }
            set
            {
                BarType previousBarType = _BarType;
                _BarType = value;
                if (_BarType == BarType.Warning)
                {
                    BackgroundColor = _BarTypeToColor(value);
                    //_TypeIcon.Source = ImageSource.FromResource("RemoteX.UI.Icon.RedWarning.png");
                }
                else if (_BarType == BarType.Succeed)
                {
                    BackgroundColor = _BarTypeToColor(value);
                    //_TypeIcon.Source = ImageSource.FromResource("RemoteX.UI.Icon.InfoIcon.png");
                }
                if(_TypeTransitionAnimation == null)
                {
                    _TypeTransitionAnimation = new Animation(_TypeChangeAnimate);
                }
                if (this.AnimationIsRunning("TypeTransistion"))
                {
                    this.AbortAnimation("TypeTransistion");
                    _StartTypeColor = BackgroundColor;
                }
                else
                {
                    _StartTypeColor = _BarTypeToColor(previousBarType);
                }
                _TypeTransitionAnimation.Commit(this, "TypeTransistion", 16, 250, Easing.Linear,
                     (double v, bool canceled) => {
                         if(!canceled)
                         {
                             BackgroundColor = _BarTypeToColor(_BarType);
                         }
                         else
                         {
                             _StartTypeColor = BackgroundColor;
                         }
                         
                     }
                     ,
                     ()=> { return false; });

            }
        }

        /// <summary>
        /// 切换时的过渡时间
        /// </summary>
        private TimeSpan _ColorTransitionTimespan;

        /// <summary>
        /// 文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///  Bar的高度(Absolute)
        /// </summary>
        public double BarHeight { get; set; }

        private void _TypeChangeAnimate(double v)
        {
            Color startColor = _StartTypeColor;
            Color endColor = _BarTypeToColor(_BarType);
            double newR = lerp(startColor.R, endColor.R, v);
            double newG = lerp(startColor.G, endColor.G, v);
            double newB = lerp(startColor.B, endColor.B, v);
            Color newColor = new Color(newR, newG, newB);
            _CurrentTypeColor = newColor;
            BackgroundColor = _CurrentTypeColor;

        }
        private double lerp(double a, double b, double t)
        {
            return t * (b - a) + a;
        }
        

        private Color _BarTypeToColor(BarType barType)
        {
            switch(barType)
            {
                case BarType.Error:
                    return new Color(255/255.0, 218/255.0, 218/255.0);
                case BarType.Warning:
                    return new Color(255/255.0, 253/255.0, 199/255.0);
                case BarType.Succeed:
                    return new Color(215/255.0, 255/255.0, 221/255.0);
            }
            return Color.Black;
        }

        

		public SnackBar ()
		{
            _TypeIcon = new Image() { Margin = new Thickness(5, 0, 5, 0), VerticalOptions = LayoutOptions.Center };
            BarType = BarType.Error;
            BarHeight = 30;
            Text = "Really?? No Message??sdfasdfasdfasdf";
            HasShadow = true;
            Padding = new Thickness(0, 5, 0, 5);
            AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);
            AbsoluteLayout.SetLayoutBounds(this, new Rectangle(0.5, 0.95, 0.95, BarHeight));
            Button b;
            TapGestureRecognizer btnTap = new TapGestureRecognizer();
            btnTap.Tapped += onBtnTapped;
            //TextButton label = new TextButton();
            TextButton label = new TextButton() { Text = "Cancel", FontSize = 12, HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.Center, Margin = new Thickness(5, 0, 5, 0) };
            Content = new StackLayout {
                Orientation = StackOrientation.Horizontal,
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
        }

    }
}