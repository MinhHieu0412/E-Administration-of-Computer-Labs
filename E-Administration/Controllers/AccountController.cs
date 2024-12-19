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
                    //string hashedPassword = BCrypt.Net.BCrypt.HashPassword("$12$OuYictQNaUloxwWwozyyauBn8iVw9IKMnskt0AXfQeBUiTIbzCdCm");

                    if (!BCrypt.Net.BCrypt.Verify(model.Password, acc.Password))
                    {
                        ModelState.AddModelError("Password", "Invalid password.");
                    }
                    else
                    {
                        // Tạo thông tin xác thực
                        var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, acc.Email),
                        new Claim(ClaimTypes.Role, acc.Role)
                    };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        // Chuyển hướng sau khi đăng nhập
                        if (acc.Role == "Admin")
                        {
                            return RedirectToAction("Index", "Admin", new { area = "Admin" });
                        }
                        else if (acc.Role == "Technician")
                        {
                            return RedirectToAction("Index", "PageUser", new { area = "User" });
                        }
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
