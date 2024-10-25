using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // POST: Insuree/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                // Base premium of $50
                decimal quote = 50m;

                // Calculate user's age
                int age = DateTime.Now.Year - insuree.DateOfBirth.Year;
                if (DateTime.Now.DayOfYear < insuree.DateOfBirth.DayOfYear)
                {
                    age--; // Adjust if the user's birthday hasn't occurred this year
                }

                // Age-based additions
                if (age <= 18)
                {
                    quote += 100;
                }
                else if (age >= 19 && age <= 25)
                {
                    quote += 50;
                }
                else if (age > 25)
                {
                    quote += 25;
                }

                // Car year additions
                if (insuree.CarYear < 2000)
                {
                    quote += 25;
                }
                else if (insuree.CarYear > 2015)
                {
                    quote += 25;
                }

                // Car make and model additions
                if (insuree.CarMake.ToLower() == "porsche")
                {
                    quote += 25;
                    if (insuree.CarModel.ToLower() == "911 carrera")
                    {
                        quote += 25; // Additional $25 for Porsche 911 Carrera
                    }
                }

                // Add $10 for every speeding ticket
                quote += insuree.SpeedingTickets * 10;

                // If the user has had a DUI, increase the quote by 25%
                if (insuree.DUI)
                {
                    quote *= 1.25m;
                }

                // If full coverage is selected, increase the quote by 50%
                if (insuree.CoverageType)
                {
                    quote *= 1.5m;
                }

                // Assign calculated quote to the Insuree object
                insuree.Quote = quote;

                // Save the insuree object to the database
                db.Insurees.Add(insuree);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insuree);
        }

        // GET: Insuree/Admin
        public ActionResult Admin()
        {
            var quotes = db.Insurees.ToList(); // Fetching all insurees from the database
            return View(quotes); // Returning the list to the Admin view
        }
    }
}
