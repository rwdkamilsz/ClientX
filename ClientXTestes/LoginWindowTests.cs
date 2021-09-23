using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ClientX;
namespace ClientXTestes
{
    [TestClass]
    public class LoginWindowTests
    {
        [TestMethod]
        public void TestLoginValidation()
        {
            LoginWindow login = new LoginWindow();
            bool expected = true;
            string user = "user";
            string password = "user";

            bool actual = login.checkCredentials(user, password);

            Assert.AreEqual(expected, actual);
        }
    }
}
