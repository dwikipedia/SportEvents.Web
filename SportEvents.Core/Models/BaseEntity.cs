using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SportEvents.Core.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
    }

    public class BaseEntityPaging
    {
        public int Page { get; set; } = 50;
        public int PerPage { get; set; } = 10;
        public string SearchValue { get; set; }
        public string SortDirection { get; set; }
        public string SortColumn { get; set; }
    }

    public class PagedResponse<T>
    {
        [JsonPropertyName("recordsTotal")]
        public int RecordsTotal { get; set; }

        [JsonPropertyName("recordsFiltered")]
        public int RecordsFiltered { get; set; }

        [JsonPropertyName("data")]
        public List<T> Data { get; set; } = new();

        [JsonPropertyName("meta")]
        public MetaResponse Meta { get; set; }
    }

    public class MetaResponse
    {
        public Pagination Pagination { get; set; }
    }

    public class Pagination
    {
        public int Total { get; set; }
        public int Count { get; set; }

        [JsonPropertyName("per_page")]
        public int PerPage { get; set; }

        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        public Links Links { get; set; }

    }

    public class Links
    {
        public string Previous { get; set; }
        public string Next { get; set; }
    }
}
