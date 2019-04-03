using System;
using System.Collections.Generic;
using System.Text;

namespace HttpBroker
{
    public class UserFile
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string OriginalName { get; set; }

        public long Length { get; set; }

        public string Url { get; set; }

        public string Ext { get; set; }

        public int FileType { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
