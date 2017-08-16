using System.Collections.Generic;
using SchoolClient.Models;

namespace SchoolClient
{
    public interface IProxiedApiClient
    {
        [ServiceDiscovery(Endpoint = "/api/students", HttpVerb = "GET")]
        IEnumerable<Student> GetStudents();

        [ServiceDiscovery(Endpoint = "/api/courses", HttpVerb = "GET")]
        IEnumerable<Course> GetCourses();
    }
}