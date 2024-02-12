using CDN.Data;
using CDN.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserController> _logger;
        /*private readonly Uri _baseUri = new Uri("https://localhost:7083");*/
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }



        /// <summary>
        /// Retrieves a list of all users.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                var user = await _context.User.ToListAsync();
                return Ok(user);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(500, "Internal Server Error");
            }
        }


        /// <summary>
        /// Search by id
        /// </summary>
        [HttpGet("{id}")]
        /*[Route("Search")]*/
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }


        /// <summary>
        /// Insert
        /// </summary>
        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult<User>> AddUser([FromBody] User newUser)
        {
            _context.User.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserId }, newUser);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            try
            {
                var existingUser = await _context.User.FindAsync(id);

                if (existingUser == null)
                {
                    return NotFound();
                }

                // Ensure the ID cannot be modified
                updatedUser.UserId = existingUser.UserId;

                _context.Entry(existingUser).CurrentValues.SetValues(updatedUser);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /*/// <summary>
        /// Insert and Update
        /// </summary>
        [HttpPost("{id}")]
        public async Task<ActionResult<User>> InsertUpdateUser(int id, [FromBody] User updatedUser)
        {
            if (id != updatedUser.UserId)
            {
                return BadRequest("Invalid ID in the request body");
            }

            var existingUser = await _context.User.FindAsync(id);

            if (existingUser == null)
            {
                // User doesn't exist, so create a new one
                _context.User.Add(updatedUser);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUserById), new { id = updatedUser.UserId }, updatedUser);
            }
            else
            {
                // User exists, so update the existing one
                _context.Entry(existingUser).CurrentValues.SetValues(updatedUser);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }*/
    }
    
}
