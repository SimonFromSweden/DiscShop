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
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscShop.Controllers
{
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            List<Cart> carts;
            if (await _userManager.IsInRoleAsync(user, "ADMIN"))
            {

                carts = await _context.Cart
                             .Include(c => c.Disc)
                             .Include(u => u.User)
                             .OrderByDescending(i => i.Id)
                             .ToListAsync();
            }
            else
            {
                carts = await _context.Cart
                             .Where(i => i.UserId == userId)
                             .Include(c => c.Disc)
                             .Include(u => u.User)
                             .OrderByDescending(i => i.Id)
                             .ToListAsync();
            }

            return View(carts);
        }

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Disc)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        [Authorize(Roles = "Administrator,Customer")]
        // GET: Carts/Cart
        public async Task<IActionResult> Cart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _context.Cart
                                    .Where(c => c.UserId == userId)
                                    .Include(c => c.Disc)
                                    .ToListAsync(); // Filters so that only the users cart-items are included, and includes Disc-item

            var cartItemCount = 0;
            foreach (var cartItem in cartItems)
            {
                cartItemCount += cartItem.Quantity;
            }
            ViewData["CartItemCount"] = cartItemCount;

            return View(cartItems);  // Passing the correct model to the cart-view
        }

        // POST: Carts/AddToCart
        [HttpPost]
        [Authorize(Roles = "Administrator, Customer")]
        public async Task<ActionResult> AddToCart([FromBody] int discId)
        {
            Console.WriteLine($"Received discId: {discId}");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Find userId

            var cartItems = await _context.Cart
                                    .Where(c => c.UserId == userId)
                                    .Include(c => c.Disc)
                                    .ToListAsync();

            var discInCart = cartItems.FirstOrDefault(c => c.DiscId == discId);
            var discToAdd = await _context.Disc.FindAsync(discId);

            if (discInCart != null)
            {
                if (discToAdd != null && discToAdd.Quantity > discInCart.Quantity) {
                    discInCart.Quantity++;
                    _context.Update(discInCart);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return Json(new { success = true, message = "Can't add more of this disc - Not enough in stock." });
                }
            }
            else
            {
                if (discToAdd != null)
                {
                    var newCartItem = new Cart
                    {
                        UserId = userId,
                        DiscId = discId,
                        Quantity = 1
                    };

                    _context.Cart.Add(newCartItem);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return Json(new { success = false, message = "Disc not found." });
                }
            }
            return Json(new { success = true, message = "Disc added to basket." });
        }

        // POST: Carts/RemoveFromCart
        [HttpPost]
        [Authorize(Roles = "Administrator, Customer")]
        public async Task<ActionResult> RemoveFromCart([FromBody] int discId)
        {
            Console.WriteLine($"Received discId: {discId}");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Find userId

            var cartItem = await _context.Cart // gather cart-item belonging to active user matching the clicked disc-button
                            .Where(c => c.UserId == userId && c.Disc.Id == discId)
                            .Include(c => c.Disc)
                            .FirstOrDefaultAsync();

            if (cartItem != null)
            {
                if (cartItem.Quantity > 1)
                {
                    // Decrease quantity by 1
                    cartItem.Quantity--;
                    _context.Cart.Update(cartItem);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Remove item from cart
                    _context.Cart.Remove(cartItem);
                    await _context.SaveChangesAsync();
                }
                //TempData["CartMessage"] = $"{cartItem.Disc.Brand} {cartItem.Disc.Model} removed from cart"; // Writing temp data
                return Json(new { success = true, message = "Disc removed from basket." });
            }
            else
            {
                return Json(new { success = false, message = "Disc not found in cart." });
            }
        }
    

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["DiscId"] = new SelectList(_context.Disc, "Id", "Id");

            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Customer")]
        public async Task<IActionResult> Create([Bind("Id,Quantity,UserId,DiscId")] Cart cart)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/AccessDenied");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Find user Id of logged in person
            var user = await _userManager.FindByIdAsync(userId); // Find user in database by their Id
            cart.User = user; // Assign user with Id userId to cart (represents logged in user)

            var discToAdd = _context.Disc.Find(cart.DiscId); // find the disc that is to be added to cart

            var existingCartItem = _context.Cart.Where(i => i.UserId == userId).FirstOrDefault(c => c.DiscId == cart.DiscId);
            if (existingCartItem != null) {
                existingCartItem.Quantity += cart.Quantity;
                _context.Update(existingCartItem);
            }
            else
            {
                _context.Add(cart);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["DiscId"] = new SelectList(_context.Disc, "Id", "Id", cart.DiscId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DiscId,Quantity,UserId")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            //}
            //ViewData["DiscId"] = new SelectList(_context.Disc, "Id", "Id", cart.DiscId);
            //return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Disc)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Cart.FindAsync(id);
            if (cart != null)
            {
                _context.Cart.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Cart.Any(e => e.Id == id);
        }
    }
}
