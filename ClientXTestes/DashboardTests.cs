using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static ClientX.Dashboard;
using ClientX;

namespace ClientXTestes
{
    [TestClass] 
    public class DashboardTests 
    {
        [TestMethod]
        public void validateFreight()
        {

            Dashboard dashboard = new Dashboard();

            string provided = "0";
            decimal expected = 1001;

            decimal actual = (decimal)dashboard.ValidateFreight(provided);

            Assert.AreEqual(expected, actual, "Values are not equal.");
        }
    }
}
