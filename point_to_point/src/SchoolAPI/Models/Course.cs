using System;

namespace SchoolAPI.Models
{
    public class Course
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CourseNumber { get; set; }
        public DateTime StartTime { get; set; }
    }
}