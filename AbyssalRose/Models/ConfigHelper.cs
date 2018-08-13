using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AbyssalRose.Models
{
    public sealed class ConfigHelper
    {
        private static Config instance = null;
        private static readonly object padlock = new object();

        ConfigHelper()
        {
        }

        public static Config Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ApplicationDbContext().Config.Where(c => c.ConfigName == "Active").First();
                    }
                    return instance;
                }
            }
        }

        internal static void SaveChanges(ApplicationDbContext db)
        {
            db.SaveChanges();
            instance = db.Config.Where(x => x.ConfigName == "Active").First();
        }

        internal static void SetFocusedUpgradeID(int upgradeId)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            db.Config.Where(x => x.ConfigName == "Active").First().FocusedUpgradeID = upgradeId;
            SaveChanges(db);
        }
    }
}