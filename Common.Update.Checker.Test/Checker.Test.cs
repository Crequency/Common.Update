namespace Common.Update.Checker.Test
{
    [TestClass]
    public class Checker_Test
    {
        [TestMethod]
        public void ≤‚ ‘“ªœ¬()
        {
            Checker checker = new Checker()
                .SetRootDirectory(@"D:\tmp\")
                .AppendIgnoreFormat(".zip");
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