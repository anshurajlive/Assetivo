using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetivoBackend.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PropertiesController : ControllerBase
    {
        private readonly AssetivoContext _context;
        private readonly IMapper _mapper;

        public PropertiesController(AssetivoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/properties
        [HttpGet]
        public async Task<IActionResult> GetProperties()
        {
            var properties = await _context.Properties.ToListAsync();
            var result = _mapper.Map<List<PropertySummaryDto>>(properties);
            return Ok(result);
        }

        // GET: api/properties/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProperty(Guid id)
        {
            var property = await _context.Properties
                .Include(p => p.Tenants)
                .Include(p => p.Documents)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
                return NotFound();

            var result = _mapper.Map<PropertyDetailDto>(property);
            return Ok(result);
        }

        // POST: api/properties
        [HttpPost]
        public async Task<IActionResult> CreateProperty([FromBody] PropertyCreateDto dto)
        {
            var property = _mapper.Map<Property>(dto);
            property.Id = Guid.NewGuid();

            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<PropertyDetailDto>(property);
            return CreatedAtAction(nameof(GetProperty), new { id = property.Id }, result);
        }

        // PUT: api/properties/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProperty(Guid id, [FromBody] PropertyUpdateDto dto)
        {
            var existing = await _context.Properties.FindAsync(id);
            if (existing == null)
                return NotFound();

            _mapper.Map(dto, existing);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/properties/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProperty(Guid id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return NotFound();

            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
