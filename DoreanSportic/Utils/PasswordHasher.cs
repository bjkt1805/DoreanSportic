namespace DoreanSportic.Web.Utils
{
    public static class PasswordHasher
    {
        // Esta clase utiliza BCrypt para hashear y verificar contraseñas.
        public static string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        // Verifica si la contraseña proporcionada coincide con el hash almacenado.
        public static bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
