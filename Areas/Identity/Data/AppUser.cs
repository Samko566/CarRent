using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarRent.Models;
using Microsoft.AspNetCore.Identity;

namespace CarRent.Areas.Identity.Data;

//Дані профілю для користувачів програми, властивості класу AppUser
    public class AppUser : IdentityUser
{
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public List<CartItem> CartItems { get; set; }

}

