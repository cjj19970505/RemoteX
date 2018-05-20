using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using RemoteX.UI;
using System.Diagnostics;

namespace RemoteX.Controller
{
    /// <summary>
    /// 所有的控制器页面都从这里继承吧
    /// 控制器页面都添加到Content里面
    /// </summary>
	public class ControllerPage : ContentPage
	{
        private View _ControllerContentView;
        private AbsoluteLayout _DefaultLayout;
        SnackBar _ConnectionStateSnackBar;
        IConnectionManager _ConnectionManager;
        public View ControllerContentView
        {
            get
            {
                return _ControllerContentView;
            }
            protected set
            {
                if(_ControllerContentView != null)
                {
                    _DefaultLayout.Children.Remove(_ControllerContentView);
                }
                this._ControllerContentView = value;
                AbsoluteLayout.SetLayoutFlags(_ControllerContentView, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(_ControllerContentView, new Rectangle(0, 0, 1, 1));
                _DefaultLayout.Children.Add(_ControllerContentView);
            }
        }
        public ControllerPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            _DefaultLayout = new AbsoluteLayout();
            _ConnectionStateSnackBar = new SnackBar();
            _ConnectionManager = DependencyService.Get<IConnectionManager>();
            
            if (_ConnectionManager.ControllerConnection == null || _ConnectionManager.ControllerConnection.ConnectionEstablishState == ConnectionEstablishState.NoEstablishment)
            {
                _ConnectionStateSnackBar.BarType = BarType.Error;
            }
            else if (_ConnectionManager.ControllerConnection.ConnectionEstablishState == ConnectionEstablishState.Connecting)
            {
                _ConnectionStateSnackBar.BarType = BarType.Warning;
            }
            else if(_ConnectionManager.ControllerConnection.ConnectionEstablishState == ConnectionEstablishState.Succeed)
            {
                _ConnectionStateSnackBar.IsVisible = false;
            }
            Content = _DefaultLayout;
            _DefaultLayout.Children.Add(_ConnectionStateSnackBar);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ConnectionManager.onControllerConnectionEstalblishResult += _OnConnectionResult;
            Debug.WriteLine("FUCK");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _ConnectionManager.onControllerConnectionEstalblishResult -= _OnConnectionResult;
        }

        private void _OnConnectionResult(IConnection connection, ConnectionEstablishState connectionEstablishState)
        {
            Debug.WriteLine("BULLSHIT2");
            if (connectionEstablishState == ConnectionEstablishState.Succeed)
            {
                _ConnectionStateSnackBar.BarType = BarType.Succeed;
                //_ConnectionStateSnackBar.FadeTo(0, 1000);
            }
            if(connectionEstablishState == ConnectionEstablishState.Disconnect)
            {
                _ConnectionStateSnackBar.BarType = BarType.Error;
            }
            if (connectionEstablishState == ConnectionEstablishState.Connecting)
            {
                _ConnectionStateSnackBar.BarType = BarType.Error;
            }


        }
	}
}