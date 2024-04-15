using System.ComponentModel.DataAnnotations;

namespace TimeTrackingSystem.Models
{
    public class Task
    {
        [Key]
        public int TaskID { get; set; }
        public string Title { get; set; }
        [DataType(DataType.Text)]
        public string? Description { get; set; }
        public DateTime StartAt { get; set; } = DateTime.Now;
        public DateTime DueAt { get; set; }

        //Foreign key
        public int AccountID { get; set; }

        //Relations
        public virtual Account? Account { get; set; }
    }
}
