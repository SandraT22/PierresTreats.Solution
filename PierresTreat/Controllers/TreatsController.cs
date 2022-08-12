using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PierresTreat.Models;
using System.Collections.Generic;
using System.Linq;

namespace PierresTreat.Controllers
{
  public class TreatsController : Controller
  {
    private readonly PierresTreatContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public TreatsController(UserManager<ApplicationUser> userManager, PierresTreatContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public async Task<ActionResult> Index()
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      IEnumerable<Treat> treats = _db.Treats.ToList();
      if (userId != null)
      {
        var currentUser = await _userManager.FindByIdAsync(userId);
        IEnumerable<Treat> userTreats = _db.Treats.Where(treat => treat.User.Id == currentUser.Id).ToList();
        treats = _db.Treats.Where(treat => treat.User.Id != currentUser.Id).ToList();
        return View((treats, userTreats));
      }
      IEnumerable<Treat> emptyList = new List<Treat>();
      return View((treats, emptyList));
    }

    public ActionResult Create()
    {
      ViewBag.AuthorId = new SelectList(_db.Authors, "AuthorId", "Name");
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> Create(Treat treat, int AuthorId)
    {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      treat.User = currentUser;
      _db.Treats.Add(treat);
      _db.SaveChanges();
      if (AuthorId != 0)
      {
        _db.AuthorTreat.Add(new AuthorTreat() { AuthorId = AuthorId, TreatId = treat.TreatId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      var thisTreat = _db.Treats
          .Include(treat => treat.JoinEntities)
          .ThenInclude(join => join.Author)
          .FirstOrDefault(treat => treat.TreatId == id);
      // var thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == id);
      ViewBag.Copies = _db.Copies.Where(copy => copy.TreatId == id);
      return View(thisTreat);
    }

    public ActionResult Edit(int id)
    {
      var thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == id);
      ViewBag.AuthorId = new SelectList(_db.Authors, "AuthorId", "Name");
      return View(thisTreat);
    }

    [HttpPost]
    public ActionResult Edit(Treat treat, int AuthorId)
    {
      if (AuthorId != 0)
      {
        _db.AuthorTreat.Add(new AuthorTreat() { AuthorId = AuthorId, TreatId = treat.TreatId });
      }
      _db.Entry(treat).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddAuthor(int id)
    {
      var thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == id);
      ViewBag.AuthorId = new SelectList(_db.Authors, "AuthorId", "Name");
      return View(thisTreat);
    }

    [HttpPost]
    public ActionResult AddAuthor(Treat treat, int AuthorId)
    {
      if (AuthorId != 0)
      {
        _db.AuthorTreat.Add(new AuthorTreat() { AuthorId = AuthorId, TreatId = treat.TreatId });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id)
    {
      var thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == id);
      return View(thisTreat);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id)
    {
      var thisTreat = _db.Treats.FirstOrDefault(treat => treat.TreatId == id);
      _db.Treats.Remove(thisTreat);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteAuthor(int joinId)
    {
      var joinEntry = _db.AuthorTreat.FirstOrDefault(entry => entry.AuthorTreatId == joinId);
      _db.AuthorTreat.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}
