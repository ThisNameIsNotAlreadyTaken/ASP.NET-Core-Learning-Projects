using System.ComponentModel.DataAnnotations;

namespace First_App.Models
{
    public class OfficeAssignment
    {
        [Key]
        //[ForeignKey("Instructor")]
        public int InstructorId { get; set; }

        [StringLength(50)]
        [Display(Name = "Office Location")]
        public string Location { get; set; }

        public virtual Instructor Instructor { get; set; }
    }
}
