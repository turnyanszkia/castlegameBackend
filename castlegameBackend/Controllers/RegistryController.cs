using castlegameBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace castlegameBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistryController : ControllerBase
    {
        [HttpPost]

        public async Task<IActionResult> Registry(User user)
        {
            using (var cx = new CastlegameContext())
            {
                try
                {
                    if (cx.Users.FirstOrDefault(f => f.LoginNev == user.LoginNev) != null)
                    {
                        return Ok("Már létezik ilyen felhasználónév!");
                    }
                    if (cx.Users.FirstOrDefault(f => f.Email == user.Email) != null)
                    {
                        return Ok("Ezzel az e-mail címmel már regisztráltak!");
                    }
                    user.PermissionId = 1;
                    user.Active = 0;
                    user.Hash = Program.CreateSHA256(user.Hash);
                    await cx.Users.AddAsync(user);
                    await cx.SaveChangesAsync();

                    Program.SendEmail(user.Email, "Regisztráció", $"http://localhost:5000/api/Registry?felhasznaloNev={user.LoginNev}&email={user.Email}");

                    return Ok("Sikeres regisztráció. Fejezze be a regisztrációját az e-mail címére küldött link segítségével!");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpGet]

        public async Task<IActionResult> EndOfTheRegistry(string felhasznaloNev, string email)
        {
            using (var cx = new CastlegameContext())
            {
                User user = await cx.Users.FirstOrDefaultAsync(f => f.LoginNev == felhasznaloNev && f.Email == email);
                if (user == null)
                {
                    return Ok("Sikertelen a regisztráció befejezése!");
                }
                else
                {
                    user.Active = 1;
                    cx.Users.Update(user);
                    await cx.SaveChangesAsync();
                    return Ok("A regisztráció befejezése sikeresen megtörtént.");
                }
            }
            return null;
        }


    }
}
