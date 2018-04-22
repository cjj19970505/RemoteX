using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace RemoteX
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

            //MainPage = new RemoteX.MainPage();
            //MainPage = new NavigationPage(new Bluetooth.BluetoothDeviceListPage());
            MainPage = new MainPage.MainMasterDetailPage();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
