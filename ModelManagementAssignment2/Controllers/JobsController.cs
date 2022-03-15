﻿#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelManagementAssignment2.Data;
using ModelManagementAssignment2.Models;
using ModelManagementAssignment2.ViewModels;

namespace ModelManagementAssignment2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly ModelManagementDb _context;

        public JobsController(ModelManagementDb context)
        {
            _context = context;
        }

        // GET: api/Jobs/
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
        {
            return await _context.Jobs.ToListAsync();
        }

        // GET: api/GetJobsForModel/
        //Krav: Hente en liste med alle jobs for en angiven model – uden expenses.
        [HttpGet("JobsForModel/{modelid}")]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobsForModel(long modelid)
        {
            var model = await _context.Models.Where(x => x.ModelId == modelid).Include(j => j.Jobs).SingleOrDefaultAsync();

            var result = model.Jobs.ToList();

            return Ok(result.Adapt<GetJobViewModel>());
        }

        // GET: api/Jobs/
        //Krav: Hente en liste med alle jobs. Skal inkludere navn på modeller, som er sat på de enkelte jobs, men ikke expenses.
        [HttpGet("JobsAndModels")]
        public async Task<ActionResult<IEnumerable<GetJobViewModel>>> GetJobsAndModels()
        {
            var rawresult = await _context.Jobs.Include(m => m.Models).ToListAsync();

            return Ok(rawresult.Adapt<GetJobViewModel>());
        }

        // GET: api/Jobs/5
        //Krav: Hente job med den angivne JobId. Skal inkludere listen med alle expenses for jobbet.
        [HttpGet("{jobid}")]
        public async Task<ActionResult<Job>> GetJob(long jobid)
        {
            var job = await _context.Jobs.Where(x => x.JobId == jobid).SingleOrDefaultAsync();

            if (job == null)
            {
                return NotFound();
            }

            return job;
        }

        // PUT: api/Jobs/5
        [HttpPut("{jobid}")]
        public async Task<IActionResult> PutJob(long jobid, UpdateJobViewModel job)
        {
            if (jobid != job.JobId)
            {
                return BadRequest();
            }

            _context.Entry(job.Adapt<Job>()).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(jobid))
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

        // PUT: api/Jobs/5
        //Krav: Tilføj model til job. Bemærk at der godt kan være flere modeller på samme job.
        [HttpPatch("ModelToJob/{jobid}/{modelid}")]
        public async Task<IActionResult> PatchModelToJob(long jobid, long modelid)
        {
            var model = await _context.Models.Where(x => x.ModelId == modelid).Include(m => m.Jobs).SingleOrDefaultAsync();
            var job = await _context.Jobs.Where(x => x.JobId == jobid).Include(j => j.Models).SingleOrDefaultAsync();

            if (job.Models == null)
            {
                job.Models = new List<Model>();
            }
            if (!job.Models.Contains(model))
            {
                job.Models.Add(model);
                model.Jobs.Add(job);
                _context.Entry(job).State = EntityState.Modified;
                _context.Entry(model).State = EntityState.Modified;
            }
            else
            {
                return BadRequest();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(jobid))
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

        // PUT: api/Jobs/5
        //Krav: Slet model fra job
        [HttpPut("RemoveModelFromJob/{jobid}/{modelid}")]
        public async Task<IActionResult> PutRemoveModelFromJob(long jobid, long modelid)
        {
            var model = await _context.Models.Where(x => x.ModelId == modelid).Include(m => m.Jobs).SingleOrDefaultAsync();
            var job = await _context.Jobs.Where(x => x.JobId == jobid).Include(j => j.Models).SingleOrDefaultAsync();

            if (job.Models != null && job.Models.Contains(model))
            {
                job.Models.Remove(model);
                model.Jobs.Remove(job);
                _context.Entry(job).State = EntityState.Modified;
                _context.Entry(model).State = EntityState.Modified;
            }
            else
            {
                return BadRequest();
            }


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(jobid))
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

        // POST: api/Jobs
        //Krav: Opret nyt jobpost
        [HttpPost]
        public async Task<ActionResult<Job>> PostJob(CreateJobViewModel job)
        {
            _context.Jobs.Add(job.Adapt<Job>());
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJob", new { jobid = job.JobId }, job);
        }

        // DELETE: api/Jobs/5
        //Krav: Slet et job
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(long id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JobExists(long id)
        {
            return _context.Jobs.Any(e => e.JobId == id);
        }
    }
}
