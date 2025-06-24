namespace Applications.Responses
{
    public class Pagination<T> where T : class
    {
        public Pagination ( int pageNumber, int pageSize, int totalItems, IReadOnlyList<T> data )
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalItems = totalItems;
            Data = data;
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public IReadOnlyList<T> Data { get; set; }

         public static Pagination<T> Empty(int pageNumber, int pageSize)
        {
            return new Pagination<T>(pageNumber, pageSize, 0, new List<T>());
        }
    }

}
