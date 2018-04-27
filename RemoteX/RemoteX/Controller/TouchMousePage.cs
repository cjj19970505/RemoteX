using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Xamarin.Forms;
using RemoteX.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RemoteX.Controller
{
	public class TouchMousePage : ControllerPage
	{
        private ObservableCollection<TouchInfoItem> _TouchInfoItems;
		public TouchMousePage ():base()
		{
            IInputManager inputManager = DependencyService.Get<IInputManager>();
            _TouchInfoItems = new ObservableCollection<TouchInfoItem>();
            inputManager.OnTouchAction += onTouchAction;
            Title = "Touch Mouse Page";
            ListView touchesListView = new ListView()
            {
                ItemsSource = _TouchInfoItems,
                ItemTemplate = new DataTemplate(typeof(TouchInfoCell))
            };
            Content = new StackLayout {
				Children = {
					new Label { Text = "Input" },
                    touchesListView
				}
			};
            
            
		}
        private class TouchInfoCell:ViewCell
        {
            public TouchInfoCell()
            {
                Label idLabel = new Label();
                idLabel.SetBinding(Label.TextProperty, "Id");
                Label posLabel = new Label();
                posLabel.SetBinding(Label.TextProperty, "Position");
                this.View = new StackLayout()
                {

                    Children = {
                        idLabel,
                        posLabel
                    }
                };
            }
        }

        private class TouchInfoItem: INotifyPropertyChanged
        {
            private ITouch _Touch;
            public TouchInfoItem(ITouch touch)
            {
                this._Touch = touch;
            }
            public ITouch Touch
            {
                get
                {
                    return _Touch;
                }
            }
            public string Position
            {
                get
                {
                    return _Touch.Position.ToString();
                }
            }
            public int Id
            {
                get
                {
                    return _Touch.Id;
                }
            }
            public void propertyChange()
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Position"));
            }
            public event PropertyChangedEventHandler PropertyChanged;
        }

        void onTouchAction(ITouch touch, TouchMotionAction action)
        {
            if(action == TouchMotionAction.Down)
            {
                _TouchInfoItems.Add(new TouchInfoItem(touch));
            }
            else if(action == TouchMotionAction.Up)
            {
                for(int i = 0; i<_TouchInfoItems.Count;i++)
                {
                    if(_TouchInfoItems[i].Touch == touch)
                    {
                        _TouchInfoItems.Remove(_TouchInfoItems[i]);
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _TouchInfoItems.Count; i++)
                {
                    if (_TouchInfoItems[i].Touch == touch)
                    {
                        _TouchInfoItems[i].propertyChange();
                        break;
                    }
                }
            }
        }
	}
}