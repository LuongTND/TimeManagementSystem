using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTrackingSystem.Models
{
    public class Meeting
    {
        [Key]
        public Guid MeetingCode { get; set; }
        public DateTime OpenAt { get; set; } = DateTime.Now;
        public DateTime CloseAt { get; set; }

        //Foreign key
        public int GroupID { get; set; }

        //Relations
        [ForeignKey(nameof(GroupID))]
        public virtual Group? Group { get; set; }
    }
}
