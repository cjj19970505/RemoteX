using System;
using System.Collections.Generic;
using System.Text;
using SkiaBehaviour;
using SkiaSharp;
using Xamarin.Forms;
using RemoteX.Input;

namespace RemoteX.SkiaComponent
{
    public abstract class Button : SkiaObject
    {

        protected virtual IArea Area { get; set; }
        protected List<ITouch> OnTouches;
        private IInputManager inputManager;
        protected override void Init()
        {
            inputManager = DependencyService.Get<IInputManager>();
            inputManager.OnTouchAction += handleTouchAction;
            OnTouches = new List<ITouch>();
        }
        private void handleTouchAction(ITouch touch, TouchMotionAction action)
        {
            CanvasInfoProvider canvasInfoProvider = SkiaBehaviourEngine.CanvasInfoProvider as CanvasInfoProvider;
            if (action == TouchMotionAction.Down)
            {
                if (Area.IsOverlapPoint(canvasInfoProvider.DeviceToCanvasPoint(touch.Position)))
                {
                    bool firstTouch = false;
                    if (OnTouches.Count == 0)
                    {
                        firstTouch = true;
                    }
                    OnTouches.Add(touch);
                    if (firstTouch)
                    {
                        OnButtonPressed();
                    }
                }
            }
            else if (action == TouchMotionAction.Move)
            {
                if (!OnTouches.Contains(touch) && Area.IsOverlapPoint(touch.Position))
                {
                    bool firstTouch = false;
                    if (OnTouches.Count == 0)
                    {
                        firstTouch = true;
                    }
                    OnTouches.Add(touch);
                    if (firstTouch)
                    {
                        OnButtonPressed();
                    }

                }
                else if (OnTouches.Contains(touch) && !Area.IsOverlapPoint(touch.Position))
                {
                    OnTouches.Remove(touch);
                    if (OnTouches.Count == 0)
                    {
                        OnButtonUp();
                    }
                }
            }
            else if (action == TouchMotionAction.Up)
            {
                if (OnTouches.Contains(touch))
                {
                    OnTouches.Remove(touch);
                    if (OnTouches.Count == 0)
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
            inputManager.OnTouchAction -= handleTouchAction;
        }


    }
}
