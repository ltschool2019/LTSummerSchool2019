using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using LTTimeRegistrator.Models;

namespace HelloAngularApp.Controllers
{
  [Route("api/users")]
  public class UsersController : Controller
  {
    ApplicationContext db;
    public UsersController(ApplicationContext context)
    {
      db = context;
      if (!db.Users.Any())
      {
        db.Users.Add(new User { Name = "Alexander", Surname = "Moskvin"});
        db.Users.Add(new User { Name = "Olga", Surname = "Kalatusha" });
        db.SaveChanges();
      }
    }
    [HttpGet]
    public IEnumerable<User> Get()
    {
      return db.Users.ToList();
    }

    [HttpGet("{id}")]
    public User Get(int id)
    {
      User product = db.Users.FirstOrDefault(x => x.ID == id);
      return product;
    }

    [HttpPost]
    public IActionResult Post([FromBody]User user)
    {
      if (ModelState.IsValid)
      {
        db.Users.Add(user);
        db.SaveChanges();
        return Ok(user);
      }
      return BadRequest(ModelState);
    }

    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody]User user)
    {
      if (ModelState.IsValid)
      {
        db.Update(user);
        db.SaveChanges();
        return Ok(user);
      }
      return BadRequest(ModelState);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
      User product = db.Users.FirstOrDefault(x => x.ID == id);
      if (product != null)
      {
        db.Users.Remove(product);
        db.SaveChanges();
      }
      return Ok(product);
    }
  }
}
