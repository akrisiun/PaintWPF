using System;
using System.Collections.Generic;
using System.Linq;
using Paint.WPF.Tools;
using System.Windows.Media;
using System.Windows;

namespace Paint.WPF
{
    /// <summary>
    /// Static paint editor state
    /// </summary>
    internal static class GlobalState
    {
        public static bool PressLeftButton { get; set; }

        public static event EventHandler ChangeInstrument;
        public static event EventHandler ChangeColor;

        public static int LayersCount { get; set; }

        public static int LayersIndexes { get; set; }

        private static Brush _color = Brushes.Black;
        public static Brush Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                ChangeColor(value, null);
            }
        }

        private static Instruments _currentTool = Instruments.Arrow;
        public static Instruments CurrentTool
        {
            get
            {
                return _currentTool;
            }
            set
            {
                _currentTool = value;
                ChangeInstrument(value, null);
            }
        }

        private static Size _brushSize;
        public static Size BrushSize
        {
            get { return _brushSize; }
            set { _brushSize = value; }
        }
        
    }
}
