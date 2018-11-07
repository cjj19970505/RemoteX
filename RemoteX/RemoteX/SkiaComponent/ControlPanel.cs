using SkiaBehaviour;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX.SkiaComponent
{
    public abstract class ControlPanel
    {
        public SkiaBehaviourEngine SkiaBehaviourEngine { get; private set; }
        public ControlPanel(SkiaBehaviourEngine skiaBehaviourEngine)
        {
            SkiaBehaviourEngine = skiaBehaviourEngine;
        }

        public virtual void Start()
        {

        }
        public virtual void Update()
        {

        }
    }
}
