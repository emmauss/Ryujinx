using System;

namespace Ryujinx.Ui
{
    public class ApplicationWidget : IDisposable
    {
        public ApplicationList Widget { get; set; }

        public ApplicationWidget()
        {
            Widget = new ApplicationList
            {
                Expand = true
            };
        }

        public void Update(ApplicationData applicationsData)
        {
            Widget.AddItem(applicationsData);
        }

        public void Clear()
        {
            Widget.ClearItems();
        }
        public void Dispose()
        {
            Widget.Dispose();
        }
    }
}
