
namespace SQLiteTestDb
{
    public class CreateEmptyDb
    {
        public static void CreateTestDb(string connectionString)
        {
            const string sql = @"
                CREATE TABLE 'tblBlogs' ('id' INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL ,                                          
                                         'url' TEXT, 
                                         'name' TEXT, 
                                         'thumbnail' BLOB,
                                         'NumberOfPosts' INTEGER,
                                         'AddDate' DATETIME);

                CREATE TABLE 'tblReports' ('id' INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL ,                                          
                                         'PdfRpt' BLOB,
                                         'AddDate' DATETIME);

                CREATE TABLE 'tblParents' ('Id' INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL,                                          
                                           'BirthDate' DATETIME,
                                           'Name' TEXT,
                                           'LastName' TEXT);
                CREATE TABLE 'tblKids' ('Id' INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL,      
                                        'ParentId' INTEGER,
                                        'BirthDate' DATETIME,                                    
                                        'Name' TEXT);
            ";

            SQLiteDbHelper.ExecSQL(connectionString, sql);
        }
    }
}
