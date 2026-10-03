using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LostAndFound.Data;
using LostAndFound.Models;

namespace LostAndFound.Controllers
{
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, ItemType? type)
        {
            try
            {
                var items = _context.Items.Include(i => i.Category).AsQueryable();

                if (!string.IsNullOrEmpty(searchString))
                {
                    items = items.Where(s => s.Title.Contains(searchString) || s.Location.Contains(searchString));
                }

                if (type.HasValue)
                {
                    items = items.Where(s => s.Type == type.Value);
                }

                return View(await items.OrderByDescending(i => i.CreatedAt).ToListAsync());
            }
            catch
            {
                // หากต่อ Database บน Cloud ไม่ได้ ให้ส่ง List ว่างกลับไปแทนเพื่อให้หน้าเว็บรันติด ไม่แสดง Error 500
                return View(new List<Item>());
            }
        }

        public IActionResult Create()
        {
            try
            {
                ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
            }
            catch
            {
                ViewBag.CategoryId = new SelectList(Enumerable.Empty<SelectListItem>());
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Item item, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                        if (!Directory.Exists(uploadDir))
                        {
                            Directory.CreateDirectory(uploadDir);
                        }

                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        string filePath = Path.Combine(uploadDir, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        item.ImageUrl = "/uploads/" + fileName;
                    }

                    item.CreatedAt = DateTime.Now;
                    _context.Add(item);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    ModelState.AddModelError("", "ไม่สามารถบันทึกข้อมูลได้เนื่องจากไม่ได้เชื่อมต่อ Database");
                }
            }

            try
            {
                ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", item.CategoryId);
            }
            catch
            {
                ViewBag.CategoryId = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var item = await _context.Items.FindAsync(id);
                if (item != null)
                {
                    if (!string.IsNullOrEmpty(item.ImageUrl))
                    {
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", item.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    _context.Items.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
            catch
            {
                // ดักจับ Error ตอนลบข้อมูล
            }

            return RedirectToAction(nameof(Index));
        }
    }
}