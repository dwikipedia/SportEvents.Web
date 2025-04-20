using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
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
        public int Page { get; set; }
        public int PerPage { get; set; }
    }
}
