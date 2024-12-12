using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using E_Administration.Data;
using Microsoft.EntityFrameworkCore;
using E_Administration.Models;

namespace E_Administration.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly DemoDbContext ctx;

        public AdminController(DemoDbContext ctx)
        {
            this.ctx = ctx;
        }

        public IActionResult Index()
        {
            return View("Dashboard");
        }

    public async Task<IActionResult>Contact()
        {
            var contact = await ctx.Contacts.ToListAsync();
            return View(contact);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var contact = await ctx.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            return View(contact);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Contact contact)
        {
            if (id != contact.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ctx.Update(contact);
                    await ctx.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Contact));
            }
            return View(contact);
        }

        private bool ContactExists(int id)
        {
            return ctx.Contacts.Any(e => e.Id == id);
        }
    }
}
    
