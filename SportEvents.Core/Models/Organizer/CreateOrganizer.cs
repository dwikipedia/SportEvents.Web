using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportEvents.Domain.Models.Organizer
{
    public class CreateOrganizer
    {
        [Required]
        public string OrganizerName { get; set; }

        [Required]
        public string ImageLocation { get; set; }
    }
}
