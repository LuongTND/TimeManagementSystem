using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTrackingSystem.Models
{
    public class Clock
    {
        [Key]
        public int ClockId { get; set; }

        public int WorkingTime { get; set; }

        public int RestTime { get; set; }

        [ForeignKey("Clock")]
        public int AccountId { get; set; }

        public virtual Account? Account { get; set; }

    }
}
