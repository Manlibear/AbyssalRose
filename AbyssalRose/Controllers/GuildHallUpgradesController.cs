using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AbyssalRose.Models;
using AbyssalRose.Services;
using static AbyssalRose.Extensions.ManliExtensions;

namespace AbyssalRose.Controllers
{
    public class GuildHallUpgradesController : Controller
    {
        // GET: GuildHallUpgrades
        public ActionResult Index()
        {
            GuildHallUpgradeViewModel gvm = new GuildHallUpgradeViewModel();
            gvm.guildHallUpgrades = GW2API.GetGuildUpgrades().OrderBy(x => x.Name).ToList();

            if (ConfigHelper.Instance.FocusedUpgradeID > 0)
            {
                gvm.focusedUpgrade = gvm.guildHallUpgrades.Where(x => x.UpgradeID == ConfigHelper.Instance.FocusedUpgradeID).First();
                GW2API.AddIconsToRequiredMaterials(ref gvm.focusedUpgrade.RequiredMaterials);
                GW2API.AddGuildStashAmountToMaterials(ref gvm.focusedUpgrade.RequiredMaterials);
            }
            else
            {
                gvm.focusedUpgrade = new Models.GuildHallUpgradeModels.GuildHallUpgrade();

            }

            return View(gvm);
        }

        [HttpPost]
        [Roles(AbyssalUser.ROLE_ARCHITECT, AbyssalUser.ROLE_LEADER)]
        public ActionResult SetFocusedUpgrade(int upgradeId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ConfigHelper.SetFocusedUpgradeID(upgradeId);

            return RedirectToAction("Index");
        }
    }
}