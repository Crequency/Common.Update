﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Common.Update.Checker
{
    public class Checker
    {
        private readonly List<string> _ignoreformat = new List<string>();

        private readonly List<string> _includefile = new List<string>();

        private readonly List<string> _ignorefolder = new List<string>();

        private readonly Dictionary<string, string> _hash_md5 = new Dictionary<string, string>();

        private readonly Dictionary<string, string> _hash_sha1 = new Dictionary<string, string>();

        private string _rootDir = string.Empty;

        private bool _lock = false;

        public Checker()
        {

        }

        /// <summary>
        /// 追加忽略的格式
        /// </summary>
        /// <param name="format">格式, 形如 .txt</param>
        /// <returns>检查器</returns>
        public Checker AppendIgnoreFormat(string format)
        {
            if (!_ignoreformat.Contains(format)) _ignoreformat.Add(format.ToLower());
            return this;
        }

        /// <summary>
        /// 追加一系列忽略的格式
        /// </summary>
        /// <param name="formats">格式们</param>
        public Checker AppendIgnoreFormats(List<string> formats)
        {
            foreach (string format in formats) _ = AppendIgnoreFormat(format);
            return this;
        }

        /// <summary>
        /// 追加额外包含的文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>检查器</returns>
        public Checker AppendIncludeFile(string path)
        {
            string rele = Path.GetRelativePath(_rootDir, Path.GetFullPath(path));
            if (!_includefile.Contains(rele)) _includefile.Add(rele);
            return this;
        }

        /// <summary>
        /// 追加忽略的文件夹及其子文件夹和子文件
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <returns>检查器</returns>
        public Checker AppendIgnoreFolder(string path)
        {
            string fpath = Path.GetFullPath($"{_rootDir}/{path}");
            if (!_ignorefolder.Contains(fpath)) _ignorefolder.Add(fpath);
            return this;
        }

        /// <summary>
        /// 设置根目录
        /// </summary>
        /// <param name="RootDirectory">根目录路径</param>
        /// <returns>检查器</returns>
        public Checker SetRootDirectory(string RootDirectory)
        {
            _rootDir = Path.GetFullPath(RootDirectory);
            return this;
        }

        /// <summary>
        /// 锁定自己
        /// </summary>
        /// <returns>检查器</returns>
        public Checker Lock()
        {
            _lock = true;
            return this;
        }

        /// <summary>
        /// 解锁
        /// </summary>
        /// <returns>检查器</returns>
        public Checker Unlock()
        {
            _hash_md5.Clear();
            _hash_sha1.Clear();
            _includefile.Clear();
            _lock = false;
            return this;
        }

        /// <summary>
        /// 打印忽略的格式列表
        /// </summary>
        /// <param name="stream">输出流</param>
        public void PrintIgnoredFormats(TextWriter stream)
        {
            foreach (var item in _ignoreformat)
                stream.WriteLine(item);
            stream.Flush();
        }

        /// <summary>
        /// 打印扫描到的文件
        /// </summary>
        /// <param name="stream">输出流</param>
        public void PrintScannedFiles(TextWriter stream)
        {
            foreach (var item in _includefile)
                stream.WriteLine(item);
            stream.Flush();
        }

        /// <summary>
        /// 打印所有文件的散列值
        /// </summary>
        /// <param name="stream">输出流</param>
        public void PrintFilesHashed(TextWriter stream)
        {
            foreach (var item in _includefile)
            {
                stream.WriteLine($"File Path: {item}");
                stream.WriteLine($"\t MD5:  {_hash_md5[item]}");
                stream.WriteLine($"\t SHA1: {_hash_sha1[item]}");
                stream.WriteLine();
            }
            stream.Flush();
        }

        /// <summary>
        /// 获取计算结果
        /// </summary>
        /// <returns>键: 文件相对路径, 值: 元组 - 前者为 MD5 值 - 后者为 SHA1 值</returns>
        public Dictionary<string, (string, string)> GetCalculateResult()
        {
            Dictionary<string, (string, string)> result = new Dictionary<string, (string, string)>();
            foreach (var item in _includefile)
                result.Add(item, (_hash_md5[item], _hash_sha1[item]));
            return result;
        }

        /// <summary>
        /// 开始扫描
        /// </summary>
        public void Scan() => ScanFolder(_rootDir);

        /// <summary>
        /// 扫描文件夹并追加文件
        /// </summary>
        /// <param name="folder">文件夹路径</param>
        private void ScanFolder(string folder)
        {
            DirectoryInfo dir = new DirectoryInfo(folder);
            foreach (var item in dir.GetFiles())
            {
                if (!_ignoreformat.Contains(Path.GetExtension(item.FullName).ToLower()))
                {
                    string path = Path.GetRelativePath(_rootDir, item.FullName);
                    if (!_includefile.Contains(path))
                        _includefile.Add(path);
                }
            }
            foreach (var item in dir.GetDirectories())
            {
                if (_ignorefolder.Contains(Path.GetFullPath(item.FullName))) continue;
                ScanFolder(item.FullName);
            }
        }

        /// <summary>
        /// 获取 MD5 值
        /// </summary>
        /// <param name="bytes">要计算的数据</param>
        /// <returns>MD5 值</returns>
        private static string GetMD5(byte[] bytes)
        {
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(bytes);
            md5.Dispose();
            return Encoding.UTF8.GetString(result);
        }

        /// <summary>
        /// 获取 SHA1 值
        /// </summary>
        /// <param name="bytes">计算对象</param>
        /// <returns>SHA1 值</returns>
        private static string GetSHA1(byte[] bytes)
        {
            using SHA1Managed sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(bytes);
            return Encoding.UTF8.GetString(hash);
        }

        private int finished_count = 0;

        /// <summary>
        /// 计算散列值
        /// </summary>
        public void Calculate()
        {
            if (_lock) throw new Exception("Can't calculate when locked.");

            //  4 个文件一组进行异步计算
            for (int i = 0; i < _includefile.Count; i += 4)
                CalcRegion(i, Math.Min(i + 4, _includefile.Count));
            //  剩余文件单独一组计算
            CalcRegion(_includefile.Count - (_includefile.Count % 4), _includefile.Count);

            while (finished_count != _includefile.Count) { }

            _ = Lock();
        }

        /// <summary>
        /// 计算某个区间
        /// </summary>
        /// <param name="s">起始下标</param>
        /// <param name="e">终止于e下标之前</param>
        private void CalcRegion(int s, int e)
        {
            List<string> task = new List<string>();
            for (int j = s; j < e; ++j)
                task.Add(_includefile[j]);
            new Thread(() =>
            {
                foreach (var item in task)
                {
                    byte[] bytes = File.ReadAllBytes(Path.GetFullPath($"{_rootDir}/{item}"));
                    _hash_md5.Add(item, GetMD5(bytes));
                    _hash_sha1.Add(item, GetSHA1(bytes));
                    Interlocked.Increment(ref finished_count);
                }
            }).Start();
        }
    }
}
