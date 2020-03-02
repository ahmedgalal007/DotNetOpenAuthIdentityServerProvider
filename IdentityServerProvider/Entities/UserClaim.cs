using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Galal.IDP.Entities
{
    [Table("Claims")]
    public class UserClaim
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
        public string ClaimType { get; set; }

        [Required]
        [MaxLength(250)]
        public string ClaimValue { get; set; }

        public UserClaim(string claimType, string claimValue)
        {
            ClaimType = claimType;
            ClaimValue = claimValue;
        }
        public UserClaim(string claimType, string claimValue, string subjectId)
        {
            ClaimType = claimType;
            ClaimValue = claimValue;
            SubjectId = subjectId;
        }

            public UserClaim()
        {

        }
    }
}
