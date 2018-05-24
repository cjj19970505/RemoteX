using System;
using System.Collections.Generic;
using System.Text;

namespace SkiaBehaviour
{
    /// <summary>
    /// 一个控件存在的最基本底层构架
    /// </summary>
    public class SkiaObject:ISkiaBehaviourEngineProvider
    {
        public ICanvasInfoProvider CanvasInfoProvider { get; private set; }
        public RectTransform RectTransform { get; private set; }

        public SkiaBehaviourEngine SkiaBehaviourEngine { get; private set; }
        public bool Enable { get; set; }
        public SkiaObject(SkiaBehaviourEngine engine)
        {
            SkiaBehaviourEngine = engine;
            RectTransform = new RectTransform(this);
        }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void Draw() { }
    }
}
