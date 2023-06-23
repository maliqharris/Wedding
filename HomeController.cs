using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LogReg.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LogReg.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

      private MyContext _context;

    public HomeController(ILogger<HomeController> logger,  MyContext context)
    {
        _logger = logger;
         _context = context;


    }

    public IActionResult Index()
    {
        
        return View("Index");
    }




    [HttpPost("register")]
    public IActionResult Register(User newUser)
    {
        if(!ModelState.IsValid)
        {
            return Index();
        }
        PasswordHasher<User> hashBrowns = new PasswordHasher<User>();
        newUser.Password = hashBrowns.HashPassword(newUser, newUser.Password);
        _context.Users.Add(newUser);
        _context.SaveChanges();

        HttpContext.Session.SetInt32("UUID", newUser.UserId);
        return RedirectToAction("Success");

    }


    [HttpGet("weddings")]
    public IActionResult Success()
    {
           // GET NAME
        int? userId = HttpContext.Session.GetInt32("UUID");
        User user = _context.Users.FirstOrDefault(user => user.UserId == userId);
        if (user == null)
            {
                return RedirectToAction("Index");
            }
    ViewBag.FirstName = user?.FirstName;

// List of weddings obv, Uses Inclued to get the num of guests!
    List<Wedding> weddings = _context.Weddings.Include(w => w.Guests).ToList();
    return View("Success", weddings);
    }


    [HttpGet("weddings/new")]
    public IActionResult CreateWedding()
    {
           // GET NAME
         int? userId = HttpContext.Session.GetInt32("UUID");
        User user = _context.Users.FirstOrDefault(user => user.UserId == userId);
         ViewBag.FirstName = user?.FirstName;


        return View("CreateWedding");
    }

    [HttpPost("SaveWedding")]
    public IActionResult SaveWedding(Wedding wedding)
    {
        if (!ModelState.IsValid)
            {
               
                Console.WriteLine("No Good");
                 return View("CreateWedding", wedding);
            }
             wedding.UserId = (int)HttpContext.Session.GetInt32("UUID");
            _context.Weddings.Add(wedding);
            _context.SaveChanges();
            return RedirectToAction("Success");

    }

    [HttpGet("weddings/{weddingId}")]
    public IActionResult Guests(int weddingId)
    {
        Wedding wedding = _context.Weddings
        .Include(w => w.Guests)
        .ThenInclude(r => r.User)
        .FirstOrDefault(w => w.WeddingId == weddingId);

   // GET NAME
         int? userId = HttpContext.Session.GetInt32("UUID");
        User user = _context.Users.FirstOrDefault(user => user.UserId == userId);
         ViewBag.FirstName = user?.FirstName;

        if (wedding == null)
        {
            return RedirectToAction("Index");
        }

        return View("Guests", wedding);
}


    [HttpPost("RSVP")]
    // ----------------------Get wed Id
    public IActionResult RSVP(int weddingId)
    {

    // GET IDS FROM SESSION, Include to make guests list!
    int? userId = HttpContext.Session.GetInt32("UUID");
    User user = _context.Users.FirstOrDefault(u => u.UserId == userId);
    Wedding wedding = _context.Weddings.Include(w => w.Guests).FirstOrDefault(w => w.WeddingId == weddingId);
    // Check if null , find guest that are linked to user, if none add! if it is there it removes! Like a toggle.
    if (user != null && wedding != null)
    {
        Guests guests = wedding.Guests.FirstOrDefault(g => g.UserId == user.UserId);
        if (guests == null)
        // ADD!
        {
            guests = new Guests { User = user, Wedding = wedding };
            _context.Guests.Add(guests);
        }
        else
        // remove!
        {
            _context.Guests.Remove(guests);
        }
        _context.SaveChanges();
    }

    return RedirectToAction("Success");
}




    [HttpPost("login")]
    public IActionResult Login(LoginUser loginUser)
    {
        if(!ModelState.IsValid)
        {
            return Index();
        }
        User? dbUser = _context.Users.FirstOrDefault(user => user.Email == loginUser.LoginEmail);
        if(dbUser == null)
        {
            ModelState.AddModelError("Email", "not found");
            return Index();
        }
        PasswordHasher<LoginUser> hashBrowns = new PasswordHasher<LoginUser>();
        PasswordVerificationResult pwCompareResult = hashBrowns.VerifyHashedPassword(loginUser, dbUser.Password, loginUser.LoginPassword);

        if(pwCompareResult == 0)
        {
            ModelState.AddModelError("LoginPassword", "invalid password");
        }
        HttpContext.Session.SetInt32("UUID", dbUser.UserId);
        return RedirectToAction("Success");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
