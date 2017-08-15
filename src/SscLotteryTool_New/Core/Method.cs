using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SscLotteryTool
{
    public class Method
    {
        public static List<int> GetNumber(string numberStr)
        {
            List<int> buffer = new List<int>();
            if (string.IsNullOrEmpty(numberStr))
            {
                return buffer;
            }
            int x = int.Parse(numberStr);
            int n = 0;
           

            while (x != 0)
            {
                int y = x % 10;
                if (n < 3) buffer.Add(y);
                x /= 10;
                n++;
            }
            return buffer;
        }
    }
}
