using SkiaSharp;
using SkiaSharp.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class ListBox : Box
    {
        private Orientation orientation;
        private SKColor backgroundColor = SKColors.Transparent;
        private SKColor foregroundColor;
        private int selectedIndex;

        public override SKColor BackgroundColor
        {
            get => backgroundColor; set
            {
                backgroundColor = value;

                Items.Select(x => x.BackgroundColor = backgroundColor);
            }
        }

        public override SKColor ForegroundColor
        {
            get => foregroundColor; set
            {
                foregroundColor = value;

                Items.Select(x => x.ForegroundColor = foregroundColor);
            }
        }

        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }

            set
            {
                if (selectedIndex != value)
                {
                    SelectedItem?.RemoveSelection();

                    if(value > -1 && value < Items.Count)
                    {
                        SelectedItem?.RemoveSelection();

                        selectedIndex = value;
                    }
                    else
                    {
                        selectedIndex = -1;
                    }
                }

                Scene.IManager.Instance.InvalidateMeasure();
            }
        }

        public ListItem SelectedItem
        {
            get
            {
                if (selectedIndex > -1 && selectedIndex < Items.Count)
                {
                    return Items[selectedIndex];
                }

                return null;
            }

            set
            {
                var index = Items.FindIndex(x => x == value);
                if (index > -1)
                {
                    SelectedItem?.RemoveSelection();

                    value.OnSelect();
                    value.OnHover();

                    selectedIndex = index;
                }
            }
        }

        public override Orientation Orientation
        {
            get => orientation; set
            {

            }
        }

        public List<ListItem> Items { get; set; }

        public ListBox(SKRect bounds) : base(bounds)
        {
            Items = new List<ListItem>();

            orientation = Orientation.Vertical;

            HorizontalAlignment = LayoutOptions.Stretch;

            ScrollEnabled = true;
        }

        public void Add(object item)
        {
            lock (Items)
            {
                ListItem listItem = new ListItem(item);

                listItem.Margin = new Margin(0);

                Items.Add(listItem);

                AddElement(listItem);

                listItem.ForegroundColor = ForegroundColor;
                listItem.BackgroundColor = BackgroundColor;

                listItem.Activate += ListItem_Activate;

                OnItemsChanged(Items.Count - 1, item);
            }
        }

        public void Clear()
        {
            Items.Clear();
            
            OnItemsChanged(-1, null);
        }

        private void ListItem_Activate(object sender, EventArgs e)
        {
            if (sender is ListItem listItem)
            {
                SelectedIndex = Items.FindIndex(x => x == listItem);
            }
        }

        public void Remove(object item)
        {
            lock (Items)
            {
                var listItemIndex = Items.FindIndex(x => x.Value == item);

                if (listItemIndex != -1)
                {
                    var listItem = Items[listItemIndex];

                    Items.RemoveAt(listItemIndex);

                    listItem.Dispose();

                    listItem.Activate -= ListItem_Activate;

                    OnItemsChanged(listItemIndex, listItem.Value);
                }
            }
        }

        public void RemoveAt(int index)
        {
            lock (Items)
            {
                if (index > -1 && index < Items.Count)
                {
                    var listItem = Items[index];

                    Items.RemoveAt(index);

                    listItem.Dispose();

                    listItem.Activate -= ListItem_Activate;

                    OnItemsChanged(index, listItem.Value);
                }
            }
        }

        public void Insert(object item, int index)
        {
            lock (Items)
            {
                ListItem listItem = new ListItem(item);

                listItem.Margin = new Margin(0);

                Items.Insert(index, listItem);

                AddElement(listItem);

                listItem.Activate += ListItem_Activate;

                OnItemsChanged(index, item);
            }
        }

        public override void Measure()
        {
            lock (Items)
            {
                Elements.Clear();

                Elements.AddRange(Items.ToArray());

                var selected = SelectedItem;

                foreach (var item in Items)
                {
                    if (item != selected)
                    {
                        item.RemoveSelection();
                    }
                    else
                    {
                        item.OnSelect();
                    }
                }
            }

            base.Measure();
        }

        private void OnItemsChanged(int index, object item = null)
        {
            Scene.IManager.Instance.InvalidateMeasure();
        }
    }
}