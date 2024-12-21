using E_Administration.Data;
using E_Administration.Dto;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_Administration.Controllers
{
    public class AccountController : Controller
    {
        readonly DemoDbContext ctx;

        public AccountController(DemoDbContext ctx)
        {
            this.ctx = ctx;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var acc = await ctx.Users.SingleOrDefaultAsync(x => x.Email == model.Email);
                if (acc != null)
                {

                    if(BCrypt.Net.BCrypt.Verify(model.Password, acc.Password))
                    {
                        // Tạo thông tin xác thực
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, acc.Email),
                            new Claim(ClaimTypes.Role, acc.Role),
                            new Claim("UserID", acc.ID.ToString()) // Save the user ID in a custom claim
                        };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        // Chuyển hướng sau khi đăng nhập
                        if (acc.Role == "Admin")
                        {
                            return RedirectToAction("Index", "Admin", new { area = "Admin" });
                        }
                        else 
                        {
                            return RedirectToAction("Index", "PageUser", new { area = "User" });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Invalid password.");
                    }
                   
                }
                else
                {
                    // Email not found
                    ModelState.AddModelError("Email", "Email not found.");
                }
            }
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("Login", "Account");
        }
    }
}
