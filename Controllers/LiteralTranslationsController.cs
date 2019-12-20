using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE.Example.Data;
using BE.Example.Models;
using BE.ExampleAPI.ModelViews;

namespace BE.ExampleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LiteralTranslationsController : ControllerBase
    {
        private readonly ExampleDBContext _context;

        public LiteralTranslationsController(ExampleDBContext context)
        {
            _context = context;
        }
        
        //C.2
        [HttpGet("{moduleId}/{languageId}/{countryId}")]
        public async Task<ActionResult<IEnumerable<LiteralTranslation>>> TranslationsByModuleAndCulture(int moduleId, int languageId, int countryId)
        {
//#if DEBUG
            //Solo para testing
            moduleId = 1;
            countryId = 1;
            languageId = 1;
//#endif

            var literalsTransByModuleAndCulture =
                    _context
                        .LiteralTranslations
                        .Include(x => x.Literal)
                            .ThenInclude(x => x.Module)
                        .Where(x => x.CountryId == countryId && x.LanguageId == languageId && x.Literal.ModuleId == moduleId);

            //TODO: Se debe hacer 'Union' con la tabla Literals para obtener todos los literales no traducidos

            return await literalsTransByModuleAndCulture.ToListAsync();
        }

        
        //C.3
        [HttpGet]
        [Route("Status")]
        public async Task<ActionResult<IEnumerable<Status>>> Status()
        {
            var status =
                    _context
                        .LiteralTranslations
                            .Include(x => x.Literal)
                            .Include(x => x.Language)
                            .Include(x => x.Country)
                            .GroupBy(x => new { x.Language, x.Country })
                            .Select(g => new Status
                            {
                                Language = g.Key.Language.Name,
                                Country = g.Key.Country.Name,
                                Translated = g.Count(),
                                InReview = _context.LiteralTranslations.Where(x => x.InReview == true).Count()
                            });

            return await status.ToListAsync();
        }

        //C.4
        [HttpGet]
        [Route("InReview")]
        public async Task<ActionResult<IEnumerable<InReview>>> InReview()
        {
            var inReview =
                    _context
                        .LiteralTranslations
                            .Include(x => x.Literal)
                            .Include(x => x.Language)
                            .Include(x => x.Country)
                            .Where(x => x.InReview == true)
                            .Select(x => new InReview
                            {
                                Language = x.Language.Name,
                                Country = x.Country == null ? "General" : x.Country.Name,
                                Literal = x.Literal.Code
                            });

            return await inReview.ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LiteralTranslation>>> GetLiteralTranslations()
        {
            return await _context.LiteralTranslations.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LiteralTranslation>> GetLiteralTranslation(int id)
        {
            var literalTranslation = await _context.LiteralTranslations.FindAsync(id);

            if (literalTranslation == null)
            {
                return NotFound();
            }

            return literalTranslation;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutLiteralTranslation(int id, LiteralTranslation literalTranslation)
        {
            if (id != literalTranslation.LiteralTranslationId)
            {
                return BadRequest();
            }

            _context.Entry(literalTranslation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LiteralTranslationExists(id))
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

        // POST: api/LiteralTranslations
        [HttpPost]
        public async Task<ActionResult<LiteralTranslation>> PostLiteralTranslation(LiteralTranslation literalTranslation)
        {
            _context.LiteralTranslations.Add(literalTranslation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLiteralTranslation", new { id = literalTranslation.LiteralTranslationId }, literalTranslation);
        }

        // DELETE: api/LiteralTranslations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<LiteralTranslation>> DeleteLiteralTranslation(int id)
        {
            var literalTranslation = await _context.LiteralTranslations.FindAsync(id);
            if (literalTranslation == null)
            {
                return NotFound();
            }

            _context.LiteralTranslations.Remove(literalTranslation);
            await _context.SaveChangesAsync();

            return literalTranslation;
        }

        private bool LiteralTranslationExists(int id)
        {
            return _context.LiteralTranslations.Any(e => e.LiteralTranslationId == id);
        }
    }
}
