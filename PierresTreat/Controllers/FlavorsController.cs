using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using PierresTreat.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace PierresTreat.Controllers
{
  public class FlavorsController : Controller
  {
    private readonly PierresTreatContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public FlavorsController(UserManager<ApplicationUser> userManager, PierresTreatContext db)
    {
      _userManager = userManager;
      _db = db;
    }

    public ActionResult Index()
    {
      ViewBag.PageTitle = "View All Flavors";
      return View(_db.Flavors.ToList());
    }

    public ActionResult Create()
    {
      ViewBag.PageTitle = "Add New Flavor";
      return View();
    }

     [HttpPost]
    public ActionResult Create(Flavor flavor)
    {
      if (_db.Flavors.FirstOrDefault(a => a.Name == flavor.Name) == null)
      {
        _db.Flavors.Add(flavor);
        _db.SaveChanges();
      }
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id)
    {
      Flavor flavor = _db.Flavors.FirstOrDefault(a => a.FlavorId == id);
      ViewBag.PageTitle = $"Flavor {flavor.Name}";

      ViewBag.TreatId = new SelectList(_db.Treats, "TreatId", "Title");
      return View(flavor);
    }

    [HttpPost]
    public ActionResult Details(FlavorTreat ab)
    {
      if (_db.FlavorTreat.FirstOrDefault(a => a.FlavorId == ab.FlavorId && a.TreatId == ab.TreatId) == null)
      {
        _db.FlavorTreat.Add(ab);
        _db.SaveChanges();
      }
      return RedirectToAction("Details", new { id = ab.FlavorId });
    }

    public ActionResult Edit(int id)
    {
      Flavor flavor = _db.Flavors.FirstOrDefault(a => a.FlavorId == id);
      ViewBag.PageTitle = $"Edit {flavor.Name}";
      return View(flavor);
    }

    [HttpPost]
    public ActionResult Edit(Flavor flavor)
    {
      _db.Entry(flavor).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Details", new { id = flavor.FlavorId });
    }

    public ActionResult Delete(int id)
    {
      Flavor flavor = _db.Flavors.FirstOrDefault(a => a.FlavorId == id);
      ViewBag.PageTitle = $"Delete {flavor.Name}?";
      return View(flavor);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult Deleted(int id)
    {
      Flavor flavor = _db.Flavors.FirstOrDefault(a => a.FlavorId == id);
      _db.Flavors.Remove(flavor);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteFlavor(int flavorTreatId)
    {
      var ab = _db.FlavorTreat.FirstOrDefault(a => a.FlavorTreatId == flavorTreatId);
      if (ab != null)
      {
        _db.FlavorTreat.Remove(ab);
        _db.SaveChanges();
      }
      return RedirectToAction("Details", new { id = ab.FlavorId });
    }
  }
}