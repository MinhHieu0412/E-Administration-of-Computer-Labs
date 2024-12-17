<<<<<<< HEAD
﻿using E_Administration.Data;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Administration.Areas.User.Controllers
{
    [Area("User")]
    public class PageUserController : Controller
    {
        private readonly DemoDbContext _context;

        public PageUserController(DemoDbContext ctx)
        {
            _context = ctx;
        }

        public IActionResult Index()
        {
            // Lấy danh sách eLearning từ database
            var ELearns = _context.ELearning.ToList();

            // Tạo ViewModel và gán danh sách Elearnings
            var viewModel = new HomeViewModel
            {
                Elearnings = ELearns
            };

            // Truyền ViewModel vào View
            return View(viewModel);
=======
﻿using Microsoft.AspNetCore.Mvc;

namespace E_Administration.Controllers
{
<<<<<<<< HEAD:E-Administration/Controllers/HomeController.cs
    public class HomeController : Controller
========
    [Area("User")]
    public class PageUserController : Controller
>>>>>>>> duyanh:E-Administration/Areas/User/Controllers/PageUserController.cs
    {
        public IActionResult Index()
        {
            return View();
>>>>>>> duyanh
        }
    }
}
