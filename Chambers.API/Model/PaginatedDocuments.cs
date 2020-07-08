using System.Collections.Generic;

namespace Chambers.API.Model
{
    public class PaginatedDocuments
    {
        public int PageIndex { get; private set; }

        public int PageSize { get; private set; }

        public long Count { get; private set; }

        public IEnumerable<Document> Data { get; private set; }

        public PaginatedDocuments(int pageIndex, 
            int pageSize, 
            long count, 
            IEnumerable<Document> data)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.Count = count;
            this.Data = data;
        }
    }
}
