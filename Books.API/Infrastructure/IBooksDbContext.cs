using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace Books.API.Infrastructure
{
    public interface IBooksDbContext
    {
        ILiteDatabase Database { get; }
    }
}
