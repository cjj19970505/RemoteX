using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
namespace RemoteX.MainPage
{
    class MasterMenuItem
    {
        public string Title { get; set; }
        public Type TargetPageType { get; set; }
        public MasterMenuItem(string title, Type targetPageType)
        {
            this.Title = title;
            this.TargetPageType = targetPageType;
        }
    }
}
