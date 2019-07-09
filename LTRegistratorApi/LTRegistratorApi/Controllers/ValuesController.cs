using System.Collections.Generic;
using System.Linq;
using LTRegistratorApi.Model;
using LTTimeRegistrator.Models;
using Microsoft.AspNetCore.Mvc;

namespace LTRegistratorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        ApplicationContext db;
        public ValuesController(ApplicationContext context)
        {
            db = context;
            if (!db.Values.Any())
            {
                db.Values.Add(new Value() { Content = "Value1" });
                db.Values.Add(new Value() { Content = "Value2" });
                db.SaveChanges();
            }
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return this.db.Values.Select(v => v.Content).ToList();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            string value = this.db.Values.SingleOrDefault(V => V.ID == id)?.Content;

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
            this.db.Values.Add(new Value() { Content = value });
            this.db.SaveChanges();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string content)
        {
            Value value = this.db.Values.SingleOrDefault(V => V.ID == id);

            if (value != null)
            {
                value.Content = content;
                this.db.SaveChanges();
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            Value value = this.db.Values.SingleOrDefault(V => V.ID == id);
            if (value != null)
            {
                this.db.Values.Remove(value);
                this.db.SaveChanges();
            }
        }
    }
}
