using System;
using System.Collections.Generic;

namespace MekanRehberi
{
    public class UserManager
    {
        // Anlık giriş yapmış kullanıcıyı tutar
        public static User CurrentUser { get; set; } = null;

        public static string Register(User newUser)
        {
            if (string.IsNullOrWhiteSpace(newUser.Password) || string.IsNullOrWhiteSpace(newUser.Nickname))
            {
                return "Kullanıcı adı veya şifre boş olamaz.";
            }

            // Doğrudan veritabanına kayıt isteği gönderiyoruz
            return DataManagement.AddUserToDb(newUser);
        }

        public static User Login(string username, string password)
        {
            // Veritabanından kullanıcıyı sorgula
            User user = DataManagement.GetUserFromDb(username, password);

            if (user != null)
            {
                // Giriş başarılı, global değişkene ata
                CurrentUser = user;

                // Kullanıcının eski puanlarını ve yorumlarını yükle
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
