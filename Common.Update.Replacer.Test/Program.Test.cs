using Common.Update.Replacer;
using System.Diagnostics;

namespace Common.Update.Replacer.Test
{
    [TestClass]
    public class Program_Test
    {
        [TestMethod]
        public void ������������()
        {
            Process.Start("./Common.Update.Replacer.exe", "--test-arg");
        }
    }
}