namespace Common.Update.Replacer
{
    public class Replacer
    {
        private static string _rootDir = string.Empty;

        private static string _newFilesDir = string.Empty;

        /// <summary>
        /// 换个颜色执行
        /// </summary>
        /// <param name="cc">新颜色</param>
        /// <param name="action">动作</param>
        private static void DoColor(ConsoleColor cc, Action action)
        {
            ConsoleColor now = Console.ForegroundColor;
            Console.ForegroundColor = cc;
            action();
            Console.ForegroundColor = now;
        }

        public static void Main(string[] args)
        {
            try
            {

                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--source-dir":
                            if (i != args.Length - 1)
                            {
                                ++i;
                                _rootDir = args[i];
                            }
                            else throw new Exception("参数 --source-dir 缺少值");
                            break;
                        case "--update-from":
                            if (i != args.Length - 1)
                            {
                                ++i;
                                _newFilesDir = args[i];
                            }
                            else throw new Exception("参数 --update-from 缺少值");
                            break;
                        case "--test-arg":
                            PrintArguments(Console.Out);
                            break;
                    }
                }

                Replace();
            }
            catch(Exception e)
            {
                DoColor(ConsoleColor.Red, new(() =>
                {
                    Console.WriteLine(e.Message);
                }));
            }
        }

        /// <summary>
        /// 打印启动参数
        /// </summary>
        /// <param name="stream">输出流</param>
        private static void PrintArguments(TextWriter stream)
        {
            stream.WriteLine($"--source-dir:  {_rootDir}");
            stream.WriteLine($"--update-from: {_newFilesDir}");
        }

        /// <summary>
        /// 扫描文件
        /// </summary>
        private static void Scan(string root, string dir, ref List<string> path)
        {
            DirectoryInfo dirinfo = new(dir);
            foreach (var item in dirinfo.GetFiles())
                path.Add(Path.GetRelativePath(Path.GetFullPath(root), item.FullName));
            foreach (var item in dirinfo.GetDirectories())
                Scan(root, item.FullName, ref path);
        }

        /// <summary>
        /// 替换旧文件
        /// </summary>
        private static void Replace()
        {
            List<string> newFiles = new();
            Scan(_newFilesDir, _newFilesDir, ref newFiles);
            foreach (var item in newFiles)
            {
                string newPath = Path.GetFullPath($"{_rootDir}/{item}");
                if (File.Exists(newPath))
                    File.Delete(newPath);
                File.Copy(Path.GetFullPath($"{_newFilesDir}/{item}"), newPath);
            }
        }
    }
}

