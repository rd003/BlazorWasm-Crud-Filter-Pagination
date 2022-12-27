using BlazorWasmCrud.Server.Data;
using BlazorWasmCrud.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorWasmCrud.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly DatabaseContext _ctx;
        public PersonController(DatabaseContext ctx)
        {
            _ctx = ctx; 
        }

        [HttpPost]
        public IActionResult AddUpdate(Person person)
        {
            var status = new Status();
            if (!ModelState.IsValid)
            {
                status.StatusCode = 0;
                status.Message = "Please pass the valid data";
                return Ok(status);
            }
            try
            {
                if (person.Id == 0)
                    _ctx.Person.Add(person);
                else
                    _ctx.Person.Update(person);
                _ctx.SaveChanges();
                status.StatusCode = 1;
                status.Message = "Saved successfully";

            }
            catch (Exception ex)
            {
                status.StatusCode = 0;
                status.Message = "Server error";
            }
            return Ok(status);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Status status = new();
            var person = _ctx.Person.Find(id);
            if(person is null)
            {
                status.StatusCode = 0;
                status.Message = "Person does not exist";
                //return Ok(person);
            }
            _ctx.Person.Remove(person);
            _ctx.SaveChanges();
            status.StatusCode = 1;
            status.Message = "Deleted successfully";
            return Ok(status);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var data = _ctx.Person.Find(id);
            return Ok(data);
        }

        [HttpGet]
        public ActionResult GetAll(string sTerm="",int pageNo=1)
        {
            sTerm = sTerm.ToLower();
            var data = (from person in _ctx.Person
                        where sTerm == null || person.Name.ToLower().StartsWith(sTerm)
                        select new Person
                        {
                            Name = person.Name,
                            Email = person.Email,
                            Id = person.Id
                        }
                        ).ToList();
            var totalRecords = data.Count;
            int pageSize = 3;
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            int skip = (pageNo - 1) * pageSize; 
            data = data.Skip(skip).Take(pageSize).ToList();
            var model = new PersonList
            {
                Persons = data,
                SearchTerm=sTerm,
                TotalPages=totalPages,
                PageNumber=pageNo
            };
  
            return Ok(model);
        }

    }
}
