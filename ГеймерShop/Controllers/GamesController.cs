using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ГеймерShop.Data;
using ГеймерShop.Models;
using ГеймерShop.ViewModels;

namespace ГеймерShop.Controllers
{
    public class GamesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var genres = await _context.Genres.ToListAsync();
            genres.Add(new Genre { Id = 0, Name = "Любой жанр" });
            genres.Reverse();

            var plaingField = await _context.PlaingFields.ToListAsync();
            plaingField.Add(new PlaingField { Id = 0, Name = "Любая площадка" });
            plaingField.Reverse();

            var systemReq = await _context.SystemRequirements.ToListAsync();

            ViewData["Genres"] = new SelectList(genres, "Name", "Name", 0);
            ViewData["PlaingFields"] = new SelectList(plaingField, "Name", "Name", 0);
            
            ViewData["Price"] = new SelectList(new List<string>{ "Все цены", "До 30", "До 40", "До 50"}, 0);

            var gamesWithKey = await _context.Keys.Where(p => p.KeyStatusId == 1).Select(p => p.GameId).Distinct().ToListAsync();

            var applicationDbContext = _context.Games
                .Where(g => gamesWithKey.Contains(g.Id))
                .Include(g => g.Genre)
                .Include(g => g.Picture)
                .Include(g => g.PlaingField)
                .Include(g => g.SystemRequirements);

            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Filter([Bind("Genre, PlaingField, Price")] FilterViewModel model)
        {
            var gamesWithKey = await _context.Keys.Where(p => p.KeyStatusId == 1).Select(p => p.GameId).Distinct().ToListAsync();

            var games = await _context.Games
                .Where(g => gamesWithKey.Contains(g.Id))
                .Include(g => g.Genre)
                .Include(g => g.Picture)
                .Include(g => g.PlaingField)
                .Include(g => g.SystemRequirements)
                .ToListAsync();

            if (model.Price != "Все цены")
            {
                int limit = model.Price == "До 30" ? 30 : (model.Price == "До 40" ? 40 : 50);
                games = games.Where(g => g.Price < limit).ToList();
            }
            if (model.PlaingField != "Любая площадка")
            {
                games = games.Where(g => g.PlaingField.Name == model.PlaingField).ToList();
            }
            if (model.Genre != "Любой жанр")
            {
                games = games.Where(g => g.Genre.Name == model.Genre).ToList();
            }

            return PartialView("Assortment", games);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "Id", "Name");
            ViewData["PlaingFieldId"] = new SelectList(_context.Set<PlaingField>(), "Id", "Name");
            ViewData["OSs"] = await _context.SystemRequirements.Select(s => s.OS).Distinct().ToListAsync();
            ViewData["CPUs"] = await _context.SystemRequirements.Select(s => s.CPU).Distinct().ToListAsync();
            ViewData["GPUs"] = await _context.SystemRequirements.Select(s => s.GPU).Distinct().ToListAsync();
            ViewData["RAMs"] = await _context.SystemRequirements.Select(s => s.RAM).Distinct().ToListAsync();
            ViewData["Memory"] = await _context.SystemRequirements.Select(s => s.Memory).Distinct().ToListAsync();

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GameViewModel model)
        {
            if (ModelState.IsValid)
            {
                byte[] imageData = null;

                if (model.Image != null)
                {
                    using (var binaryReader = new BinaryReader(model.Image.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)model.Image.Length);
                    }
                }

                Picture picture = new Picture { Image = imageData };
                _context.Pictures.Add(picture);

                SystemRequirements systemReq = new SystemRequirements
                {
                    OS = model.OS,
                    CPU = model.CPU,
                    RAM = model.RAM,
                    GPU = model.GPU,
                    Memory = model.Memory
                };

                Game game = new Game
                {
                    Picture = picture,
                    SystemRequirements = systemReq,
                    Name = model.Name,
                    Price = model.Price,
                    GenreId = model.GenreId,
                    PlaingFieldId = model.PlaingFieldId
                };

                await _context.AddAsync(game);
                await _context.SystemRequirements.AddAsync(systemReq);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Management));
            }

            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "Id", "Name");
            ViewData["PlaingFieldId"] = new SelectList(_context.Set<PlaingField>(), "Id", "Name");
            ViewData["OSs"] = await _context.SystemRequirements.Select(s => s.OS).Distinct().ToListAsync();
            ViewData["CPUs"] = await _context.SystemRequirements.Select(s => s.CPU).Distinct().ToListAsync();
            ViewData["GPUs"] = await _context.SystemRequirements.Select(s => s.GPU).Distinct().ToListAsync();
            ViewData["RAMs"] = await _context.SystemRequirements.Select(s => s.RAM).Distinct().ToListAsync();
            ViewData["Memory"] = await _context.SystemRequirements.Select(s => s.Memory).Distinct().ToListAsync();

            return View(model);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Management()
        {
            var games = _context.Games
                .Include(g => g.Genre)
                .Include(g => g.Picture)
                .Include(g => g.PlaingField)
                .Include(g => g.SystemRequirements);

            return View(await games.ToListAsync());
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.SystemRequirements)
                .Where(g => g.Id == id)
                .FirstOrDefaultAsync();

            if (game == null)
            {
                return NotFound();
            }

            GameViewModel model = new GameViewModel
            {
                Id = game.Id,
                Name = game.Name,
                Price = game.Price,
                OS = game.SystemRequirements.OS,
                CPU = game.SystemRequirements.CPU,
                RAM = game.SystemRequirements.RAM,
                GPU = game.SystemRequirements.GPU,
                Memory = game.SystemRequirements.Memory,
                GenreId = game.GenreId
            };

            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "Id", "Name", model.GenreId);
            ViewData["PlaingFieldId"] = new SelectList(_context.Set<PlaingField>(), "Id", "Name", model.PlaingFieldId);
            ViewData["OSs"] = await _context.SystemRequirements.Select(s => s.OS).Distinct().ToListAsync();
            ViewData["CPUs"] = await _context.SystemRequirements.Select(s => s.CPU).Distinct().ToListAsync();
            ViewData["GPUs"] = await _context.SystemRequirements.Select(s => s.GPU).Distinct().ToListAsync();
            ViewData["RAMs"] = await _context.SystemRequirements.Select(s => s.RAM).Distinct().ToListAsync();
            ViewData["Memory"] = await _context.SystemRequirements.Select(s => s.Memory).Distinct().ToListAsync();
            
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, GameViewModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Game game = await _context.Games.Include(g => g.SystemRequirements).Where(g => g.Id == id).FirstOrDefaultAsync();

                if (game == null)
                {
                    return NotFound();
                }

                byte[] imageData = null;
                Picture picture = null;

                if (model.Image != null)
                {
                    using (var binaryReader = new BinaryReader(model.Image.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)model.Image.Length);
                    }

                    picture = new Picture { Image = imageData };
                    game.Picture = picture;
                }

                game.Name = model.Name;
                game.PlaingFieldId = model.PlaingFieldId;
                game.SystemRequirements.CPU = model.CPU;
                game.SystemRequirements.GPU = model.GPU;
                game.SystemRequirements.Memory = model.Memory;
                game.SystemRequirements.RAM = model.RAM;
                game.SystemRequirements.OS = model.OS;
                game.GenreId = model.GenreId;
                game.Price = model.Price;

                _context.Games.Update(game);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Management));
            }


            ViewData["GenreId"] = new SelectList(_context.Set<Genre>(), "Id", "Name", model.GenreId);
            ViewData["PlaingFieldId"] = new SelectList(_context.Set<PlaingField>(), "Id", "Name", model.PlaingFieldId);
            ViewData["OSs"] = await _context.SystemRequirements.Select(s => s.OS).Distinct().ToListAsync();
            ViewData["CPUs"] = await _context.SystemRequirements.Select(s => s.CPU).Distinct().ToListAsync();
            ViewData["GPUs"] = await _context.SystemRequirements.Select(s => s.GPU).Distinct().ToListAsync();
            ViewData["RAMs"] = await _context.SystemRequirements.Select(s => s.RAM).Distinct().ToListAsync();
            ViewData["Memory"] = await _context.SystemRequirements.Select(s => s.Memory).Distinct().ToListAsync();

            return View(model);
        }   

        [HttpPost]
        public async Task<IActionResult> Search(string searchName)
        {
            if (searchName == null)
            {
                searchName = "";
            }
            var gamesWithKey = await _context.Keys.Where(p => p.KeyStatusId == 1)
                .Select(p => p.GameId).Distinct().ToListAsync();

            var games = await _context.Games
                .Include(g => g.Picture)
                .Include(g => g.PlaingField)
                .Include(g => g.SystemRequirements)
                .Where(g => g.Name.Contains(searchName))
                .Where(g => gamesWithKey.Contains(g.Id))
                .ToListAsync();

            return PartialView("Assortment", games);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Genre)
                .Include(g => g.Picture)
                .Include(g => g.PlaingField)
                .Include(g => g.SystemRequirements)
                .Where(g => g.Id == id)
                .FirstOrDefaultAsync();

            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == id);
           

            if (game == null)
            {
                return NotFound();
            }

            var picture = await _context.Pictures.Where(p => p.Id == game.PictureId).FirstOrDefaultAsync();

            _context.Games.Remove(game);

            if (picture != null)
            {
                _context.Pictures.Remove(picture);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Management));
        }

    }
}
