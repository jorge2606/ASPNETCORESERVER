using System.Collections.Generic;
using server.Models;

namespace server.Helpers
{
    public class PagedResult<T>
    {
        public List<User> List { get; set; }
        public long TotalRecords { get; set; }
    }
}