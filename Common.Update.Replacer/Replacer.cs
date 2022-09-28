using System.Collections.Generic;
using System.IO;

namespace Common.Update.Replacer
{
    public class Replacer
    {
        private static string _rootDir = string.Empty;

        private static string _newFilesDir = string.Empty;

        public Replacer()
        {

        }

        /// <summary>
        /// 设置新文件目录
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>替换器</returns>
        public Replacer SetSourceDir(string path)
        {
            _newFilesDir = path;
            return this;
        }

        /// <summary>
        /// 设置根目录
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>替换器</returns>
        public Replacer SetRootDir(string path)
        {
            _rootDir = path;
            return this;
        }

        /// <summary>
        /// 扫描文件
        /// </summary>
        public void Scan(string root, string dir, ref List<string> path)
        {
            DirectoryInfo dirinfo = new DirectoryInfo(dir);
            foreach (var item in dirinfo.GetFiles())
                path.Add(Path.GetRelativePath(Path.GetFullPath(root), item.FullName));
            foreach (var item in dirinfo.GetDirectories())
                Scan(root, item.FullName, ref path);
        }

        /// <summary>
        /// 替换旧文件
        /// </summary>
        public void Replace()
        {
            List<string> newFiles = new List<string>();
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

