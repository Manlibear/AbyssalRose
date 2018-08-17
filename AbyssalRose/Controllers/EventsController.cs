using AbyssalRose.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AbyssalRose.Controllers
{
    public class EventsController : Controller
    {
        // GET: Events
        public ActionResult Index()
        {
            EventsViewModel evm = new EventsViewModel();
            ApplicationDbContext db = new ApplicationDbContext();

            evm.AllEvents = db.Events.ToList(); 

            return View(evm);
        }
    }
}