using System.Collections.Generic;

namespace MekanRehberi
{
    public class UserManager
    {
        public static List<User> AllUsers { get; set; }= new List<User>();
        
        public static User CurrentUser { get; set; } = null;

        public static string Register(User newUser)
        {
            if (string.IsNullOrWhiteSpace(newUser.Password) || string.IsNullOrWhiteSpace(newUser.Nickname))
            {
                return "Kullanıcı adı veya şifre boş olamaz.";
            }
            else
            {
                foreach (var user in AllUsers)
                {
                    if (user.Nickname == newUser.Nickname)
                    {
                        return "Aynı kullanıcı adına sahip bir kullanıcı zaten bulunmakta.";
                    }
                }
                AllUsers.Add(newUser);
                return "Kayıt başarılı";
            }
        }

        public static User Login(string username, string password)
        {
            foreach (var user in AllUsers) 
            {
                if (user.Nickname == username && user.Password == password)
                {
                    CurrentUser = user;
                    return user;
                }
            }
            return null;
        }
    }
}