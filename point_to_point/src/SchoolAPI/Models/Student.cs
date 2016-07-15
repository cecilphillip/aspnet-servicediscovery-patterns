namespace SchoolAPI.Models
{
    public class Student
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsFullTime { get; set; }
        public string Email { get; set; }
    }
}