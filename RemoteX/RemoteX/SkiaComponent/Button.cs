using System;
using System.Collections.Generic;
using System.Text;
using RemoteX.Input;
using SkiaSharp;
using SkiaSharp.Views;
using Xamarin.Forms;

namespace RemoteX.SkiaComponent
{
    public class Button:SkiaComponent
    {

        protected ICollider Collider { get; set; }

        public bool Pressing { get; protected set; }

        protected IInputManager InputManager;

        /// <summary>
        /// 正在按钮上面乱摸的Touch
        /// </summary>
        protected List<ITouch> OnTouches;

        public Button():base()
        {
            InputManager = DependencyService.Get<IInputManager>();
            OnTouches = new List<ITouch>();
        }

        
        /// <summary>
        /// 强烈建议重写时base.Update()一下
        /// </summary>
        public virtual void Update()
        {
            ITouch[] touches = InputManager.Touches;
            List<ITouch> touchesOnThisUpdate = new List<ITouch>();
            bool noTouchesBefore = OnTouches.Count == 0;
            foreach(ITouch touch in touches)
            {
                if(Collider.OverlapPoint(touch.Position))
                {
                    touchesOnThisUpdate.Add(touch);
                }
            }
            List<ITouch> leavedTouches = new List<ITouch>(OnTouches);
            List<ITouch> newTouches = new List<ITouch>();
            foreach(ITouch touch in touchesOnThisUpdate)
            {
                if(leavedTouches.Contains(touch))
                {
                    leavedTouches.Remove(touch);
                }
                else
                {
                    newTouches.Add(touch);
                }
            }
            for(int i = OnTouches.Count-1;i>=0;i--)
            {
                if(leavedTouches.Contains(OnTouches[i]))
                {
                    OnTouches.Remove(OnTouches[i]);
                }
            }
            OnTouches.AddRange(newTouches);

            if (leavedTouches.Count > 0 && OnTouches.Count == 0)
            {
                OnUp();
            }
            else if(noTouchesBefore && newTouches.Count>0)
            {
                OnPressed();
            }
        }
        protected virtual void OnPressed()
        {
            System.Diagnostics.Debug.WriteLine("PRESSED");
        }
        protected virtual void OnUp()
        {
            System.Diagnostics.Debug.WriteLine("UPUP");
        }




    }
}
