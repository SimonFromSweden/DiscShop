using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DiscShop.Data;
using DiscShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DiscShop.Controllers
{
    public class DiscsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DiscsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager; 
        }

        // GET: Discs
        [Authorize(Roles = "Administrator")] 
        public async Task<IActionResult> Index()
        {
            return View(await _context.Disc.ToListAsync());
        }

        // GET: Discs/Shop
        public async Task<IActionResult> Shop(string sortBy, string search)
        {
            ViewBag.SelectedSort = sortBy; // allows for remembering the last sort-category

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var cartItems = _context.Cart
									.Where(c => c.UserId == userId)
									.Include(c => c.Disc)
									.ToList();
            var cartItemCount = 0;
            foreach (var cartItem in cartItems)
            {
                cartItemCount += cartItem.Quantity;
            }
            ViewData["CartItemCount"] = cartItemCount; // Send number of items in cart to the shop-view

			var discs = await _context.Disc.ToListAsync(); // Gathering the discs from DB, putting into list

            if (string.IsNullOrEmpty(search)) 
            {
                discs = await _context.Disc.ToListAsync();
            }
            else if (!string.IsNullOrEmpty(search)) // Logic for the search field
            {
                discs = await _context.Disc.Where(disc => disc.Brand.Contains(search) || disc.Model.Contains(search)).ToListAsync();
            }

            switch (sortBy) // Sorting the discs in the shop view according to the select button
            {
                case "Brand":
                    discs = discs.OrderBy(d => d.Brand).ToList();
                    break;
                case "Model":
                    discs = discs.OrderBy(d => d.Model).ToList();
                    break;
                case "Speed":
                    discs = discs.OrderBy(d => d.Speed).ThenByDescending(d => d.Quantity).ToList();
                    break;
                case "Price":
                    discs = discs.OrderBy(d => d.Price).ToList();
                    break;
                case "Stability":
                    discs = discs.OrderBy(d => d.Fade + d.Turn).ToList();
                    break;
                default:
                    discs = discs.OrderBy(d => d.Brand).ToList();
                    break;
            }
            return View(discs);
        }

        // GET: Discs/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disc = await _context.Disc
                .FirstOrDefaultAsync(m => m.Id == id);
            if (disc == null)
            {
                return NotFound();
            }

            return View(disc);
        }

        // GET: Discs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Discs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Brand,Model,Plastic,Colour,Weight,Quantity,Price,Speed,Glide,Turn,Fade,ImgUrl")] Disc disc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(disc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(disc);
        }

        // GET: Discs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disc = await _context.Disc.FindAsync(id);
            if (disc == null)
            {
                return NotFound();
            }
            return View(disc);
        }

        // POST: Discs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Model,Plastic,Colour,Weight,Quantity,Price,Speed,Glide,Turn,Fade,ImgUrl")] Disc disc)
        {
            if (id != disc.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(disc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscExists(disc.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(disc);
        }

        // GET: Discs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disc = await _context.Disc
                .FirstOrDefaultAsync(m => m.Id == id);
            if (disc == null)
            {
                return NotFound();
            }

            return View(disc);
        }

        // POST: Discs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var disc = await _context.Disc.FindAsync(id);
            if (disc != null)
            {
                _context.Disc.Remove(disc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiscExists(int id)
        {
            return _context.Disc.Any(e => e.Id == id);
        }

        // GET: Discs/Home
        public IActionResult Home()
        {
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var cartItems = _context.Cart
									.Where(c => c.UserId == userId)
									.Include(c => c.Disc)
									.ToList();
			var cartItemCount = 0;
			foreach (var cartItem in cartItems)
			{
				cartItemCount += cartItem.Quantity;
			}
			ViewData["CartItemCount"] = cartItemCount;

			var discs = _context.Disc.OrderByDescending(c => c.Id).Take(4).ToList();

			return View(discs);
        }

        // GET: Discs/About
        public IActionResult About()
        {
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var cartItems = _context.Cart
									.Where(c => c.UserId == userId)
									.Include(c => c.Disc)
									.ToList();
			var cartItemCount = 0;
			foreach (var cartItem in cartItems)
			{
				cartItemCount += cartItem.Quantity;
			}
			ViewData["CartItemCount"] = cartItemCount;

			return View();
        }

    }
}
