using RemoteX.Controller;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.MainPage
{
    class ControllerItemsGroup:MasterItemGroup
    {
        
        public ControllerItemsGroup():base("Controller")
        {
            this.Add(new MasterMenuItem("Message Talker", typeof(WordsTalkerPage)));
            this.Add(new MasterMenuItem("SensorTestr", typeof(SensorTestPage)));
            this.Add(new MasterMenuItem("TouchTest", typeof(TouchMousePage)));
            this.Add(new MasterMenuItem("SkiaTest", typeof(SkiaTestController)));
            this.Add(new MasterMenuItem("SKiaController", typeof(SkiaController)));
            //this.Add());
            // new Controller.SensorTestPage();
        }
    }
}
