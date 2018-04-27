using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.MainPage
{
    class MasterItemGroup:List<MasterMenuItem>
    {
        public string Title { get; set; }
        protected MasterItemGroup(string title):base()
        {
            this.Title = title;
        }
        public static IList<MasterItemGroup> All { private set; get; }
        static MasterItemGroup()
        {
            List<MasterItemGroup> groups = new List<MasterItemGroup>
            {
                new MasterItemGroup("Connection")
                {
                    new MasterMenuItem("Bluetooth",new Bluetooth.BluetoothDeviceListPage()),
                    new MasterMenuItem("WIFI P2P"),
                    new MasterMenuItem("Network")
                },
                new ControllerItemsGroup()
            };
            All = groups;
        }
    }
}
