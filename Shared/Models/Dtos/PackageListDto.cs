using System.Collections.Generic;

namespace WingetNexus.Shared.Models.Dtos
{
    public class PackageListDto
    {
        public IEnumerable<ApplicationDto>? Items { get; set; }
        public int ItemTotalCount { get; set; } = 0; // The total count of items before paging
    }
}