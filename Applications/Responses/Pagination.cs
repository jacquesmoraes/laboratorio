namespace Applications.Responses
{
    public class Pagination<T> ( int pageNumber,
        int pageSize,
        int totalItems,
        IReadOnlyList<T> data ) where T : class
    {
        public int PageNumber { get; set; } = pageNumber;
        public int PageSize { get; set; } = pageSize;
        public int TotalItems { get; set; } = totalItems;
        public IReadOnlyList<T> Data { get; set; } = data;
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
        public static Pagination<T> Empty(int pageNumber, int pageSize)
        {
            return new Pagination<T>(pageNumber, pageSize, 0, new List<T>());
        }
    }

}
