namespace Common.Update.Checker.Test
{
    [TestClass]
    public class Checker_Test
    {
        [TestMethod]
        public void Test_Checker_基础功能()
        {
            string root = @"D:\tmp\";

            Checker checker = new Checker()
                .SetRootDirectory(root)
                .AppendIgnoreFormat(".zip")
                .AppendIgnoreFolder("ignore")
                .AppendIncludeFile($"{root}/test.kxp");
            _ = checker.AppendIgnoreFormats(new()
            {
                ".mp4", ".mp3", ".jpg", ".png", ".xml", ".kxp"
            });

            checker.PrintIgnoredFormats(Console.Out);

            checker.Scan();

            checker.PrintScannedFiles(Console.Out);

            checker.Calculate();

            checker.PrintFilesHashed(Console.Out);
        }

        [TestMethod]
        public void 专项测试()
        {
            string wd = @"E:\Development\Projects\Crequency\KitX\KitX Build\Dashboard\Debug\net6.0";
            string ld = $"{wd}/Languages";
            Checker checker = new Checker()
                .SetRootDirectory(wd)
                .AppendIgnoreFolder("Config")
                .AppendIgnoreFolder("Languages")
                .AppendIgnoreFolder("Log")
                .AppendIncludeFile($"{ld}/zh-cn.axaml")
                .AppendIncludeFile($"{ld}/zh-cnt.axaml")
                .AppendIncludeFile($"{ld}/en-us.axaml")
                .AppendIncludeFile($"{ld}/ja-jp.axaml");
            checker.Scan();

            checker.PrintScannedFiles(Console.Out);

            checker.Calculate();

            checker.PrintFilesHashed(Console.Out);
        }
    }
}