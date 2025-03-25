using castlegameBackend.DTOs;
using castlegameBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace castlegameBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost("SaltRequest/{loginName}")]

        public async Task<IActionResult> SaltRequest(string loginName)
        {
            using (var cx = new CastlegameContext())
            {
                try
                {
                    User response = await cx.Users.FirstOrDefaultAsync(f => f.LoginNev == loginName);
                    return response == null ? BadRequest("Hiba") : Ok(response.Salt);
                }
                catch
                (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPost]

        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            using (var cx = new CastlegameContext())
            {
                try
                {
                    string Hash = Program.CreateSHA256(loginDTO.TmpHash);
                    User loggedUser = await cx.Users.FirstOrDefaultAsync(f => f.LoginNev == loginDTO.LoginName && f.Hash == Hash);
                    if (loggedUser != null && loggedUser.Active == 1) // Ha az aktív állapotot 1 jelenti
                    {
                        //Egyszerre csak egy gépről lehet dolgozni eleje
                        bool talalt = false;
                        int index = 0;
                        int elemSzam = Program.LoggedInUsers.Count;
                        while (!talalt && index < elemSzam)
                        {
                            if (Program.LoggedInUsers.ElementAt(index).Value.LoginNev.ToUpper() == loginDTO.LoginName.ToUpper())
                            {
                                lock (Program.LoggedInUsers)
                                {
                                    Program.LoggedInUsers.Remove(Program.LoggedInUsers.ElementAt(index).Key);
                                }
                                talalt = true;
                            }
                            index++;
                        }
                        //Egyszerre csak egy gépről lehet dolgozni vége
                        string token = Guid.NewGuid().ToString();
                        lock (Program.LoggedInUsers)
                        {
                            Program.LoggedInUsers.Add(token, loggedUser);
                        }
                        return Ok(new LoggedUser { Name = loggedUser.Name, Email = loggedUser.Email, Permission = loggedUser.PermissionId, ProfilePicturePath = loggedUser.ProfilePicturePath, Token = token });
                    }
                    else
                    {
                        return BadRequest("Hibás név vagy jelszó/inaktív felhasználó!");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(new LoggedUser { Permission = -1, Name = ex.Message, ProfilePicturePath = "", Email = "" });
                }
            }
        }


    }
}
