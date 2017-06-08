using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbuseFunction
{
    class DbContext : DataContext
    {
        public Table<UserProfile> UserProfiles;
        public DbContext(SqlConnection connection) : base(connection) { }
    }
}
