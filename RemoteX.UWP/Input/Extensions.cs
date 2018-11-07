using RemoteX.Data.Mathf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace RemoteX.UWP.Input
{
    public static class Extensions
    {
        public static Vector2 ToVector2(this Point self)
        {
            return new Vector2((float)self.X, (float)self.Y);
        }
    }
}
