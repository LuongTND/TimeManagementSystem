using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTrackingSystem.Models
{
    public class Group
    {
        [Key]
        public int GroupID { get; set; }
        public string Name { get; set; }
        
        //Foreign key
        public int LeaderID { get; set; }

        //Relations
        [ForeignKey("LeaderID")]
        public virtual Account? Leader { get; set; }
        public virtual ICollection<Meeting>? Meeting { get; set; }
        public virtual ICollection<GroupMember>? Members { get; set; }
    }
}
