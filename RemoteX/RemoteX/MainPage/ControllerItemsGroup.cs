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
        }
    }
}
