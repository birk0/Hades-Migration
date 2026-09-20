using HadesWeb.Services;

namespace HadesWeb.Tests.Services
{
    public static class PasswordManagerTests
    {
        [Test]
        public static void Test_ValidatePassword()
        {
            string hash = PasswordManager.HashPassword("pass");
            Assert.That(PasswordManager.ValidatePassword("pass", hash), Is.True);
        }
    }
}