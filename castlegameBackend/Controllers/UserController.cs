using castlegameBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using castlegameBackend.DTOs;

namespace castlegameBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("{token}")]
        public async Task<IActionResult> GetFull(string token)
        {
            if (Program.LoggedInUsers.ContainsKey(token) && Program.LoggedInUsers[token].PermissionId == 9)
            {
                try
                {
                    using (var cx = new CastlegameContext())
                    {
                        return Ok(await cx.Users.ToListAsync());
                    }
                }
                catch (Exception ex) 
                {
                    return BadRequest(ex.InnerException.Message);
                }
            }
            else 
            {
                return BadRequest("Nincs jogod Hozzá!");
            }
        }
        [HttpGet("/EmailName/{token}")]
        public async Task<IActionResult> GetEmailName(string token)
        {
            if (Program.LoggedInUsers.ContainsKey(token) && Program.LoggedInUsers[token].PermissionId == 9)
            {
                try
                {
                    using (var cx = new CastlegameContext())
                    {
                        return Ok(await cx.Users.Select(f => new EmailNameDTO { Email = f.Email, Name = f.Name }).ToListAsync());
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.InnerException.Message);
                }
            }
            else
            {
                return BadRequest("Nincs jogod Hozzá!");
            }
        }
        [HttpGet("{id},{token}")]
        public async Task<IActionResult> GetId(int id, string token)
        {
            if (Program.LoggedInUsers.ContainsKey(token) && Program.LoggedInUsers[token].PermissionId == 9)
            {
                try
                {
                    using (var cx = new CastlegameContext())
                    {
                        return Ok(await cx.Users.FirstOrDefaultAsync(f => f.Id == id));
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.InnerException.Message);
                }
            }
            else
            {
                return BadRequest("Nincs jogod Hozzá!");
            }
        }
        [HttpPost("{token}")]

        public IActionResult Post(string token, User user)
        {
            if (Program.LoggedInUsers.ContainsKey(token) && Program.LoggedInUsers[token].PermissionId == 9)
            {
                try
                {
                    using (var cx = new CastlegameContext())
                    {
                        cx.Users.Add(user);
                        cx.SaveChanges();
                        return Ok("Új felhasznaaló adatai rögzítve");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.InnerException.Message);
                }
            }
            else
            {
                return BadRequest("Nincs jogod Hozzá!");
            }
        }
        [HttpPut("{token}")]
        public IActionResult Put(string token, User user)
        {
            if (Program.LoggedInUsers.ContainsKey(token) && Program.LoggedInUsers[token].PermissionId == 9)
            {
                try
                {
                    using (var cx = new CastlegameContext())
                    {
                        cx.Users.Update(user);
                        cx.SaveChanges();
                        return Ok("Új felhasznaaló adatai rögzítve");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.InnerException.Message);
                }
            }
            else
            {
                return BadRequest("Nincs jogod Hozzá!");
            }
        }
        [HttpDelete("{token},{id}")]
        public IActionResult Delete(string token, int id)
        {
            if (Program.LoggedInUsers.ContainsKey(token) && Program.LoggedInUsers[token].PermissionId == 9)
            {
                try
                {
                    using (var cx = new CastlegameContext())
                    {
                        cx.Users.Remove(new User { Id=id});
                        cx.SaveChanges();
                        return Ok("A felhasznaaló adatai törölve");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.InnerException.Message);
                }
            }
            else
            {
                return BadRequest("Nincs jogod Hozzá!");
            }
        }
    }
}
