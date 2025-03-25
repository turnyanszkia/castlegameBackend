using castlegameBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using castlegameBackend.DTOs;

namespace castlegameBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JatekosokController : ControllerBase
    {
        // Ranglista lekérdezése (GET) – Csak admin jogú felhasználóknak (permissionId == 9)
        [HttpGet("{token}")]
        public async Task<IActionResult> GetAll(string token)
        {
            if (Program.LoggedInUsers.ContainsKey(token) && Program.LoggedInUsers[token].PermissionId == 9)
            {
                try
                {
                    using (var cx = new CastlegameContext())
                    {
                        var ranglista = await cx.Jatekosoks.ToListAsync();
                        return Ok(ranglista);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            else
            {
                return BadRequest("Nincs jogod Hozzá!");
            }
        }

        // Ranglista lekérdezése (GET) a játékosok adataival (Helyezés, Név, Pontszám)
        [HttpGet("ranglista/{token}")]
        [HttpGet("ranglista")]
public async Task<IActionResult> GetRanglista()
{
    try
    {
        using (var cx = new CastlegameContext())
        {
            var ranglista = await cx.Jatekosoks
                .OrderBy(j => j.Helyezes)
                .Select(j => new
                {
                    Helyezes = j.Helyezes,
                    Nev = j.Nev,
                    Pontszam = j.Pont
                })
                .ToListAsync();

            if (ranglista == null || !ranglista.Any())
            {
                return NotFound("A ranglista üres vagy nem található.");
            }

            return Ok(ranglista);
        }
    }
    catch (Exception ex)
    {
        return BadRequest($"Hiba történt: {ex.Message}");
    }
}


        // Játékos adatainak lekérése ID alapján (GET)
        [HttpGet("{id},{token}")]
        public async Task<IActionResult> GetById(int id, string token)
        {
            if (Program.LoggedInUsers.ContainsKey(token))
            {
                try
                {
                    using (var cx = new CastlegameContext())
                    {
                        var jatekos = await cx.Jatekosoks.FirstOrDefaultAsync(j => j.Id == id);
                        if (jatekos == null)
                        {
                            return NotFound("Játékos nem található!");
                        }
                        return Ok(jatekos);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            else
            {
                return BadRequest("Nincs jogod Hozzá!");
            }
        }

        // Új játékos felvétele (POST)
        [HttpPost("{token}")]
        public IActionResult Post(string token, [FromBody] Jatekosok jatekos)
        {
            if (Program.LoggedInUsers.ContainsKey(token) && Program.LoggedInUsers[token].PermissionId == 9)
            {
                try
                {
                    using (var cx = new CastlegameContext())
                    {
                        cx.Jatekosoks.Add(jatekos);
                        cx.SaveChanges();
                        return Ok("Új játékos adatainak rögzítése sikeres!");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            else
            {
                return BadRequest("Nincs jogod Hozzá!");
            }
        }

        // Játékos adatainak módosítása (PUT)
        [HttpPut("{token}")]
        public IActionResult Put(string token, [FromBody] Jatekosok jatekos)
        {
            if (Program.LoggedInUsers.ContainsKey(token) && Program.LoggedInUsers[token].PermissionId == 9)
            {
                try
                {
                    using (var cx = new CastlegameContext())
                    {
                        cx.Jatekosoks.Update(jatekos);
                        cx.SaveChanges();
                        return Ok("A játékos adatainak módosítása sikeres!");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            else
            {
                return BadRequest("Nincs jogod Hozzá!");
            }
        }

        // Játékos törlése (DELETE)
        [HttpDelete("{token},{id}")]
        public IActionResult Delete(string token, int id)
        {
            if (Program.LoggedInUsers.ContainsKey(token) && Program.LoggedInUsers[token].PermissionId == 9)
            {
                try
                {
                    using (var cx = new CastlegameContext())
                    {
                        var jatekos = new Jatekosok { Id = id };
                        cx.Jatekosoks.Remove(jatekos);
                        cx.SaveChanges();
                        return Ok("A játékos adatainak törlése sikeres!");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.InnerException?.Message ?? ex.Message);
                }
            }
            else
            {
                return BadRequest("Nincs jogod Hozzá!");
            }
        }
    }
}
