using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteX
{
    static class Extensions
    {
        public static void PrintArray(this Array self)
        {
            string s = "";
            for(int i = 0; i < self.Length; i++)
            {
                s += self.GetValue(i);
                if(i == self.Length - 1)
                {
                    
                }
                else
                {
                    s += ", ";
                }
            }
            System.Diagnostics.Debug.WriteLine(s);
        }
    }
}
