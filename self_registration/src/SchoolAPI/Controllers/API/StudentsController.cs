using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using SchoolAPI.Filter;
using SchoolAPI.Infrastructure;
using SchoolAPI.Models;

namespace SchoolAPI.Controllers.API
{
    [Route("api/[controller]")]
    public class StudentsController : Controller
    {
        private readonly DataStore _dataStore;

        public StudentsController(DataStore dataStore)
        {
            _dataStore = dataStore;
        }

        [HttpGet]
        public IActionResult Get()
        {
            if (_dataStore.Students != null)
                return Ok(_dataStore.Students);

            return NotFound();
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var student = _dataStore.Students.SingleOrDefault(c => c.ID == id);
            if (student != null)
            {
                return Ok(student);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateModel]
        public IActionResult Post([FromBody]Student student)
        {
            _dataStore.Students.Add(student);
            return Created(Request.GetDisplayUrl() + "/" + student.ID, student);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Student student)
        {
            var exisitingStudent = _dataStore.Students.SingleOrDefault(c => c.ID == id);

            if (exisitingStudent == null) return NotFound();

            _dataStore.Students.Remove(exisitingStudent);
            _dataStore.Students.Add(student);

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var exisitingStudent = _dataStore.Students.SingleOrDefault(c => c.ID == id);

            if (exisitingStudent == null) return NotFound();

            _dataStore.Students.Remove(exisitingStudent);
            return Ok();
        }
    }
}
