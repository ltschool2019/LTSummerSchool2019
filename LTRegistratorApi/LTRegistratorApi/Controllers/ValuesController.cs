using System;
using System.Collections.Generic;
using System.Linq;
using LTRegistrator.DAL;
using LTRegistrator.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LTRegistratorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly LTRegistratorDbContext _db;
        public ValuesController(LTRegistratorDbContext context)
        {
            _db = context;
            if (!_db.Values.Any())
            {
                _db.Values.Add(new Value() { Content = "Value1" });
                _db.Values.Add(new Value() { Content = "Value2" });
                _db.SaveChanges();
            }
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return _db.Values.Select(v => v.Content).ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(Guid id)
        {
            string value = _db.Values.SingleOrDefault(v => v.Id == id)?.Content;

            if (value == null)
            {
                return BadRequest();
            }

            return value;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
            _db.Values.Add(new Value() { Content = value });
            _db.SaveChanges();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] string content)
        {
            Value value = _db.Values.SingleOrDefault(v => v.Id == id);

            if (value != null)
            {
                value.Content = content;
                _db.SaveChanges();
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            Value value = _db.Values.SingleOrDefault(v => v.Id == id);
            if (value != null)
            {
                _db.Values.Remove(value);
                _db.SaveChanges();
            }
        }
    }
}
