using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TesteApi.Data;
using TesteApi.Models;

namespace TesteApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly DataContext _context;

        public HomeController(DataContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetAllPersons() =>
            await _context.Persons.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetById(int id)
        {
            var person = await _context.Persons.FindAsync(id);

            if (person == null) return NotFound();

            return person;
        }

        [HttpPost]
        public async Task<ActionResult<Person>> RegisterPerson([FromBody] Person person)
        {
            if (_context.Persons.Any(p => p.Name.Equals(person.Name))) return BadRequest(person);

            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new {id = person.Id}, person);

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePerson(int id, Person person)
        {
            if (id != person.Id) return BadRequest();

            _context.Entry(person).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
                
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = await _context.Persons.FindAsync(id);

            if (person == null) return NotFound();

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonExists(int id) =>
            _context.Persons.Any(e => e.Id == id);

    }
}