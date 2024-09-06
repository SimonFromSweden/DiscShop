using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DiscShop.Data;
using DiscShop.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace DiscShop.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PurchasesController> _logger; // specifies that the logger will only be used here
        private readonly UserManager<IdentityUser> _userManager;

        public PurchasesController(ApplicationDbContext context, ILogger<PurchasesController> logger, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: Purchases
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationDbContext = _context.Purchases
                .Where(p => p.UserId == userId)
                .Include(p => p.Cart);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Purchases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(p => p.Cart)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // GET: Purchases/Create
        public IActionResult Create()
        {
            ViewData["CartId"] = new SelectList(_context.Cart, "Id", "Id");
            return View();
        }

        // POST: Purchases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CartId,Quantity,TotalPrice,PurchaseDate,UserId")] Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                _context.Add(purchase);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CartId"] = new SelectList(_context.Cart, "Id", "Id", purchase.CartId);
            return View(purchase);
        }

        // GET: Purchases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }
            ViewData["CartId"] = new SelectList(_context.Cart, "Id", "Id", purchase.CartId);
            return View(purchase);
        }

        // POST: Purchases/PurchaseItems
        [HttpPost]
        public async Task<IActionResult> PurchaseItems()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Find userId
            var cartItems = await _context.Cart
                                    .Where(c => c.UserId == userId)
                                    .Include(c => c.Disc)
                                    .ToListAsync();

            foreach (var item in cartItems)
            {
                // Save each item as a purchase
                _context.Purchases.Add(new Purchase
                {
                    CartId = item.Id,
                    Quantity = item.Quantity,
                    TotalPrice = item.Disc.Price * item.Quantity,
                    PurchaseDate = DateTime.Now,
                    UserId = userId,
                });

                // Find disc in Discs Database
                var disc = _context.Disc.FirstOrDefault(d => d.Id == item.DiscId);

                if (disc != null)
                {
                    // Reduce the quantity in discs database
                    disc.Quantity -= item.Quantity;
                    _context.Entry(disc).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                else
                {
                    _logger.LogWarning("Disc not found in the database: Disc ID = {DiscId}", item.Disc.Id);
                }
            }

            // Remove cart items after processing the purchases
            //

            // Save all changes to the database
            await _context.SaveChangesAsync();

            _context.Cart.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return RedirectToAction("Success", "Purchases");
        }

        // GET: Discs/Success
        public IActionResult Success()
        {
            return View();
        }

        // POST: Purchases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CartId,Quantity,TotalPrice,PurchaseDate,UserId")] Purchase purchase)
        {
            if (id != purchase.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseExists(purchase.Id))
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
            ViewData["CartId"] = new SelectList(_context.Cart, "Id", "Id", purchase.CartId);
            return View(purchase);
        }

        // GET: Purchases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(p => p.Cart)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // POST: Purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase != null)
            {
                _context.Purchases.Remove(purchase);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseExists(int id)
        {
            return _context.Purchases.Any(e => e.Id == id);
        }
    }
}
