using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.Data
{
    public class DataBase
    {
        private string _connectionString;


        public DataBase(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<Ad> GetAllAds()
        {


            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();


            cmd.CommandText = @"SELECT a.*, u.Name FROM Ads a
                                JOIN Users u
                                ON u.Id=a.UserId
                                ORDER BY a.Date DESC";

            connection.Open();



            List<Ad> ads = new();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Date = (DateTime)reader["Date"],
                    UserId = (int)reader["UserId"],
                    Description = (string)reader["Description"],
                    Name = (string)reader["Name"]



                });

            }
            return ads;
        }


        public void AddUser(User user, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Name, Email, PasswordHash) " +
                "VALUES (@name, @email, @hash)";
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@hash", BCrypt.Net.BCrypt.HashPassword(password));

            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isValid ? user : null;

        }
        private User GetByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT TOP 1 * FROM Users WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = (int)reader["Id"],
                Email = (string)reader["Email"],
                Name = (string)reader["Name"],
                PasswordHash = (string)reader["PasswordHash"]
            };
        }
        public List<int> GetListOfAdIdsByEmail(string email)
        {


            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();


            cmd.CommandText = @"SELECT a.Id FROM Ads a
                                    JOIN Users u
                                    ON u.Id=a.UserId
                                    Where u.Email=@email";
            cmd.Parameters.AddWithValue("@email", email);

            connection.Open();



            List<int> ads = new();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add((int)reader["Id"]);

            }
            return ads;
        }

        public void DeleteAd(int id)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM Ads WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();

        }
        public List<Ad> GetAdsForUser(string email)
        {


            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();


            cmd.CommandText = @"SELECT a.*, u.Name FROM Ads a
                                JOIN Users u
                                ON u.Id=a.UserId
                                WHERE u.Email = @email
                                ORDER BY a.Date DESC";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();



            List<Ad> ads = new();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Date = (DateTime)reader["Date"],
                    UserId = (int)reader["UserId"],
                    Description = (string)reader["Description"],
                    Name = (string)reader["Name"]



                });

            }
            return ads;
        }
        public int AddAd(Ad ad,string email)
        {

            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = conn.CreateCommand();


            cmd.CommandText = "INSERT INTO Ads ( UserId, PhoneNumber,  Description, Date) " +
               "VALUES ( @userId, @number,@ad, @date) SELECT SCOPE_IDENTITY()";

           


            cmd.Parameters.AddWithValue("@userId", GetUserIdByEmail(email));
            cmd.Parameters.AddWithValue("@number", ad.PhoneNumber);
            cmd.Parameters.AddWithValue("@ad", ad.Description);
            cmd.Parameters.AddWithValue("@date", DateTime.Now);


            conn.Open();

            return (int)(Decimal)cmd.ExecuteScalar();
        }

        private int GetUserIdByEmail(string email)
        {


            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand cmd = connection.CreateCommand();


            cmd.CommandText = @"SELECT Id FROM Users
                                WHERE Email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();



            List<Ad> ads = new();
            return (int)cmd.ExecuteScalar();

        }
    }
}