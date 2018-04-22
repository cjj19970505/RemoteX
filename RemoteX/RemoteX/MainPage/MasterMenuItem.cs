using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
namespace RemoteX.MainPage
{
    class MasterMenuItem
    {
        public string Title { get; set; }
        public ContentPage TargetPage { get; set; }
        public MasterMenuItem(string title, ContentPage targetPage)
        {
            this.Title = title;
            this.TargetPage = targetPage;
        }
        public MasterMenuItem(string title)
        {
            this.Title = title;
            this.TargetPage = null;
        }
    }
}
