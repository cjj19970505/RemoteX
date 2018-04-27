using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.MainPage
{
    class ControllerItemsGroup:MasterItemGroup
    {
        
        public ControllerItemsGroup():base("Controller")
        {
            this.Add(new MasterMenuItem("Message Talker", new Controller.WordsTalkerPage()));
            this.Add(new MasterMenuItem("Mother Fucker", new Controller.SensorTestPage()));
            this.Add(new MasterMenuItem("Mother Fucker", new Controller.TouchMousePage()));
            //this.Add());
            // new Controller.SensorTestPage();
        }
    }
}
