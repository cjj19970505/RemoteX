using SkiaBehaviour;
using System;
using System.Collections.Generic;
using System.Text;
using RemoteX.Input;
using Xamarin.Forms;
using SkiaSharp;

namespace RemoteX.SkiaComponent
{
    public class SkiaInputManager : SkiaObject
    {
        private List<SkiaTouch> skiaTouches;
        public delegate void SkiaTouchMotionHandler(SkiaTouch skiaTouch, TouchMotionAction action);
        public event SkiaTouchMotionHandler OnSkiaTouchAction;
        protected override void Init()
        {
            base.Init();
            IInputManager inputManager = DependencyService.Get<IInputManager>();
            skiaTouches = new List<SkiaTouch>();
            inputManager.OnTouchAction += handleTouchAction;
            
        }
        public override void Update()
        {
            IInputManager inputManager = DependencyService.Get<IInputManager>();
        }
        private void handleTouchAction(ITouch touch, TouchMotionAction action)
        {
            CanvasInfoProvider canvasInfoProvider = SkiaBehaviourEngine.CanvasInfoProvider as CanvasInfoProvider;
            SkiaTouch skiaTouch = null;
            if (action == TouchMotionAction.Down)
            {
                int heightLevel = 0;
                ISkiaInputComponent skiaInputComponent = getSkiaInputComponent(canvasInfoProvider.DeviceToCanvasPoint(touch.Position.ToSKPoint()));
                if (skiaInputComponent != null)
                {
                    heightLevel = skiaInputComponent.InputHeightLevel;
                }
                skiaTouch = new SkiaTouch(touch, heightLevel);
                skiaTouches.Add(skiaTouch);
            }
            else if (action == TouchMotionAction.Up)
            {
                for (int i = 0; i < skiaTouches.Count; i++)
                {
                    if (skiaTouches[i].Touch == touch)
                    {
                        skiaTouch = skiaTouches[i];
                        skiaTouches.RemoveAt(i);
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < skiaTouches.Count; i++)
                {
                    if (skiaTouches[i].Touch == touch)
                    {
                        skiaTouch = skiaTouches[i];
                    }
                }
            }
            if (skiaTouch == null)
            {
                return;
            }
            OnSkiaTouchAction?.Invoke(skiaTouch, action);
        }

        private ISkiaInputComponent getSkiaInputComponent(SKPoint point)
        {
            List<ISkiaInputComponent> inputComponents = new List<ISkiaInputComponent>();
            for (int i = 0; i < SkiaBehaviourEngine.SkiaObjectCount; i++)
            {
                SkiaObject skiaObject = SkiaBehaviourEngine.GetSkiaObject(i);
                if (skiaObject is ISkiaInputComponent)
                {
                    ISkiaInputComponent inputComponent = skiaObject as ISkiaInputComponent;
                    if (inputComponent.FirstTouchArea.IsOverlapPoint(point))
                    {
                        inputComponents.Add(inputComponent);
                    }
                }
            }
            if (inputComponents.Count == 0)
            {
                return null;
            }
            ISkiaInputComponent topInputComponent = inputComponents[0];
            for (int i = 0; i < inputComponents.Count; i++)
            {
                if (inputComponents[i].InputHeightLevel > topInputComponent.InputHeightLevel)
                {
                    topInputComponent = inputComponents[i];
                }
            }
            return topInputComponent;
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            IInputManager inputManager = DependencyService.Get<IInputManager>();
            inputManager.OnTouchAction -= handleTouchAction;
        }

    }

    public class SkiaTouch
    {
        public ITouch Touch { get; private set; }
        public int HeightLevel { get; private set; }
        public SkiaTouch(ITouch touch, int heightLevel)
        {
            HeightLevel = heightLevel;
            Touch = touch;
        }
    }
}
