using System;
using System.Collections.Generic;
using BookManage.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;

namespace BookManage.Services
{
    public class BookManagementService
    {
        private readonly ISqlService _sqlService;

        public BookManagementService(ISqlService sqlService)
        {
            _sqlService = sqlService;
        }

        public string IntiDatabase()
        {
            var conn = _sqlService.GetConnection();
            try
            {
                conn.Open();
                var sql = @"create table if not exists books(
bid int unsigned auto_increment,
class varchar(20),
title varchar(20) not null,
publish varchar(20),
pubyear int,
author varchar(20),
price decimal(10, 2),
total int not null,
stock int not null,
primary key(bid)
) default CHARSET=utf8;";
                var cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                
                sql = @"create table if not exists cards(
cid int unsigned auto_increment,
name varchar(20) not null,
company varchar(20),
class varchar(20) not null,
password varchar(20) not null,
primary key(cid)
) default CHARSET=utf8;";
                cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                sql = @"create table if not exists records(
rid int unsigned auto_increment,
cid int unsigned not null,
bid int unsigned not null,
borrow date,
back date,
primary key(rid),
foreign key(cid) references cards(cid) on delete cascade ,
foreign key(bid) references books(bid) on delete cascade 
) default CHARSET=utf8;";
                cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                conn.Close();
            }

            return "Initialize Database";
        }

        public Book AddBook(Book book)
        {
            var conn = _sqlService.GetConnection();
            try
            {
                conn.Open();
                var sql = @"insert into books (class, title, publish, pubyear, author, price, total, stock)
                values (@CLASS, @TITLE, @PUBLISH, @PUBYEAR, @AUTHOR, @PRICE, @TOTAL, @STOCK);";
                var cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@CLASS", book.Class);
                cmd.Parameters.AddWithValue("@TITLE", book.Title);
                cmd.Parameters.AddWithValue("@PUBLISH", book.Publish);
                cmd.Parameters.AddWithValue("@PUBYEAR", book.Pubyear);
                cmd.Parameters.AddWithValue("@AUTHOR", book.Author);
                cmd.Parameters.AddWithValue("@PRICE", book.Price);
                cmd.Parameters.AddWithValue("@TOTAL", book.Total);
                cmd.Parameters.AddWithValue("@STOCK", book.Stock);

                cmd.ExecuteNonQuery();
                var curIDsql = "select max(bid) from books;";
                var curIDCmd = new MySqlCommand(curIDsql, conn);
                using MySqlDataReader rdr = curIDCmd.ExecuteReader();

                while (rdr.Read())
                {
                    book.id = rdr.GetInt32(0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                conn.Close();
            }

            return book;
        }

        public int AddCard(Person p)
        {
            var conn = _sqlService.GetConnection();
            int ans = -1;
            try
            {
                conn.Open();
                var sql = @"insert into cards (name, company, class, password)
                values (@NAME, @COMPANY, @CLASS, @PASSWORD);";
                var cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@CLASS", p.Class);
                cmd.Parameters.AddWithValue("@NAME", p.Name);
                cmd.Parameters.AddWithValue("@COMPANY", p.Company);
                cmd.Parameters.AddWithValue("@PASSWORD", p.Password);

                cmd.ExecuteNonQuery();

                var curIDsql = "select max(cid) from cards;";
                var curIDCmd = new MySqlCommand(curIDsql, conn);
                using MySqlDataReader rdr = curIDCmd.ExecuteReader();

                while (rdr.Read())
                {
                    ans = rdr.GetInt32(0);
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                conn.Close();
            }

            return ans;
        } 

        public string DelCard(int id)
        {
            var conn = _sqlService.GetConnection();
            try
            {
                conn.Open();
                var sql = @"delete from cards 
                where cards.cid=@ID;";
                var cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ID", id);

                cmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                conn.Close();
            }

            return "Delete card successfully";
        } 
        public List<Book> GetBook(Query q)
        {
            var conn = _sqlService.GetConnection();
            List<Book> ans = new List<Book>();
            try
            {
                conn.Open();

                string query = "";
                bool firstQ = false;

                if (q.Class != null)
                {
                    query += "books.class='" + q.Class + "'";
                    firstQ = true;
                }

                if (q.Title != null)
                {
                    if (firstQ) 
                        query += " and books.title='" + q.Title + "'";
                    else
                    {
                        query += "books.title='" + q.Title + "'";
                        firstQ = true;
                    }
                }

                if (q.Publish != null)
                {
                    if (firstQ)
                    {
                        query += " and books.publish='" + q.Publish + "'";
                    }
                    else
                    {
                        query += "books.publish='" + q.Publish + "'";
                        firstQ = true;
                    }
                }

                if (q.Pubyear.Item1 != -1)
                {
                    if (firstQ)
                    {
                        query += " and books.pubyear>=" + q.Pubyear.Item1.ToString();
                    }
                    else
                    {
                        query += "books.pubyear>=" + q.Pubyear.Item1.ToString();
                        firstQ = true;
                    }
                }

                if (q.Pubyear.Item2 != 3000)
                {
                    if (firstQ)
                    {
                        query += " and books.pubyear<=" + q.Pubyear.Item2.ToString();
                    }
                    else
                    {
                        query += "books.pubyear<=" + q.Pubyear.Item2.ToString();
                        firstQ = true;
                    }
                }

                if (q.Author != null)
                {
                    if (firstQ)
                    {
                        query += " and books.author='" + q.Author + "'";
                    }
                    else
                    {
                        query += "books.author='" + q.Author + "'";
                        firstQ = true;
                    }
                }

                if (q.Price.Item1 > -1)
                {
                    if (firstQ)
                    {
                        query += " and books.pubyear>=" + q.Price.Item1.ToString();
                    }
                    else
                    {
                        query += "books.pubyear>=" + q.Price.Item1.ToString();
                        firstQ = true;
                    }
                }

                if (q.Price.Item2 < 3000)
                {
                    if (firstQ)
                    {
                        query += " and books.price<=" + q.Price.Item2.ToString();
                    }
                    else
                    {
                        query += "books.price<=" + q.Price.Item2.ToString();
                    }
                }

                var sql = @"select * from books where " + query + ";";
                using var cmd = new MySqlCommand(sql, conn);
                // cmd.Parameters.AddWithValue("@QUERY", query);
                using MySqlDataReader rdr = cmd.ExecuteReader();
                
                Console.WriteLine(sql);
                Console.WriteLine(query);

                
                while (rdr.Read())
                {
                    ans.Add(new Book(
                        rdr.GetString(1), 
                        rdr.GetString(2), 
                        rdr.GetString(3),
                        rdr.GetInt32(4),
                        rdr.GetString(5),
                        rdr.GetDouble(6),
                        rdr.GetInt32(7),
                        rdr.GetInt32(8)));
                    ans[^1].id = rdr.GetInt32(0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                conn.Close();
            }

            return ans;
        }
        public Book GetBook(int bid)
        {
            var conn = _sqlService.GetConnection();
            Book ans = new Book();
            try
            {
                conn.Open();

                var sql = @"select * from books where bid=@ID;";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ID", bid);
                using MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    ans = new Book(
                        rdr.GetString(1), 
                        rdr.GetString(2), 
                        rdr.GetString(3),
                        rdr.GetInt32(4),
                        rdr.GetString(5),
                        rdr.GetDouble(6),
                        rdr.GetInt32(7),
                        rdr.GetInt32(8));
                    ans.id = rdr.GetInt32(0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                conn.Close();
            }

            return ans;
        }

        public List<Book> GetBorrowed(int cid)
        {
            var conn = _sqlService.GetConnection();
            List<Book> ans = new List<Book>();
            try
            {
                conn.Open();

                var sql = @"select * 
from books 
where books.bid in (select bid 
from records
where records.cid=@ID);";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ID", cid.ToString());
                using MySqlDataReader rdr = cmd.ExecuteReader();

                
                while (rdr.Read())
                {
                    ans.Add(new Book(
                        rdr.GetString(1), 
                        rdr.GetString(2), 
                        rdr.GetString(3),
                        rdr.GetInt32(4),
                        rdr.GetString(5),
                        rdr.GetDouble(6),
                        rdr.GetInt32(7),
                        rdr.GetInt32(8)));
                    ans[^1].id = rdr.GetInt32(0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                conn.Close();
            }

            return ans;
        }

        public Person GetPersonInfo(int cid)
        {
            var conn = _sqlService.GetConnection();
            Person ans = new Person();
            try
            {
                conn.Open();

                var sql = @"select * 
from cards 
where cards.cid=@ID;";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ID", cid.ToString());
                using MySqlDataReader rdr = cmd.ExecuteReader();

                
                while (rdr.Read())
                {
                    ans.id = rdr.GetInt32(0);
                    ans.Name = rdr.GetString(1);
                    ans.Company = rdr.GetString(2);
                    ans.Class = rdr.GetString(3);
                    ans.Password = rdr.GetString(4);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                conn.Close();
            }

            return ans;
        }

        public bool IsEmpty(int bid)
        {
            var conn = _sqlService.GetConnection();
            bool ans = true;
            try
            {
                conn.Open();

                var sql = @"select * 
from books 
where books.bid=@ID;";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ID", bid.ToString());
                using MySqlDataReader rdr = cmd.ExecuteReader();

                
                while (rdr.Read())
                {
                    ans = (rdr.GetInt32(8) <= 0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                conn.Close();
            }

            return ans;
        }

        public String GetReturnDate(int bid)
        {
            var conn = _sqlService.GetConnection();
            String ans = "";
            try
            {
                conn.Open();

                var sql = @"select min(records.back)
from records 
where records.bid=@ID;";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ID", bid.ToString());
                using MySqlDataReader rdr = cmd.ExecuteReader();

                
                while (rdr.Read())
                {
                    ans = (rdr.GetString(4));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                conn.Close();
            }

            return ans;
        }
        
        public int IsBorrowed(int bid, int cid)
        {
            var conn = _sqlService.GetConnection();
            int ans = -1;
            try
            {
                conn.Open();

                var sql = @"select * 
from records 
where records.bid=@BID and records.cid=@CID;";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@BID", bid.ToString());
                cmd.Parameters.AddWithValue("@CID", cid.ToString());
                using MySqlDataReader rdr = cmd.ExecuteReader();

                
                while (rdr.Read())
                {
                    ans = rdr.GetInt32(0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                conn.Close();
            }

            return ans;
        }
        
        public string Borrow(int bid, int cid)
        {
            var conn = _sqlService.GetConnection();
            // string ans = "";
            try
            {
                conn.Open();

                var sql = @"insert into records (cid, bid, borrow, back) values (@CID, @BID, CURDATE(), date_add(CURDATE(), interval 1 MONTH) );";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@BID", bid.ToString());
                cmd.Parameters.AddWithValue("@CID", cid.ToString());
                
                cmd.ExecuteNonQuery();

                var stockSql = @"update books set stock=stock-1 where bid=@BID";
                using var stockCmd = new MySqlCommand(stockSql, conn);
                stockCmd.Parameters.AddWithValue("@BID", bid.ToString());
                stockCmd.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                conn.Close();
            }

            return "Borrow successfully";
        }
        
        public string Return(int rid)
        {
            var conn = _sqlService.GetConnection();
            try
            {
                conn.Open();

                var sql = @"delete from records where rid=@RID";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@RID", rid.ToString());
                
                cmd.ExecuteNonQuery();

                var stockSql = @"update books set stock=stock+1 where bid in (select bid from records where rid=@RID);";
                using var stockCmd = new MySqlCommand(stockSql, conn);
                stockCmd.Parameters.AddWithValue("@RID", rid.ToString());
                stockCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                conn.Close();
            }

            return "Return successfully";
        }
        
    }
}
