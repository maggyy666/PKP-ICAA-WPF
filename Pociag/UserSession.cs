namespace Pociag
{
    class UserSession
    {
        public static int UserId { get; set; }
        public static string Username { get; set; } = "Guest";
        public static string Email { get; set; } = string.Empty;
        public static string Password { get; set; } = string.Empty;
        public static string SelectedDiscount { get; set; } = string.Empty;
        public static int DiscountId { get; set; }

        public static bool IsLoggedIn { get; set; } = false;

        public static void Login(int userId, string username, string email, string password, int discountId, string selectedDiscount)
        {
            UserId = userId;
            Username = username;
            Email = email;
            Password = password;
            DiscountId = discountId;
            SelectedDiscount = selectedDiscount;
            IsLoggedIn = true;
        }

        public static void Logout()
        {
            UserId = -1;
            Username = "Guest";
            Email = string.Empty;
            Password = string.Empty;
            SelectedDiscount = string.Empty;
            DiscountId = -1;
            IsLoggedIn = false;
        }
    }
}
