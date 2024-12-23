using E_Administration.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Administration.Controllers
{
    public class ContactController : Controller
    {
        private readonly DemoDbContext ctx;

        public ContactController(DemoDbContext ctx)
        {
            this.ctx = ctx;
        }
        public async Task<IActionResult> Contact()
        {
            var contacts = await ctx.Contacts.ToListAsync();
            return View(contacts);
        }
    }
}
