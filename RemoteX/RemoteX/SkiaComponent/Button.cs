using System;
using System.Collections.Generic;
using System.Text;
using SkiaBehaviour;
using SkiaSharp;
using Xamarin.Forms;
using RemoteX.Input;

namespace RemoteX.SkiaComponent
{
    public abstract class Button : SkiaObject, ISkiaInputComponent
    {

        protected virtual IArea Area { get; set; }

        public int InputHeightLevel
        {
            get
            {
                return 1;
            }
        }

        public IArea FirstTouchArea
        {
            get
            {
                return Area;
            }
        }

        //protected List<ITouch> OnTouches;
        protected List<SkiaTouch> OnSkiaTouches;
        //private IInputManager inputManager;
        protected override void Init()
        {
            SkiaBehaviourEngine.GetSkiaInputManager().OnSkiaTouchAction += handleSkiaTouchAction;
            OnSkiaTouches = new List<SkiaTouch>();
        }
        private void handleSkiaTouchAction(SkiaTouch skiaTouch, TouchMotionAction action)
        {
            if(skiaTouch.HeightLevel != InputHeightLevel)
            {
                return;
            }
            ITouch touch = skiaTouch.Touch;
            CanvasInfoProvider canvasInfoProvider = SkiaBehaviourEngine.CanvasInfoProvider as CanvasInfoProvider;
            if (action == TouchMotionAction.Down)
            {
                if (Area.IsOverlapPoint(canvasInfoProvider.DeviceToCanvasPoint(touch.Position.ToSKPoint())))
                {
                    bool firstTouch = false;
                    if (OnSkiaTouches.Count == 0)
                    {
                        firstTouch = true;
                    }
                    OnSkiaTouches.Add(skiaTouch);
                    if (firstTouch)
                    {
                        OnButtonPressed();
                    }
                }
            }
            else if (action == TouchMotionAction.Move)
            {
                if (!OnSkiaTouches.Contains(skiaTouch) && Area.IsOverlapPoint(touch.Position.ToSKPoint()))
                {
                    bool firstTouch = false;
                    if (OnSkiaTouches.Count == 0)
                    {
                        firstTouch = true;
                    }
                    OnSkiaTouches.Add(skiaTouch);
                    if (firstTouch)
                    {
                        OnButtonPressed();
                    }

                }
                else if (OnSkiaTouches.Contains(skiaTouch) && !Area.IsOverlapPoint(touch.Position.ToSKPoint()))
                {
                    OnSkiaTouches.Remove(skiaTouch);
                    if (OnSkiaTouches.Count == 0)
                    {
                        OnButtonUp();
                    }
                }
            }
            else if (action == TouchMotionAction.Up)
            {
                if (OnSkiaTouches.Contains(skiaTouch))
                {
                    OnSkiaTouches.Remove(skiaTouch);
                    if (OnSkiaTouches.Count == 0)
                    {
                        OnButtonUp();
                    }
                }
            }
        }
        protected abstract void OnButtonPressed();
        protected abstract void OnButtonUp();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            SkiaBehaviourEngine.GetSkiaInputManager().OnSkiaTouchAction -= handleSkiaTouchAction;
        }


    }
}
