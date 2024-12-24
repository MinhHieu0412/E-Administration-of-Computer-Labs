using E_Administration.Data;
using E_Administration.Dto;
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

            var labName = ctx.Labs
        .Where(l => l.ID == labId) // Assuming "Labs" table exists with ID as primary key
        .Select(l => l.Name)
        .FirstOrDefault();

            // Gán LabID và LabName vào ViewBag
            ViewBag.LabId = labId;
            ViewBag.LabName = labName;

            // Pass the equipment list to the view
            return View(equipmentList);
        }

        [HttpPost]
        [Route("Admin/ListEquipment/AddEquipment")]
        public IActionResult AddEquipment([FromBody] EquipmentDto newEquipment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map DTO to the actual model (Equipments) with timestamps set
            Equipments equipmentModel = MapToModel(newEquipment);

            // Add the equipment model to the database
            ctx.Equipments.Add(equipmentModel);
            ctx.SaveChanges();

            return Ok(equipmentModel); // Return the added model
        }


        public Equipments MapToModel(EquipmentDto dto)
        {
            return new Equipments
            {
                LabID = dto.LabID,
                Name = dto.Name,
                SerialNumber = dto.SerialNumber,
                EquipmentDetails = dto.EquipmentDetails,
                Status = dto.Status,
                CreatedAt = DateTime.Now,  // Set CreatedAt timestamp here
                UpdatedAt = DateTime.Now
            };
        }

        public async Task<IActionResult> Delete(int id, int labId)
        {
            // Find the equipment by its ID
            var equipment = await ctx.Equipments.FindAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }

            // Remove the equipment from the database
            ctx.Equipments.Remove(equipment);
            await ctx.SaveChangesAsync();

            // Redirect back to Index with the current LabID
            return RedirectToAction("Index", new { labId });
        }



    }
}
