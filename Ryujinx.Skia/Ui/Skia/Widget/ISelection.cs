using System;
using System.Collections.Generic;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public interface ISelection
    {
        event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        SelectionMode SelectionMode{ get; set; }

        bool SelectionEnabled{ get;}

        List<UIElement> SelectedItems{ get; set; }

        UIElement SelectedItem{ get; set; }

        int SelectedIndex{ get; set; }
    }

    public class SelectionChangedEventArgs : EventArgs
    {
        public int PreviousIndex{ get; set; }
        public int CurrentIndex{ get; set; }

        public UIElement SelectedItem{ get; set; }
    }
}