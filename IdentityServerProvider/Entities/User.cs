using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Galal.IDP.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        [MaxLength(50)]       
        public string SubjectId { get; set; }
    
        [MaxLength(100)]
        [Required]
        public string Username { get; set; }

        [MaxLength(100)]
        public string Password { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public virtual ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();

        public virtual ICollection<UserLogin> Logins { get; set; } = new List<UserLogin>();


    }
}
