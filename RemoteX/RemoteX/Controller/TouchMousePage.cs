using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Xamarin.Forms;
using RemoteX.Input;


namespace RemoteX.Controller
{
	public class TouchMousePage : ControllerPage
	{
        
		public TouchMousePage ():base()
		{
            IInputManager inputManager = DependencyService.Get<IInputManager>();
            
            inputManager.OnTouchAction += onTouchAction;
            Title = "Touch Mouse Page";
			Content = new StackLayout {
				Children = {
					new Label { Text = "Welcome to Xamarin.Forms!" }
				}
			};
		}

        void onTouchAction(ITouch touch, TouchMotionAction action)
        {
            ITouch[] touches = DependencyService.Get<IInputManager>().Touches;
            string s = "";
            for(int i = 0; i<touches.Length;i++)
            {
                s += touches[i];
            }
            //Debug.WriteLine(s);
        }
	}
}