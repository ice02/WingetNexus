using System.Collections.Generic;

namespace WingetNexus.Shared.Models.Dtos
{
    public class DataListDto<T> where T : class
    {
        public IEnumerable<T>? Items { get; set; }
        public int ItemTotalCount { get; set; } = 0; // The total count of items before paging
        public int Page { get; set; } = 1; // The current page number
        public int PageSize { get; set; } = 10; // The number of items per page
        public string? Filter { get; set; } // The filter applied to the items
        public string? SortBy { get; set; } // The field by which the items are sorted
        public bool IsAscending { get; set; } = true; // Indicates if the sorting is ascending or descending
    }
}