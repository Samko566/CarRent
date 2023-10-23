using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarRent.Data;
using CarRent.Models;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using Microsoft.AspNetCore.Authorization;

namespace CarRent.Controllers
{
    public class CarsController : Controller
    {
        private readonly CarDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarsController(CarDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Отримання списку автомобілів
        // GET: Cars
        [Authorize]
        public async Task<IActionResult> Index()
        {
            return _context.Car != null ?
                        View(await _context.Car.ToListAsync()) :
                        Problem("Entity set 'CarDbContext.Car'  is null.");
        }

        // Деталі автомобіля
        // GET: Cars/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }

            var car = await _context.Car
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        // Форма для створення автомобіля
        // GET: Cars/Create
        public IActionResult Create()
        {
            return View(new Car());
        }

        // Додавання автомобіля до БД
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Make,Model,Year,Color,DailyRate,Available,CarImage")] Car car)
        {
            if (ModelState.IsValid)
            {
                if (car.CarImage != null && car.CarImage.Length > 0)
                {
                    string fileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(car.CarImage.FileName)}";
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/cars");
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await car.CarImage.CopyToAsync(stream);
                    }

                    car.PhotoPath = fileName;
                }
                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // Форма редагування автомобіля
        // GET: Cars/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }

            var car = await _context.Car.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        // Редагування автомобіля в БД
        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Make,Model,Year,Color,DailyRate,Available,PhotoPath,CarImage")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (car.CarImage != null && car.CarImage.Length > 0)
                    {
                        string fileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(car.CarImage.FileName)}";
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/cars");
                        string filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await car.CarImage.CopyToAsync(stream);
                        }

                        car.PhotoPath = fileName;
                    }
                    else if (car.PhotoPath == null)
                    {
                        car.PhotoPath = "";
                    }

                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // Метод Delete приймає параметр id та перевіряє його на наявність значення та наявність автомобілів у базі даних.
        // GET: Cars/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Car == null)
            {
                return NotFound();
            }
            var car = await _context.Car
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        // Метод DeleteConfirmed приймає ідентифікатор автомобіля як параметр та видаляє його з бази даних. Якщо автомобіль не знайдено, метод поверне код 404.
        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Car == null)
            {
                return Problem("Entity set 'CarDbContext.Car'  is null.");
            }
            var car = await _context.Car.FindAsync(id);
            if (car != null)
            {
                _context.Car.Remove(car);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return (_context.Car?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // Метод CreatePhoto завантажує файл зображення автомобіля та зберігає його на сервері.
        // Якщо файл не знайдено, метод поверне подання Create. В іншому випадку метод поверне представлення Create з
        // параметром PhotoPath, який містить ім'я завантаженого файлу зображення.        
        [Authorize]
        public async Task<IActionResult> CreatePhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return RedirectToAction(nameof(Create));
            }

            var fileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/cars", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return RedirectToAction(nameof(Create), new { photoPath = fileName });
        }

        [Authorize]
        public IActionResult GetPhoto(string photoPath)
        {
            if (photoPath == null)
            {
                return NotFound();
            }
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/cars", photoPath);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }
            var fileContent = System.IO.File.ReadAllBytes(filePath);
            return File(fileContent, "image/jpeg");
        }

    }
}