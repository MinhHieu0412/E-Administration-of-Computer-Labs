using E_Administration.Data;
using E_Administration.Dto;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Net.Mail;
using System.Net;

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
                         new Claim(ClaimTypes.NameIdentifier, acc.ID.ToString()), // Thêm User ID vào claim
        new Claim(ClaimTypes.Email, acc.Email),
        new Claim(ClaimTypes.Role, acc.Role),
        new Claim("UserName", acc.UserName)
                    };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        ViewBag.UserEmail = acc.Email;
                        ViewBag.UserRole = acc.Role;
                        ViewBag.UserName = acc.UserName;

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

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await ctx.Users.SingleOrDefaultAsync(x => x.Email == model.Email);
                if (user != null)
                {
                    var token = Guid.NewGuid().ToString();
                    user.ResetToken = token;
                    user.ResetTokenExpiry = DateTime.Now.AddHours(1);
                    await ctx.SaveChangesAsync();

                    var resetLink = Url.Action("ResetPassword", "Account", new { token }, Request.Scheme);

                    // Send Email
                    using (var smtp = new SmtpClient("smtp.gmail.com"))
                    {
                        smtp.Port = 587;
                        smtp.Credentials = new NetworkCredential("minhhieu114a@gmail.com", "xvgk edae kbki rbuz");
                        smtp.EnableSsl = true;

                        var mail = new MailMessage
                        {
                            From = new MailAddress("minhhieu114a@gmail.com"),
                            Subject = "Password Reset",
                            Body = $"Please click the link to reset your password: <a href='{resetLink}'>Reset Password</a>",
                            IsBodyHtml = true
                        };

                        mail.To.Add(model.Email);
                        await smtp.SendMailAsync(mail);
                    }

                    ViewBag.Message = "Password reset link has been sent to your email.";
                }
                else
                {
                    ModelState.AddModelError("Email", "Email not found.");
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            var model = new ResetPasswordModel { Token = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await ctx.Users.SingleOrDefaultAsync(x => x.ResetToken == model.Token && x.ResetTokenExpiry > DateTime.Now);
                if (user != null)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                    user.ResetToken = null;
                    user.ResetTokenExpiry = null;
                    await ctx.SaveChangesAsync();

                    ViewBag.Message = "Password has been reset successfully.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("Token", "Invalid or expired token.");
                }
            }
            return View(model);
        }
    }
}
