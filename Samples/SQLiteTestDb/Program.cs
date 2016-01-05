using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SQLiteTestDb
{
    class Program
    {
        static void Main()
        {
            var dbPath = System.IO.Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "data\\blogs.sqlite");
            if (File.Exists(dbPath))
                File.Delete(dbPath);

            string connectionStr = "Data Source=" + dbPath;
            CreateEmptyDb.CreateTestDb(connectionStr);
            FillDb.AddSomeTblBlogsTestRecords(connectionStr,
                new List<string>
                {
                     System.IO.Path.Combine(Application.StartupPath , "Images\\01.png"),
                     System.IO.Path.Combine(Application.StartupPath , "Images\\02.png"),
                     System.IO.Path.Combine(Application.StartupPath , "Images\\03.png"),
                     System.IO.Path.Combine(Application.StartupPath , "Images\\04.png")
                });
            FillDb.AddSomeParentKidsTestRecords(connectionStr);
        }
    }
}
