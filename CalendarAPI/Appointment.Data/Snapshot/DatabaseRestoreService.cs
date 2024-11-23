using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Data.Snapshot
{
    public class DatabaseRestoreService : IDatabaseRestoreService
    {
        public bool Restore()
        {
            var databaseName = "appointment";
            SqlConnectionStringBuilder connectionBuilder = new("Data Source = (LocalDb)\\LocalDB; Database = appointment; Integrated Security = sspi;")
            { InitialCatalog = "master" };

            using(var conn = new SqlConnection(connectionBuilder.ConnectionString))
            {
                conn.Open();
                using var cmd = conn.CreateCommand();
                cmd.CommandText =
$@"ALTER DATABASE {databaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; 
RESTORE DATABASE {databaseName} FROM DATABASE_SNAPSHOT = '{databaseName}_Snapshot'; 
ALTER DATABASE {databaseName} SET MULTI_USER;";
                cmd.ExecuteNonQuery();
            }
            return true;
        }
    }
}
