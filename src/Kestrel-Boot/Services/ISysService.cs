using System;
using System.Collections.Generic;
using System.Text;

namespace HttpBroker
{

    public interface ISysService<T>
    {
        List<T> Get();

        PageInfo<T> Get(int pageIndex, int pageSize);

        T Get(string id);

        T Create(T book);

        void Update(string id, T bookIn);

        void Remove(T bookIn);

        void Remove(string id);
    }

    public class PageInfo<T>
    {
        public long Total { get; set; }
        public List<T> Items { get; set; }
    }

}
