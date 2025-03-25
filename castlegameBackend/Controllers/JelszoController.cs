using castlegameBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace castlegameBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JelszoController : ControllerBase
    {
        [HttpPost("{loginName},{oldPassword},{newPassword}")]

        public async Task<IActionResult> JelszoMosositas(string loginName, string oldPassword, string newPassword)
        {
            try
            {
                using (var context = new CastlegameContext())
                {
                    User? user = context.Users.FirstOrDefault(f => f.LoginNev == loginName);
                    if (user != null)
                    {
                        if (Program.CreateSHA256(oldPassword) == user.Hash)
                        {
                            user.Hash = Program.CreateSHA256(newPassword);
                            context.Users.Update(user);
                            await context.SaveChangesAsync();
                            return Ok("A jelszó módosítása sikeresen megtörtént.");
                        }
                        else
                        {
                            return StatusCode(201, "Hibás a régi jelszó!");
                        }
                    }
                    else
                    {
                        return BadRequest("Nincs ilyen nevű felhasználó!");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{Email}")]
        public async Task<IActionResult> ElfelejtettJelszo(string Email)
        {
            using (var context = new CastlegameContext())
            {
                try
                {
                    var user = context.Users.FirstOrDefault(f => f.Email == Email);
                    if (user != null)
                    {
                        string jelszo = Program.GenerateSalt().Substring(0, 16);
                        user.Hash = Program.CreateSHA256(Program.CreateSHA256(jelszo + user.Salt));
                        context.Users.Update(user);
                        await context.SaveChangesAsync();
                        Program.SendEmail(user.Email, "Elfelejtett jelszó", "Az új jelszava: " + jelszo);
                        return Ok("E-mail küldése megtörtént.");
                    }
                    else
                    {
                        return StatusCode(210, "Nincs ilyen e-Mail cím!");
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(211, ex.Message);
                }
            }
        }


    }
}
