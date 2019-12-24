using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TabViewSam.Utils
{
    internal static class ColorSelector
    {
        internal static Color Set(params Color[] colors)
        {
            foreach (var color in colors)
            {
                if (!color.IsDefault)
                    return color;
            }
            return Color.Default;
        }
    }
}
