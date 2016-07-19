using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolService.Models
{
    public class Course
    {
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public string CourseNumber { get; set; }
        public DateTime StartTime { get; set; }
    }
}