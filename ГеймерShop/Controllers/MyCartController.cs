using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ГеймерShop.Data;
using ГеймерShop.Models;
using ГеймерShop.ViewModels;

namespace ГеймерShop.Controllers
{
    public class MyCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public MyCartController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult MyCart()
        {
            Cart cart = HttpContext.Session.Get<Cart>("cart");
            return View(cart);
        }

        public IActionResult OverflowNext()
        {
            Cart cart = HttpContext.Session.Get<Cart>("cart");
            Cart reserveCart = HttpContext.Session.Get<Cart>("reserveCart");

            foreach (var item in reserveCart.Lines())
            {
                cart.RemoveItem(_context.Games.Where(g => g.Id == item.Game.Id).FirstOrDefault(), 
                    cart.Lines().Where(c => c.Game.Id == item.Game.Id).FirstOrDefault().Count - item.Count);
            }

            HttpContext.Session.Set<Cart>("cart", cart);
            HttpContext.Session.Set<Cart>("reserveCart", null);

            return RedirectToAction(nameof(PaymentInfo));
        }

        [Authorize]
        public IActionResult PaymentInfo()
        {
            bool overflow = false;
            Cart reserveCart = new Cart();
            Cart cart = HttpContext.Session.Get<Cart>("cart");
            

            foreach(var item in cart.Lines())
            {
                int activeKeys = _context.Keys.Where(k => k.GameId == item.Game.Id).Where(k => k.KeyStatusId == 1).Count();
                
                if (activeKeys < item.Count)
                {
                    overflow = true;
                }
                else
                {
                    continue;
                }

                reserveCart.AddItem(_context.Games.Where(g => g.Id == item.Game.Id).FirstOrDefault(), activeKeys);
            }

            if (!overflow)
            {
                var email = HttpContext.Session.Get<string>("email");

                if (email != null)
                {
                    return View(new PaymentInfoViewModel { Email = email });
                }

                return View();
            }

            HttpContext.Session.Set<Cart>("reserveCart", reserveCart);

            return View("Overflow", reserveCart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult PaymentInfo(PaymentInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.Set<string>("email", model.Email);

                return View("Pay");
               
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Pay()
        {
            string emailBodyText = "Ваша покупка. Good luck! Have fun!\n";
            Cart cart = HttpContext.Session.Get<Cart>("cart");
            List<Key> keys = new List<Key>();

            foreach (var item in cart.Lines())
            {
                Game game = _context.Games.Where(g => g.Id == item.Game.Id).FirstOrDefault();

                if (game != null)
                {
                    var keysTemp = _context.Keys
                        .Where(k => k.GameId == game.Id)
                        .Where(k => k.KeyStatusId == 1)
                        .Take(item.Count);
                    if (keysTemp != null)
                    {
                        keys.AddRange(keysTemp);

                        emailBodyText += game.Name + ": ";

                        foreach (var item2 in keysTemp)
                        {
                            emailBodyText += item2.Value + " ";
                        }
                        emailBodyText +=  "\n";
                    }
                }

            }

            foreach (var key in keys)
            {
                key.KeyStatusId = 2;
            }

            Order order = new Order
            {
                Customer = await _userManager.GetUserAsync(User),
                Keys = keys.ToList(),
                Cart = HttpContext.Session.GetSessionString("cart"),
                Email = HttpContext.Session.Get<string>("email"),
                OrderDate = DateTime.Now
            };

            _context.Keys.UpdateRange(keys);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            EmailService emailService = new EmailService();
            await emailService.SendEmailAsync(order.Customer.UserName, order.Email, "Ваша покупка", emailBodyText);

            HttpContext.Session.Set<Cart>("cart", null);

            return View("PayInfo", order.Email);
        }

        public IActionResult Add(int Id)
        {
            Cart cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();

            Game game = _context.Games.Where(g => g.Id == Id).FirstOrDefault();

            if (game != null)
            {
                cart.AddItem(game, 1);
                HttpContext.Session.Set<Cart>("cart", cart);

                return PartialView(true);
            }

            return PartialView(false);
        }

        public IActionResult Remove(int Id)
        {
            Cart cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();

            Game game = _context.Games.Where(g => g.Id == Id).FirstOrDefault();

            if (game != null)
            {
                cart.RemoveItem(game, 1);

                if (cart.Lines().Count() == 0)
                {
                    cart = null;
                }

                HttpContext.Session.Set<Cart>("cart", cart);
            }

            return RedirectToAction(nameof(MyCart));
        }
    }
}
