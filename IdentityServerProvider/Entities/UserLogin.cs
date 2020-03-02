using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Galal.IDP.Entities
{
    [Table("UserLogins")]
    public class UserLogin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [MaxLength(50)]
        public string Id { get; set; }

        [MaxLength(50)]
        [Required]
        [ForeignKey("User")]
        public string SubjectId { get; set; }
        public virtual User Subject { get; set; }

        [Required]
        [MaxLength(250)]
        public string LoginProvider { get; set; }

        [Required]
        [MaxLength(250)]
        public string ProviderKey { get; set; }
    }
}
