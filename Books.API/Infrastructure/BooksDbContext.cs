using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Books.API.Infrastructure
{
    public class BooksDbContext : IBooksDbContext
    {
        public ILiteDatabase Database { get; }

        public BooksDbContext(IOptions<LiteDbOptions> options)
        {
            Database = new LiteDatabase(options.Value.DatabaseLocation);
        }
    }
}
