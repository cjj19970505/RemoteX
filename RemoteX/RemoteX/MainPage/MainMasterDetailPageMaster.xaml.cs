using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RemoteX.MainPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMasterDetailPageMaster : ContentPage
    {
        public ListView ListView;
        public Page DetailPage { get; set; }
        public MainMasterDetailPageMaster()
        {
            InitializeComponent();
            //BindingContext = new MainMasterDetailPageMasterViewModel();
            ListView = MenuItemsListView;
            ListView.ItemsSource = MasterItemGroup.All;
            ListView.ItemSelected += onMenuItemSelected;
        }

        private async void onMenuItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem == null)
            {
                return;
            }
            MasterMenuItem item = e.SelectedItem as MasterMenuItem;
            if(item.TargetPageType != typeof(ContentPage))
            {
                await DetailPage.Navigation.PushAsync((Page)Activator.CreateInstance(item.TargetPageType));
            }
        }

        class MainMasterDetailPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MainMasterDetailPageMenuItem> MenuItems { get; set; }
            
            public MainMasterDetailPageMasterViewModel()
            {
                MenuItems = new ObservableCollection<MainMasterDetailPageMenuItem>(new[]
                {
                    new MainMasterDetailPageMenuItem { Id = 0, Title = "Page 1" },
                    new MainMasterDetailPageMenuItem { Id = 1, Title = "Page 2" },
                    new MainMasterDetailPageMenuItem { Id = 2, Title = "Page 3" },
                    new MainMasterDetailPageMenuItem { Id = 3, Title = "Page 4" },
                    new MainMasterDetailPageMenuItem { Id = 4, Title = "Page 5" },
                });
            }
            
            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}