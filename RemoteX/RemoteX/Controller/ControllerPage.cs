using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RemoteX.Controller
{
    /// <summary>
    /// 所有的控制器页面都从这里继承吧
    /// </summary>
	public class ControllerPage : ContentPage
	{
        public virtual void OnEnterPage()
        {

        }

        /// <summary>
        /// 一定要Call一下base.OnExitPage()
        /// </summary>
        public virtual async Task OnExitPage()
        {
            await Navigation.PopAsync();
        }
	}
}