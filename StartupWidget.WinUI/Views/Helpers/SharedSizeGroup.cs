using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.PointOfService;

namespace StartupWidget.WinUI.Views.Helpers
{
    public class SharedSizeGroup
    {
        private readonly List<FrameworkElement> elements = [];
        private SizeType _sizeType;
        public SharedSizeGroup(SizeType sizeType)
        {
            _sizeType = sizeType;
        }

        public void Register(FrameworkElement Content)
        {
            elements.Add(Content);
        }

        public void Calculate()
        {
            SetMinSize();
            double max = FindMaxSize();
            ApplySize(max);
        }

        private void SetMinSize()
        {
            foreach(var e in elements)
            {
                if (_sizeType == SizeType.Height)
                {
                    e.MinHeight = 0;
                }
                else
                {
                    e.MinWidth = 0;
                }
                e.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
            }
        }

        private double FindMaxSize()
        {
            double max = 0;
            foreach (var i in elements)
            {
                if (_sizeType == SizeType.Height)
                {
                    if (max < i.DesiredSize.Height)
                    {
                        max = i.DesiredSize.Height;
                    }
                }
                else
                {
                    if (max < i.DesiredSize.Width)
                    {
                        max = i.DesiredSize.Width;
                    }
                }
            }
            return max;
        }

        private void ApplySize(double value)
        {
            foreach(var i in elements)
            {
                if(_sizeType == SizeType.Height)
                {
                    i.MinHeight = value;
                }
                else
                {
                    i.MinWidth = value;
                }
            }
        }
    }

    public enum SizeType
    {
        Height,Width
    }
}
