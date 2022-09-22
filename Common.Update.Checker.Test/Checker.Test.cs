namespace Common.Update.Checker.Test
{
    [TestClass]
    public class Checker_Test
    {
        [TestMethod]
        public void ≤‚ ‘“ªœ¬()
        {
            string root = @"D:\tmp\";

            Checker checker = new Checker()
                .SetRootDirectory(root)
                .AppendIgnoreFormat(".zip")
                .AppendIncludeFile($"{root}/test.kxp");
            checker.AppendIgnoreFormats(new()
            {
                ".mp4", ".mp3", ".jpg", ".png", ".xml", ".kxp"
            });

            checker.PrintIgnoredFormats(Console.Out);

            checker.Scan();

            checker.PrintScannedFiles(Console.Out);

            checker.Calculate();

            checker.PrintFilesHashed(Console.Out);
        }
    }
}