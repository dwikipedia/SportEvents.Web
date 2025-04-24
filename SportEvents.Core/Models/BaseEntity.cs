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
    }
}
