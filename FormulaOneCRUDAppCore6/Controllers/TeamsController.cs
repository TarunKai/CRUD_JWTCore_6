using FormulaOneCRUDAppCore6.Data;
using FormulaOneCRUDAppCore6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FormulaOneCRUDAppCore6.Controllers
{

    
	[Route("api/[controller]")]
	[ApiController]
	public class TeamsController : ControllerBase
	{
		private static AppDBContext _dbContext;
        public TeamsController(AppDBContext dBContext)
        {
			_dbContext = dBContext;
        }

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var teams = await _dbContext.Teams.ToListAsync();
			return Ok(teams);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> Get(int id)
        {
			var team= await _dbContext.Teams.FirstOrDefaultAsync(x=>x.ID==id);

			if (team == null)
				return BadRequest("Invalid ID");
			return Ok(team);
        }

		[HttpPost]
		[Route("PostRequest")]
		public async Task<IActionResult> Post(Team team)
        {
			await _dbContext.Teams.AddAsync(team);
			await _dbContext.SaveChangesAsync();
			return CreatedAtAction("Get", team.ID, team);
        }

		[HttpPatch]
		[Route("PatchRequest")]
		public async Task<IActionResult> Patch([FromBody] int id, string country)
        {
			var team = await _dbContext.Teams.FirstOrDefaultAsync(x=>x.ID == id);

			if(team == null)
				return NotFound("Invalid id");
			team.Country = country;
			await _dbContext.SaveChangesAsync();
			return NoContent();
        }

		[HttpDelete]
		[Route("DeleteRequest")]
		public async Task<IActionResult> Delete(int id)
        {
			var team=await _dbContext.Teams.FirstOrDefaultAsync(x=>x.ID==id);

            if (team == null)
            {
				return BadRequest("Invalid id");
            }
			 _dbContext.Teams.Remove(team);

			await _dbContext.SaveChangesAsync();
			return NoContent();
        }


	}
}
