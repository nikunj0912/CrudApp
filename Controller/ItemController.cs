using CrudApp;
using CrudApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ItemController : ControllerBase
{
    private readonly AppDbContext _context;

    public ItemController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _context.Items.ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id) => 
        await _context.Items.FindAsync(id) is Item item ? Ok(item) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Create(Item item)
    {
        _context.Items.Add(item);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Item item)
    {
        if (id != item.Id) return BadRequest();
        _context.Entry(item).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null) return NotFound();
        _context.Items.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
