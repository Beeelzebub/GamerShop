using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ГеймерShop.Data;
using ГеймерShop.Models;

namespace ГеймерShop.Controllers
{
    public class KeysController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KeysController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationDbContext = _context.Keys.Include(k => k.Game).Include(k => k.KeyStatus).Where(k => k.GameId == id);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Key key)
        {
            if (ModelState.IsValid)
            {
                await _context.AddAsync(key);
                await _context.SaveChangesAsync();

                return RedirectToAction("Management", "Games");
            }

            ViewData["GameId"] = new SelectList(_context.Games, "Id", "Name");

            return View(key);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var key = await _context.Keys.FirstOrDefaultAsync(m => m.Id == id);

            if (key == null)
            {
                return NotFound();
            }

            int? gameId = key.GameId;

            _context.Keys.Remove(key);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { id = gameId });
        }
        
    }
}
