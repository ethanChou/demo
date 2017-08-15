using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SscLotteryTool
{
    public class RandomData
    {
        public int D1 { get; set; }
        public int D2 { get; set; }
        public int D3 { get; set; }

        public bool IsSample(RandomData data)
        {
            List<int> t = new List<int>() { D1, D2, D3 };

            if (t.Contains(data.D1) && t.Contains(data.D2) && t.Contains(data.D3))
            {
                return true;
            }

            return false;
        }

        public bool Contains(int num)
        {
            if (D1 == num || D2 == num || D3 == num)
            {
                return true;
            }

            return false;
        }

        public RandomData Clone()
        {
            RandomData d = new RandomData() { D1 = this.D1, D2 = this.D2, D3 = this.D3 };
            return d;
        }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}", D1, D2, D3);
        }
    }

    public class DisplayData
    {
        public RandomData HighData { get; set; }
        public RandomData LowData { get; set; }

        public int Tick { get; set; }

        public DisplayData Clone()
        {

            DisplayData dt = new DisplayData();
            if (HighData != null)
                dt.HighData = this.HighData.Clone();
            if (LowData != null)
                dt.LowData = this.LowData.Clone();
            dt.Tick = this.Tick;

            return dt;
        }
    }
}
