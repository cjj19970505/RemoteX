using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace RemoteX.UI
{
    class TextButton:Label
    {
        private TapGestureRecognizer _TapGestureRecognizer;
        public TextButton():base()
        {
            _TapGestureRecognizer = new TapGestureRecognizer();
            _TapGestureRecognizer.Tapped += onTapped;
            GestureRecognizers.Add(_TapGestureRecognizer);
        }

        private void onTapped(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("FUCK");
        }
    }
}
