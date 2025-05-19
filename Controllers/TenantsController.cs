using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly AssetivoContext _context;
    private readonly IMapper _mapper;

    public TenantsController(AssetivoContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TenantDto>>> GetTenants()
    {
        var tenants = await _context.Tenants.AsNoTracking().ToListAsync();
        return Ok(_mapper.Map<IEnumerable<TenantDto>>(tenants));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TenantDto>> GetTenant(Guid id)
    {
        var tenant = await _context.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        if (tenant == null) return NotFound();
        return Ok(_mapper.Map<TenantDto>(tenant));
    }

    [HttpPost]
    public async Task<ActionResult<TenantDto>> PostTenant(CreateTenantDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var tenant = _mapper.Map<Tenant>(dto);
        tenant.Id = Guid.NewGuid(); // Ensure ID is set

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<TenantDto>(tenant);
        return CreatedAtAction(nameof(GetTenant), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTenant(Guid id, UpdateTenantDto dto)
    {
        if (id != dto.Id) return BadRequest();

        var tenant = await _context.Tenants.FindAsync(id);
        if (tenant == null) return NotFound();

        _mapper.Map(dto, tenant); // update existing entity with dto values

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Tenants.Any(e => e.Id == id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTenant(Guid id)
    {
        var tenant = await _context.Tenants.FindAsync(id);
        if (tenant == null) return NotFound();

        _context.Tenants.Remove(tenant);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}