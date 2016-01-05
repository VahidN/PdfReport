using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace SQLiteTestDb
{
    public class FillDb
    {
        public static void AddSomeParentKidsTestRecords(string connectionString)
        {
            addSomeParents(connectionString);
            addSomeKids(connectionString);
        }

        private static void addSomeKids(string connectionString)
        {
            using (SQLiteConnection myConnection = new SQLiteConnection())
            {
                myConnection.ConnectionString = connectionString;
                myConnection.Open();

                using (SQLiteTransaction myTransaction = myConnection.BeginTransaction())
                {
                    using (SQLiteCommand myCommand = new SQLiteCommand(myConnection))
                    {
                        var rnd = new Random();
                        for (int i = 1; i < 100; i++)
                        {
                            myCommand.CommandText = @"insert into tblKids(ParentId, BirthDate, Name) 
                                                                  values(@ParentId, @BirthDate, @Name)";
                            myCommand.Parameters.AddWithValue("@Name", "Kid" + i);
                            myCommand.Parameters.AddWithValue("@ParentId", rnd.Next(1, 11));
                            myCommand.Parameters.AddWithValue("@BirthDate", DateTime.Now.AddYears(-i));

                            myCommand.ExecuteNonQuery();
                        }
                    }
                    myTransaction.Commit();
                }
            }
        }

        private static void addSomeParents(string connectionString)
        {
            using (SQLiteConnection myConnection = new SQLiteConnection())
            {
                myConnection.ConnectionString = connectionString;
                myConnection.Open();

                using (SQLiteTransaction myTransaction = myConnection.BeginTransaction())
                {
                    using (SQLiteCommand myCommand = new SQLiteCommand(myConnection))
                    {
                        for (int i = 1; i < 11; i++)
                        {
                            myCommand.CommandText = @"insert into tblParents(BirthDate, Name, LastName) 
                                                                  values(@BirthDate, @Name, @LastNam)";
                            myCommand.Parameters.AddWithValue("@Name", "Parent" + i);
                            myCommand.Parameters.AddWithValue("@LastNam", "LM" + i);
                            myCommand.Parameters.AddWithValue("@BirthDate", DateTime.Now.AddYears(-i * 10));

                            myCommand.ExecuteNonQuery();
                        }
                    }
                    myTransaction.Commit();
                }
            }
        }

        public static void AddSomeTblBlogsTestRecords(string connectionString, IList<string> imagesPath)
        {
            using (SQLiteConnection myConnection = new SQLiteConnection())
            {
                myConnection.ConnectionString = connectionString;
                myConnection.Open();

                using (SQLiteTransaction myTransaction = myConnection.BeginTransaction())
                {
                    using (SQLiteCommand myCommand = new SQLiteCommand(myConnection))
                    {
                        foreach (var itemPath in imagesPath)
                        {
                            myCommand.CommandText = @"insert into tblBlogs(url, name, thumbnail, NumberOfPosts, AddDate) 
                                                                  values(@url, @name, @thumbnail, @NumberOfPosts, @AddDate)";
                            var name = Path.GetFileNameWithoutExtension(itemPath);
                            myCommand.Parameters.AddWithValue("@url", "www.blog" + name + ".com");
                            myCommand.Parameters.AddWithValue("@name", "blog" + name);
                            var data = File.ReadAllBytes(itemPath);
                            myCommand.Parameters.AddWithValue("@thumbnail", data);
                            myCommand.Parameters.AddWithValue("@NumberOfPosts", 10);
                            myCommand.Parameters.AddWithValue("@AddDate", DateTime.Now);

                            myCommand.ExecuteNonQuery();
                        }
                    }
                    myTransaction.Commit();
                }
            }
        }
    }
}
