using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using snapcrateBackend.Auth;
using snapcrateBackend.Model;

namespace snapcrateBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SharedFolderController : ControllerBase
    {
        private readonly SnapCrateDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SharedFolderController(SnapCrateDbContext context,UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/SharedFolder
        [Route("ByUser/{userName}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SharedFolders>>> GetSharedFolders(string userName)
        {
          if (_context.SharedFolders == null)
          {
              return NotFound();
          }
            var sharedFolders = await _context.SharedFolders.
                Include(d => d.User)
                .Include(d => d.Folder)
                .Where(d => d.User.UserName == userName)
                .ToListAsync();
            if (sharedFolders == null)
            {
                return NotFound();
            }

            return sharedFolders;
        }

        // GET: api/SharedFolder/5
        [Route("ByFolder/{id}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SharedFolders>>> GetSharedFolders(int id)
        {
          if (_context.SharedFolders == null)
          {
              return NotFound();
          }
            var sharedFolders = await _context.SharedFolders.
                Include(d => d.User)
                .Include(d => d.Folder)
                .Where(d => d.Folder.Id == id)
                .ToListAsync();

            if (sharedFolders == null)
            {
                return NotFound();
            }

            return sharedFolders;
        }

        // PUT: api/SharedFolder/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSharedFolders(int id, bool enableEditing)
        {
            var sharedFolders =await _context.SharedFolders.FindAsync(id);
            if(sharedFolders == null)
            {
                return NotFound();
            }
            sharedFolders.EnableEditing = enableEditing;
            _context.Entry(sharedFolders).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SharedFoldersExists(id))
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

        // POST: api/SharedFolder
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SharedFolders>> PostSharedFolders(SharedFolderRequest sharedFolders)
        {
            SharedFolders sharedFolderData = new SharedFolders();
            try
            {
                if (_context.SharedFolders == null)
                {
                    return Problem("Entity set 'SnapCrateDbContext.SharedFolders'  is null.");
                }
                sharedFolderData.Folder = await _context.FolderModel.FindAsync(sharedFolders.FolderId);
                sharedFolderData.User = await _userManager.FindByNameAsync(sharedFolders.UserName);
                _context.SharedFolders.Add(sharedFolderData);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetSharedFolders", new { id = sharedFolderData.Id }, sharedFolderData);
            }
            catch
            {
                return Ok(new Response { Status = "Error", Message = "Shared Folders creation failed! Please check user details and try again." });
            }
        }

        // DELETE: api/SharedFolder/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSharedFolders(int id)
        {
            if (_context.SharedFolders == null)
            {
                return NotFound();
            }
            var sharedFolders = await _context.SharedFolders.FindAsync(id);
            if (sharedFolders == null)
            {
                return NotFound();
            }

            _context.SharedFolders.Remove(sharedFolders);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SharedFoldersExists(int id)
        {
            return (_context.SharedFolders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
