namespace MangaMate
{
    public static class UserContext
    {
        public static int Id { get; set; } 
        public static string Login { get; set; } = String.Empty;
        public static string Email { get; set; } = String.Empty;
        public static byte[]? Avatar { get; set; }
    }
}
