using SportEvents.Core.Models.User;
using System.ComponentModel.DataAnnotations;

namespace SportEvents.Domain.Models.User
{
    public class UserGetByIdRequest : UserRequest
    {
        [Required]
        [Display(Name = "User ID")]
        public int Id { get; set; }
    }
}
