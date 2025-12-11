using System;
using System.Collections.Generic;

namespace MekanRehberi
{
    public class UserManager
    {
        // Sadece giriş yapmış kullanıcıyı hafızada tutuyoruz
        public static User CurrentUser { get; set; } = null;

        public static string Register(User newUser)
        {
            if (string.IsNullOrWhiteSpace(newUser.Password) || string.IsNullOrWhiteSpace(newUser.Nickname))
            {
                return "Kullanıcı adı veya şifre boş olamaz.";
            }

            // Doğrudan veritabanına kayıt isteği gönderiyoruz
            // "AllUsers" listesine eklemeye gerek yok, veritabanına ekliyoruz.
            return DataManagement.AddUserToDb(newUser);
        }

        public static User Login(string username, string password)
        {
            // Veritabanından kullanıcıyı sorgula
            User user = DataManagement.GetUserFromDb(username, password);

            if (user != null)
            {
                CurrentUser = user;

                // Kullanıcı giriş yapınca eski puanlarını yükle
                DataManagement.LoadUserRatings(CurrentUser);
                
                return user;
            }

            return null; // Kullanıcı bulunamadı veya şifre yanlış
        }
        
        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}
