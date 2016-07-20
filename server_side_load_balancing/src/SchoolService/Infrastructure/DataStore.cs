using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using SchoolService.Models;

namespace SchoolService.Infrastructure
{
    public class DataStore
    {
        public List<Student> Students { get; set; }
        public List<Course> Courses { get; set; }

        public DataStore()
        {
            LoadFakeData();
        }

        private void LoadFakeData()
        {
            Students = new Faker<Student>()
                 .RuleFor(s => s.ID, f => int.Parse(f.Random.Replace("#####")))
                 .RuleFor(s => s.FirstName, f => f.Name.FirstName())
                 .RuleFor(s => s.LastName, f => f.Name.LastName())
                 .RuleFor(s => s.Email, (f, s) => f.Internet.Email(s.FirstName, s.LastName))
                 .RuleFor(s => s.IsFullTime, f => f.Random.Bool())
                 .Generate(50).ToList();

            string[] courseTitles = { "Biology", "Accounting", "Algebra", "Astromomy", "Astrobiology", "Aviation", "Business Admistration", "Calculus", "Chemistry", "Computer Science", "Economics", "Literature", "Marketing", "Meteorology", "Oceanography", "Personal Finace", "Psychology", "Physics", "Software Engineering", "Spanish", "Statistics" };

            Courses = new Faker<Course>()
                        .RuleFor(c => c.ID, f => int.Parse(f.Random.Replace("#####")))
                        .RuleFor(c => c.CourseNumber, f => f.Random.Replace("DNM-####"))
                        .RuleFor(c => c.Title, f => f.PickRandom(courseTitles))
                        .RuleFor(c => c.StartTime, f => f.Date.Future())
                        .Generate(100).ToList();
        }
    }
}
