using System.Data.SQLite;

namespace SQLiteTestDb
{
    public class SQLiteDbHelper
    {
        public static void ExecSQL(string connectionString, string sql)
        {
            using (var myConnection = new SQLiteConnection())
            {
                myConnection.ConnectionString = connectionString;
                myConnection.Open();

                using (var myTransaction = myConnection.BeginTransaction())
                {
                    using (var myCommand = new SQLiteCommand(myConnection))
                    {
                        myCommand.CommandText = sql;
                        myCommand.ExecuteNonQuery();
                    }
                    myTransaction.Commit();
                }
            }
        }
    }
}
