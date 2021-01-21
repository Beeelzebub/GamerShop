using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            genres.Add(new Genre { Id = 0, Name = "Любой" });
            genres.Reverse();

            var plaingField = await _context.PlaingFields.ToListAsync();
            plaingField.Add(new PlaingField { Id = 0, Name = "Любая" });
            plaingField.Reverse();

            var systemReq = await _context.SystemRequirements.ToListAsync();

            var OSs = systemReq.Select(s => s.OS).Distinct().ToList();
            OSs.Add("Любая");
            OSs.Reverse();

            var CPUs = systemReq.Select(s => s.CPU).Distinct().ToList();
            OSs.Add("Любой");
            CPUs.Reverse();

            var RAMs = systemReq.Select(s => s.RAM).Distinct().ToList();
            RAMs.Add("Любая");
            RAMs.Reverse();

            var GPUs = systemReq.Select(s => s.GPU).Distinct().ToList();
            GPUs.Add("Любая");
            GPUs.Reverse();

            ViewData["Genres"] = new SelectList(genres, "Id", "Name", 0);
            ViewData["PlaingFields"] = new SelectList(plaingField, "Id", "Name", 0);
            ViewData["OSs"] = new SelectList(OSs, 0);
            ViewData["CPUs"] = new SelectList(CPUs, 0);
            ViewData["RAMs"] = new SelectList(RAMs, 0);
            ViewData["GPUs"] = new SelectList(GPUs, 0);

            var applicationDbContext = _context.Games.Include(g => g.Genre)
                .Include(g => g.Picture)
                .Include(g => g.PlaingField)
                .Include(g => g.SystemRequirements);

            return View(await applicationDbContext.ToListAsync());
        }

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

        public async Task<IActionResult> Management()
        {
            var games = _context.Games
                .Include(g => g.Genre)
                .Include(g => g.Picture)
                .Include(g => g.PlaingField)
                .Include(g => g.SystemRequirements);

            return View(await games.ToListAsync());
        }

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

                return RedirectToAction(nameof(Index));
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
            var games = await _context.Games
                .Include(g => g.Picture)
                .Include(g => g.PlaingField)
                .Include(g => g.SystemRequirements)
                .Where(g => g.Name.Contains(searchName))
                .ToListAsync();

            return PartialView("Assortment", games);
        }

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
