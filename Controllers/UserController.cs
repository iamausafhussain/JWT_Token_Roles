using JWT_Role.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JWT_Role.Controllers
{
    [Authorize(Roles = "admin,teacher")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private JwtroleContext _context;
        public UserController(JwtroleContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("getallusers")]
        public async Task<ActionResult<IEnumerable<UserInfo>>> GetAllUsers()
        {
            if (_context == null)
            {
                return NotFound("Database not found!");
            }
            return await _context.UserInfos.ToListAsync();
        }
        [Authorize(Roles = "admin")]
        [HttpPost("createuser")]
        public async Task<ActionResult<UserInfo>> CreateItem(UserInfo userInfo)
        {
            _context.UserInfos.Add(userInfo);
            {
                await _context.SaveChangesAsync();
            }
            return CreatedAtAction(nameof(GetAllUsers), new { userid = userInfo.Userid }, userInfo);
        }
        [Authorize(Roles = "admin")]
        [HttpPut("updateuser/{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserInfo userInfo)
        {
            if (id != userInfo.Userid)
            {
                return BadRequest($"No item found with id {id}");
            }
            _context.Entry(userInfo).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserAvailable(id))
                {
                    return NotFound("User Not Available, Not Found!");
                }
                else
                {
                    throw;
                }
            }
            return Ok();
        }
        private bool UserAvailable(int id)
        {
            var result = _context.UserInfos.Find(id);
            if (result.Userid == id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("deleteuser/{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.UserInfos.FindAsync(id);
            if (_context.UserInfos == null || item == null)
            {
                return NotFound($"Id {id} not found!");
            }
            _context.UserInfos.Remove(item);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
