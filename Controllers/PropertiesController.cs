using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetivoBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly AssetivoContext _context;

        public PropertiesController(AssetivoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProperties()
        {
            var properties = await _context.Properties
                .Include(p => p.Tenants)
                .Include(p => p.Documents)
                .ToListAsync();
            return Ok(properties);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProperty(Guid id)
        {
            var property = await _context.Properties
                .Include(p => p.Tenants)
                .Include(p => p.Documents)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null) return NotFound();

            return Ok(property);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProperty([FromBody] Property property)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            property.Id = Guid.NewGuid();
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProperty), new { id = property.Id }, property);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProperty(Guid id, [FromBody] Property property)
        {
            if (id != property.Id) return BadRequest("ID mismatch");

            var existing = await _context.Properties.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Name = property.Name;
            existing.Address = property.Address;
            existing.Type = property.Type;
            existing.Size = property.Size;
            existing.Status = property.Status;
            existing.GoogleMapLocation = property.GoogleMapLocation;
            existing.CurrentMarketValue = property.CurrentMarketValue;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProperty(Guid id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null) return NotFound();

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}