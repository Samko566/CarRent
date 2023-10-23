using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarRent.Models;
using CarRent.Data;
using CarRent.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace CarRent.Controllers
{
    public class CartController : Controller
    {
        private readonly CarDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CartController(CarDbContext context, UserManager<AppUser> userManager)
        {
            _context = context; //ініціалізація контексту бази даних
            _userManager = userManager; //ініціалізація менеджера користувачів
        }

        [Authorize]
        public async Task<IActionResult> AddToCart(int carId, DateTime startDate, DateTime endDate)
        {
            // Отримуємо поточного користувача
            var user = await _userManager.GetUserAsync(User);

            // Отримуємо автомобіль з бази даних CarDbContext
            var car = await _context.Car.FindAsync(carId);

            // Перевіряємо, що автомобіль існує і доступний для оренди
            if (car == null || !car.Available)
            {
                return RedirectToAction("Index", "Car"); //повертаємо користувача на сторінку з автомобілями
            }

            // Визначаємо вартість оренди
        var dailyRate = car.DailyRate;
            var days = (endDate - startDate).Days;
            var totalPrice = days * dailyRate;

            // Створюємо новий об'єкт CartItem
            var cartItem = new CartItem
            {
                CarId = car.Id,
                UserId = user.Id,
                StartDate = startDate,
                EndDate = endDate,
                DailyRate = dailyRate,
                TotalPrice = totalPrice
            };

            // Додаємо об'єкт CartItem в базу даних
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewCart", "Cart"); //повертаємо користувача на сторінку з корзиною
        }

        [Authorize]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            // Отримуємо об'єкт CartItem з бази даних
            var cartItem = await _context.CartItems.FindAsync(id);

            // Якщо об'єкт не знайдено, повертаємо користувача на сторінку з корзиною
            if (cartItem == null)
            {
                return RedirectToAction("ViewCart", "Cart");
            }

            // Видаляємо об'єкт з бази даних
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewCart", "Cart");
        }

        public async Task<int> CalculateRentPrice(int carId, DateTime startDate, DateTime endDate, int discount, int penalty)
        {
            // Отримуємо автомобіль із бд CarDbContext
            var car = await _context.Car.FindAsync(carId);

            // Перевіряємо, що автомобіль існує і доступний для оренди
            if (car == null || !car.Available)
            {
                return 0;
            }

            // Обчислюємо вартість оренди
            var dailyRate = car.DailyRate;
            var days = (endDate - startDate).Days;
            var totalPrice = days * dailyRate - penalty + discount;

            return totalPrice;
        }

        //Сторінка історія замовлень
        [Authorize]
        public IActionResult Index()
        {
            var cartItems = _context.CartItems
                .Include(ci => ci.Car)
                .GroupBy(ci => ci.UserId)
                .Select(group => group.ToList())
                .ToList();

            var cartItemList = cartItems.SelectMany(group => group.Select(ci => new CartItem
            {
                Car = ci.Car,
                StartDate = ci.StartDate,
                EndDate = ci.EndDate,
                DailyRate = ci.DailyRate,
                Discount = ci.Discount,
                Penalty = ci.Penalty,
                TotalPrice = ci.TotalPrice
            })).ToList();

            return View(cartItemList);
        }

        //Корзина
        [Authorize]
        public async Task<IActionResult> ViewCart()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Car)
                .ToList();

            foreach (var item in cartItems)
            {
                item.TotalPrice = (int)CalculateRentalPrice(item);
            }

            var model = cartItems;

            return View(model);
        }

        //здійснює оформлення замовлення користувачем, коли він готовий оплатити свої товари.
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            decimal totalPrice = 0;
            foreach (var item in cartItems)
            {
                item.TotalPrice = (int)CalculateRentalPrice(item);
                totalPrice += item.TotalPrice;
            }

            // Зберегти запис про продаж
            var sale = new Sale
            {
                UserId = user.Id,
                SaleDate = DateTime.Now,
                TotalPrice = totalPrice
            };
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            // Оновити наявність автомобіля
            foreach (var item in cartItems)
            {
                var car = await _context.Car.FindAsync(item.CarId);
                car.Available = false;
            }

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewCart", "Cart");
        }

        private decimal CalculateRentalPrice(CartItem item)
        {
            var rentalDays = (item.EndDate - item.StartDate).Days + 1;

            // Базова ціна
            var basePrice = item.DailyRate * rentalDays;

            //Застосувати знижку в %, якщо така є
            if (item.Discount > 0)
            {
                basePrice -= (basePrice * item.Discount) / 100;
            }

            // Застосувати штраф, якщо такий є
            if (item.Penalty > 0)
            {
                basePrice += item.Penalty;
            }

            return basePrice;
        }
    }
}
