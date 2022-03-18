#nullable disable
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelManagementAssignment2.Data;
using ModelManagementAssignment2.Models;
using ModelManagementAssignment2.ViewModels;

namespace ModelManagementAssignment2.Controllers
{
    //This class uses Mapster to map between ViewModels and Db Models: https://www.codeproject.com/Articles/1249355/Mapster-Your-Next-Level-Object-to-Object-Mapping-T
    [Route("api/[controller]")]
    [ApiController]
    public class ModelsController : ControllerBase
    {
        private readonly ModelManagementDb _context;

        public ModelsController(ModelManagementDb context)
        {
            _context = context;
        }

        // GET: api/Models
        //Krav: Hente en liste med alle modeller – uden data for deres jobs eller udgifter.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModelViewModel>>> GetModels()
        {
            var rawresult = await _context.Models.ToListAsync();
            var result = new List<ModelViewModel>();

            foreach (var model in rawresult)
            {
                result.Add(model.Adapt<ModelViewModel>());
            }
                
            return Ok(result);
        }

        // GET: api/Models/5
        //Krav: Hente model med den angivne ModelId inklusiv modellens jobs og udgifter.
        [HttpGet("{id}")]
        public async Task<ActionResult<Model>> GetModel(long id)
        {
            var model = await _context.Models.Where(m => m.ModelId == id).Include(j => j.Jobs).Include(e => e.Expenses).SingleOrDefaultAsync();

            if (model == null)
            {
                return NotFound();
            }

            return model;
        }

        // PATCH: api/Models/5
        //Krav: Opdatere en model – kun grunddata – ikke jobs og udgifter.
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchModel(long id, ModelViewModel model)
        {            
            if (id != model.ModelId)
            {
                return BadRequest();
            }

            _context.Entry(model.Adapt<Model>()).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModelExists(id))
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

        // POST: api/Models
        //Krav: Opret ny model – kun grunddata – ikke jobs og udgifter.
        [HttpPost]
        public async Task<ActionResult<ModelViewModel>> PostModel(ModelViewModel model)
        {
            var newModel = model.Adapt<Model>();
            _context.Models.Add(newModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetModel", new { id = model.ModelId }, model);
        }

        // DELETE: api/Models/5
        //Krav: Slette en model
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModel(long id)
        {
            var model = await _context.Models.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            _context.Models.Remove(model);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ModelExists(long id)
        {
            return _context.Models.Any(e => e.ModelId == id);
        }
    }
}
