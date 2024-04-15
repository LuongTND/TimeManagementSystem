using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTrackingSystem.Models
{
    public class Account
    {
        [Key]
        public int AccountID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        //Foreign key
        public int RoleID { get; set; }

        //Relations
        [ForeignKey(nameof(RoleID))]
        public virtual Role? Role { get; set; }
        public virtual ICollection<Task>? Tasks { get; set; }
        public virtual ICollection<Group>? LeadGroups { get; set; }
        public virtual ICollection<GroupMember>? JoinGroups { get; set; }

        public virtual Clock? Clock { get; set; }
    }
}
