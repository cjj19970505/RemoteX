using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RemoteX;
using RemoteX.Service;

[assembly: Xamarin.Forms.Dependency(typeof(RemoteX.Droid.VibratorWrapper))]
namespace RemoteX.Droid
{
    class VibratorWrapper : IVibrator
    {
        private Vibrator vibrator;
        public bool HasVibrator
        {
            get
            {
                if(vibrator == null)
                {
                    return false;
                }
                return vibrator.HasVibrator;
            }
        }

        public VibratorWrapper()
        {
            this.vibrator = (Vibrator)Application.Context.GetSystemService(Context.VibratorService);
        }
        public void Vibrate()
        {
            vibrator.Vibrate(VibrationEffect.CreateOneShot(30, 255));
        }
    }
}