using Common.Update.Replacer;
using System.Diagnostics;

namespace Common.Update.Replacer.Test
{
    [TestClass]
    public class Program_Test
    {
        [TestMethod]
        public void 测试启动参数()
        {
            Process.Start("./Common.Update.Replacer.exe", "--test-arg");
        }
    }
}