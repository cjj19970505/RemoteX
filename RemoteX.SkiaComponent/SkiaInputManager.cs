﻿using SkiaBehaviour;
using System;
using System.Collections.Generic;
using System.Text;
using RemoteX.Input;
using SkiaSharp;

namespace RemoteX.SkiaComponent
{
    public class SkiaInputManager : SkiaObject
    {
        private List<SkiaTouch> _SkiaTouches;
        public delegate void SkiaTouchMotionHandler(SkiaTouch skiaTouch, TouchMotionAction action);
        public event SkiaTouchMotionHandler OnSkiaTouchAction;
        private IInputManager _InputManager;
        public IInputManager InputManager
        {
            get
            {
                return InputManager;
            }
            set
            {
                _InputManager = value;
                _InputManager.OnTouchAction += handleTouchAction;
            }
        }
        public SkiaTouch[] Touches
        {
            get
            {
                return _SkiaTouches.ToArray();
            }
        }

        protected override void Init()
        {
            base.Init();
            _SkiaTouches = new List<SkiaTouch>();
            
        }
        public override void Update()
        {
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
                skiaTouch = new SkiaTouch(this, touch, heightLevel);
                _SkiaTouches.Add(skiaTouch);
            }
            else if (action == TouchMotionAction.Up)
            {
                for (int i = 0; i < _SkiaTouches.Count; i++)
                {
                    if (_SkiaTouches[i].Touch == touch)
                    {
                        skiaTouch = _SkiaTouches[i];
                        _SkiaTouches.RemoveAt(i);
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _SkiaTouches.Count; i++)
                {
                    if (_SkiaTouches[i].Touch == touch)
                    {
                        skiaTouch = _SkiaTouches[i];
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
            InputManager.OnTouchAction -= handleTouchAction;
        }

    }

    public class SkiaTouch
    {
        /// <summary>
        /// this should be private later
        /// </summary>
        public ITouch Touch { get; private set; }
        public int HeightLevel { get; private set; }
        public SkiaInputManager SkiaInputManager { get; }
        public SkiaTouch(SkiaInputManager skiaInputManager ,ITouch touch, int heightLevel)
        {
            HeightLevel = heightLevel;
            Touch = touch;
            SkiaInputManager = skiaInputManager;
        }

        /// <summary>
        /// Now in CanvasPhysicsSpace
        /// </summary>
        public SKPoint Position
        {
            get
            {
                throw new NotImplementedException();

            }
        }
    }
}
