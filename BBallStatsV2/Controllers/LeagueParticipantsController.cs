//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using BBallStats.Data;
//using BBallStatsV2.Data.Entities;

//namespace BBallStatsV2.Controllers
//{
//    // TODO: check / manage leagueId
//    [Route("api/fantasy/leagues/{leagueId}/participants")]
//    [ApiController]
//    public class LeagueParticipantsController : ControllerBase
//    {
//        private readonly ForumDbContext _context;

//        public LeagueParticipantsController(ForumDbContext context)
//        {
//            _context = context;
//        }

//        // GET: api/LeagueParticipants
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<LeagueParticipant>>> GetLeagueParticipants(int leagueId)
//        {
//            return await _context.LeagueParticipants
//                .Where(x => x.LeagueId == leagueId)
//                .ToListAsync();
//        }

//        // GET: api/LeagueParticipants/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<LeagueParticipant>> GetLeagueParticipant(int leagueId, int id)
//        {
//            var leagueParticipant = await _context.LeagueParticipants
//                .Include(x => x.Team)
//                .FirstOrDefaultAsync(x => x.Id == id && x.LeagueId == leagueId);

//            if (leagueParticipant == null)
//            {
//                return NotFound();
//            }

//            return leagueParticipant;
//        }

//        // PUT: api/LeagueParticipants/5
//        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//        [HttpPut("{id}")]
//        public async Task<IActionResult> PutLeagueParticipant(int id, LeagueParticipant leagueParticipant)
//        {
//            var league = await _context.Leagues
//                .FirstOrDefaultAsync(x => x.Id == leagueParticipant.LeagueId);
//            if (league == null)
//            {
//                return NotFound("League not found");
//            }

//            if (id != leagueParticipant.Id)
//            {
//                return BadRequest();
//            }

//            _context.Entry(leagueParticipant).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!LeagueParticipantExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }

//        // POST: api/LeagueParticipants
//        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//        [HttpPost]
//        public async Task<ActionResult<LeagueParticipant>> PostLeagueParticipant(LeagueParticipant leagueParticipant)
//        {
//            var league = await _context.Leagues
//                .FirstOrDefaultAsync(x => x.Id == leagueParticipant.LeagueId);
//            if (league == null)
//            {
//                return NotFound("League not found");
//            }

//            _context.LeagueParticipants.Add(leagueParticipant);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction("GetLeagueParticipant", new { id = leagueParticipant.Id }, leagueParticipant);
//        }

//        // DELETE: api/LeagueParticipants/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteLeagueParticipant(int id)
//        {
//            var leagueParticipant = await _context.LeagueParticipants.FindAsync(id);
//            if (leagueParticipant == null)
//            {
//                return NotFound();
//            }

//            var league = await _context.Leagues
//                .FirstOrDefaultAsync(x => x.Id == leagueParticipant.LeagueId);
//            if (league == null)
//            {
//                return NotFound("League not found");
//            }

//            _context.LeagueParticipants.Remove(leagueParticipant);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }

//        private bool LeagueParticipantExists(int id)
//        {
//            return _context.LeagueParticipants.Any(e => e.Id == id);
//        }
//    }
//}
