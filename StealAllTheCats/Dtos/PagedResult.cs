namespace StealAllTheCats.Dtos
{
    /// <summary>
    /// Represents a paginated result set.
    /// </summary>
    public class PagedResult<T>
    {
        /// <summary>
        /// The list of items for the current page.
        /// </summary>
        public List<T> Items { get; set; } = new();
        /// <summary>
        /// The current page number.
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// The number of items per page.
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// The total count of items across all pages.
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// The total number of pages.
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
