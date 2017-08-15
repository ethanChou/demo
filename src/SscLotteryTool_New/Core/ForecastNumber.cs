using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SscLotteryTool
{

    public class DisplayForecast
    {

        public int Index { get; set; }

        public int SuccessCount { get; set; }

        public int FailureCount { get; set; }
    }

    public class ForecastNumber
    {
        public ForecastNumber()
        {
            //Time = DateTime.Now;
        }

        public string Zhongjianghaoma { get; set; }

        public DateTime Time { get; set; }

        public List<int> HighBits { get; set; }

        public string HighString
        {
            get
            {
                return HighBits.Count == 0 ? "" : string.Format("{0}{1}{2}", HighBits[0], HighBits[1], HighBits[2]);
            }
        }

        public List<int> LowBits { get; set; }


        public string LowString
        {
            get
            {
                return LowBits.Count == 0 ? "" : string.Format("{0}{1}{2}", LowBits[0], LowBits[1], LowBits[2]);
            }
        }

        public bool IsSuccess
        {
            get
            {
                if (HighBits.Count == 0 && LowBits.Count == 0)
                {
                    return false;
                }
                return !HighBits.Contains(High) && !LowBits.Contains(Low);
            }
        }

        public bool IsRight
        {
            get
            {
                return HighBits.Contains(High) && LowBits.Contains(Low);
            }
        }

        public int High { get; set; }
        public int Low { get; set; }

        public ForecastNumber Clone()
        {
            ForecastNumber fn = new ForecastNumber();
            fn.Time = this.Time;
            fn.HighBits = new List<int>();
            fn.HighBits.AddRange(this.HighBits);
            fn.LowBits = new List<int>();
            fn.LowBits.AddRange(this.LowBits);
            fn.High = High;
            fn.Low = Low;

            return fn;
        }
    }

    public class ForecastNumberCollection : ICollection<ForecastNumber>
    {
        List<ForecastNumber> _items = new List<ForecastNumber>();

        public void Add(ForecastNumber item)
        {
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(ForecastNumber item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(ForecastNumber[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(ForecastNumber item)
        {
            return _items.Remove(item);
        }

        public IEnumerator<ForecastNumber> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }
}
