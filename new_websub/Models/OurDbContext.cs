using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

namespace new_websub.Models
{
    public class OurDbContext: DbContext
    {
        public DbSet<UserAccount> userAccounts { get; set; }
        //public DbSet<Subscription> subscriptions { get; set; }
        public DbSet<AnAddress> addresses { get; set; }
    }
}
