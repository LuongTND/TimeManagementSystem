using System.ComponentModel.DataAnnotations;

namespace TimeTrackingSystem.Models
{
    public class Role
    {
        [Key]
        public int RoleID { get; set; }
        public string Name { get; set; }

        //Relations
        public virtual ICollection<Account>? Accounts { get; set; }
    }
}
