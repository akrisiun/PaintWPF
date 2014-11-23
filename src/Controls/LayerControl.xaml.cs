using Paint.WPF.Tools;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Paint.WPF.Controls
{
    /// <summary>
    /// Layer control
    /// </summary>
    public partial class LayerControl : UserControl, INotifyPropertyChanged
    {
        #region Properties

        public event EventHandler Delete;

        public event EventHandler<CheckedEventArgs> CheckedChanged;
        public class CheckedEventArgs : EventArgs
        {
            public bool IsChecked { get; set; }
        }

        private string _layerName;
        public string LayerName
        {
            get
            {
                return _layerName;
            }
            set
            {
                _layerName = value;
                OnPropertyChanged("LayerName");
            }
        }

        public int LayerIndex { get; set; }
        public bool LayerFocus { get; set; }
        internal LayerWidget Widget { get; set; }

        private Point _clickPosition;
        #endregion

        /// <summary>
        /// ctor
        /// </summary>
        public LayerControl()
        {
            InitializeComponent();
            LayerIndex = GlobalState.LayersCount++;
            // maximal ZOrder
            Panel.SetZIndex(this, LayerIndex);

            Widget = new LayerWidget(this);
            LayerName = String.Format("{0}_{1}", "NewLayer", GlobalState.LayersIndexes + 1);

            Widget.WidgetCheckBox.Checked += SetLayerVisibility;
            Widget.WidgetCheckBox.Unchecked += SetLayerVisibility;
            Widget.WidgetDel.Click += Del;

        }

        /// <summary>
        /// Visibily change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetLayerVisibility(Object sender, EventArgs e)
        {
            switch (Widget.WidgetCheckBox.IsChecked)
            {
                case true:
                    Visibility = Visibility.Visible;
                    CheckedChanged(this, new CheckedEventArgs { IsChecked = true });
                    break;
                case false:
                    Visibility = Visibility.Hidden;
                    CheckedChanged(this, new CheckedEventArgs { IsChecked = false });
                    break;
            }

        }

        private void Del(Object sender, EventArgs e)
        {
            GlobalState.LayersCount--;
            Delete(this, e);
        }

        /// <summary>
        /// Status fixation mouse down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayerControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (GlobalState.CurrentTool == Instruments.Arrow)
            {
                GlobalState.PressLeftButton = true;
                var draggableControl = (UserControl)sender;
                _clickPosition = e.GetPosition(this);
                draggableControl.CaptureMouse();
            }
        }

        /// <summary>
        /// Status fixation mouse up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayerControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (GlobalState.CurrentTool == Instruments.Arrow)
            {
                GlobalState.PressLeftButton = false;
                var draggable = (UserControl)sender;
                draggable.ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// layer with mouse move
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayerControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (GlobalState.CurrentTool == Instruments.Arrow)
            {
                var draggableControl = (UserControl)sender;

                if (GlobalState.PressLeftButton && draggableControl != null)
                {
                    Point currentPosition = e.GetPosition((UIElement)Parent);

                    var transform = draggableControl.RenderTransform as TranslateTransform;
                    if (transform == null)
                    {
                        transform = new TranslateTransform();
                        draggableControl.RenderTransform = transform;
                    }

                    transform.X = currentPosition.X - _clickPosition.X;
                    transform.Y = currentPosition.Y - _clickPosition.Y;
                }
            }
        }

        /// <summary>
        /// Focus layer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            VisualHost.IsFocused = true;
            VisualHost.FocusSpace();
        }

        //  public VisualHost Host { get { return VisualHost; } }

        /// <summary>
        /// Leave focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NonFocus(object sender, RoutedEventArgs e)
        {
            VisualHost.IsFocused = false;
            VisualHost.UnFocusSpace();
        }

        #region interface INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }
}
