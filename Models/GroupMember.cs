using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTrackingSystem.Models
{
    public class GroupMember
    {
        public int AccountID { get; set; }
        public int GroupID { get; set; }

        [ForeignKey(nameof(AccountID))]
        public virtual Account? Account { get; set; }
        [ForeignKey(nameof(GroupID))]
        public virtual Group? Group { get; set; }
    }
}
