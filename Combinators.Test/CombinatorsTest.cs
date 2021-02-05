using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Combinators.Test
{
    [TestClass]
    public class CombinatorsTest
    {
        [TestMethod]
        public void TestIdentity()
        {
            Assert.AreEqual(3, 3.Thrush(Combinators.Identity));
        }
    }
}