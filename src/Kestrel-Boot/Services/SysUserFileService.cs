using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpBroker
{
    public class SysUserFileService : ISysService<UserFile>
    {
        private readonly IMongoCollection<UserFile> _books;

        public SysUserFileService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("vssedb"));
            var database = client.GetDatabase("vsse");
            _books = database.GetCollection<UserFile>("userfile");
        }

        public List<UserFile> Get()
        {
            return _books.Find(book => true).ToList();
        }

        public PageInfo<UserFile> Get(int pageIndex, int pageSize)
        {
            PageInfo<UserFile> page = new PageInfo<UserFile>();
            page.Total = _books.CountDocuments(book => true);
            page.Items = _books.Find(book => true).SortByDescending(t => t.CreatedAt).Skip(pageIndex * pageSize).Limit(pageSize).ToList();
            return page;
        }

        public UserFile Get(string id)
        {
            return _books.Find<UserFile>(book => book.Id == Guid.Parse(id)).FirstOrDefault();
        }

        public UserFile Create(UserFile book)
        {
            _books.InsertOne(book);
            return book;
        }

        public void Update(string id, UserFile bookIn)
        {
            _books.ReplaceOne(book => book.Id == Guid.Parse(id), bookIn);
        }

        public void Remove(UserFile bookIn)
        {
            _books.DeleteOne(book => book.Id == bookIn.Id);
        }

        public void Remove(string id)
        {
            _books.DeleteOne(book => book.Id == Guid.Parse(id));
        }
    }
}
