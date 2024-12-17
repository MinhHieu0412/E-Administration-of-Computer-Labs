using E_Administration.Data;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Administration.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ListEquipment : Controller
    {
        private readonly DemoDbContext ctx;

        public ListEquipment(DemoDbContext ctx)
        {
            this.ctx = ctx;
        }
        [HttpGet]
        [Route("Admin/ListEquipment/{labId}")]
        public IActionResult Index(int labId)
        {
            var equipmentList = ctx.Equipments
                .Where(e => e.LabID == labId) // Assuming "LabID" is a foreign key in Equipment table
                .ToList();

            // Pass the equipment list to the view
            return View(equipmentList);
        }

        [HttpPost]
        [Route("Admin/ListEquipment/AddEquipment")]
        public IActionResult AddEquipment([FromBody] Equipments newEquipment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            newEquipment.CreatedAt = DateTime.Now;
            newEquipment.UpdatedAt = DateTime.Now;


            // Lưu thiết bị mới vào cơ sở dữ liệu
            ctx.Equipments.Add(newEquipment);
            ctx.SaveChanges();

            return Ok(newEquipment); // Trả về đối tượng vừa thêm
        }

    }
}
