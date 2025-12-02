using System;
using System.IO;
using System.IO.Compression;

namespace Script
{
    public static class ZipUtils
    {
        // 服务器使用，客户端不使用
        public static byte[] ZipBytes(byte[] buffer, int offset, int length)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    ZipArchiveEntry entry = archive.CreateEntry("0", CompressionLevel.Optimal);
                    using (var stream = entry.Open())
                    {
                        stream.Write(buffer, offset, length);
                    }
                }

                var bytes = new byte[memoryStream.Length];
                memoryStream.Seek(0, SeekOrigin.Begin);

                memoryStream.Read(bytes, 0, (int)memoryStream.Length);
                return bytes;
            }
        }

        // 服务器使用，客户端不使用
        public static void UnzipInMemory(string path, Func<string, bool> shouldOpen, Action<string, byte[]> onContent)
        {
            using (var file = File.OpenRead(path))
            {
                using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in zip.Entries)
                    {
                        if (!shouldOpen(entry.FullName))
                        {
                            continue;
                        }

                        using (BinaryReader reader = new BinaryReader(entry.Open()))
                        {
                            byte[] buffer = reader.ReadBytes((int)entry.Length);
                            onContent(entry.FullName, buffer);
                        }
                    }
                }
            }
        }
    }
}