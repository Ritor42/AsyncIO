﻿// <copyright file="AsyncFile.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AsyncIO.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using CsvHelper;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Provides features for file handling.
    /// </summary>
    public class AsyncFile
    {
        private readonly Conversions conversions;
        private readonly Transaction transaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncFile"/> class.
        /// </summary>
        /// <param name="conversions">Needed conversion features.</param>
        /// <param name="transaction">Needed transaction features.</param>
        internal AsyncFile(Conversions conversions, Transaction transaction)
        {
            this.conversions = conversions ?? throw new ArgumentNullException(nameof(conversions));
            this.transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        /// <summary>
        /// Reads file's content and deserializes as Json.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="path">File path.</param>
        /// <returns>Deserialized item.</returns>
        public T ReadJson<T>(string path)
        {
            return this.conversions.FromJson<T>(File.ReadAllText(path));
        }

        /// <summary>
        /// Reads file's content async and deserializes as Json.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="path">File path.</param>
        /// <returns>Task with deserialized item.</returns>
        public async Task<T> ReadJsonAsync<T>(string path)
        {
            return this.conversions.FromJson<T>(await File.ReadAllTextAsync(path).ConfigureAwait(false));
        }

        /// <summary>
        /// Reads file's content and deserializes as Bson.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="path">File path.</param>
        /// <returns>Deserialized item.</returns>
        public T ReadBson<T>(string path)
        {
            return this.conversions.FromBson<T>(File.ReadAllBytes(path));
        }

        /// <summary>
        /// Reads file's content async and deserializes as Bson.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="path">File path.</param>
        /// <returns>Task with deserialized item.</returns>
        public async Task<T> ReadBsonAsync<T>(string path)
        {
            return this.conversions.FromBson<T>(await File.ReadAllBytesAsync(path).ConfigureAwait(false));
        }

        /// <summary>
        /// Reads file's content and deserializes as Xml.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="path">File path.</param>
        /// <returns>Deserialized item.</returns>
        public T ReadXml<T>(string path)
            where T : class
        {
            return this.conversions.FromXml<T>(File.ReadAllText(path));
        }

        /// <summary>
        /// Reads file's content async and deserializes as Xml.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="path">File path.</param>
        /// <returns>Task with deserialized item.</returns>
        public async Task<T> ReadXmlAsync<T>(string path)
            where T : class
        {
            return this.conversions.FromXml<T>(await File.ReadAllTextAsync(path).ConfigureAwait(false));
        }

        /// <summary>
        /// Reads file's content and deserializes as Csv.
        /// </summary>
        /// <typeparam name="T">Items type.</typeparam>
        /// <param name="path">File path.</param>
        /// <returns>Deserialized items.</returns>
        public IEnumerable<T> ReadCsv<T>(string path)
        {
            foreach (T item in this.conversions.FromCsv<T>(File.ReadAllText(path)))
            {
                yield return item;
            }
        }

        /// <summary>
        /// Reads file's content async and deserializes as Csv.
        /// </summary>
        /// <typeparam name="T">Item type.</typeparam>
        /// <param name="path">File path.</param>
        /// <returns>Task with deserialized items.</returns>
        public async Task<IEnumerable<T>> ReadCsvAsync<T>(string path)
        {
            return this.conversions.FromCsv<T>(await File.ReadAllTextAsync(path).ConfigureAwait(false));
        }

        /// <summary>
        /// Writes file with serialized content as Json.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="item">Item to be serialized.</param>
        public void WriteJson(string path, object item)
        {
            var targetFolder = Path.GetDirectoryName(path);
            Directory.CreateDirectory(targetFolder);
            File.WriteAllText(path, this.conversions.ToJson(item));
            this.HandleTransaction(path);
        }

        /// <summary>
        /// Writes file async with serialized content as Json.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="item">Item to be serialized.</param>
        /// <returns>Task.</returns>
        public async Task WriteJsonAsync(string path, object item)
        {
            var targetFolder = Path.GetDirectoryName(path);
            Directory.CreateDirectory(targetFolder);
            await File.WriteAllTextAsync(path, this.conversions.ToJson(item)).ConfigureAwait(false);
            this.HandleTransaction(path);
        }

        /// <summary>
        /// Writes file with serialized content as Bson.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="item">Item to be serialized.</param>
        public void WriteBson(string path, object item)
        {
            var targetFolder = Path.GetDirectoryName(path);
            Directory.CreateDirectory(targetFolder);
            File.WriteAllBytes(path, this.conversions.ToBson(item));
            this.HandleTransaction(path);
        }

        /// <summary>
        /// Writes file async with serialized content as Bson.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="item">Item to be serialized.</param>
        /// <returns>Task.</returns>
        public async Task WriteBsonAsync(string path, object item)
        {
            var targetFolder = Path.GetDirectoryName(path);
            Directory.CreateDirectory(targetFolder);
            await File.WriteAllBytesAsync(path, this.conversions.ToBson(item)).ConfigureAwait(false);
            this.HandleTransaction(path);
        }

        /// <summary>
        /// Writes file with serialized content as Xml.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="item">Item to be serialized.</param>
        public void WriteXml(string path, object item)
        {
            var targetFolder = Path.GetDirectoryName(path);
            Directory.CreateDirectory(targetFolder);
            File.WriteAllText(path, this.conversions.ToXml(item));
            this.HandleTransaction(path);
        }

        /// <summary>
        /// Writes file async with serialized content as Xml.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="item">Item to be serialized.</param>
        /// <returns>Task.</returns>
        public async Task WriteXmlAsync(string path, object item)
        {
            var targetFolder = Path.GetDirectoryName(path);
            Directory.CreateDirectory(targetFolder);
            await File.WriteAllTextAsync(path, this.conversions.ToXml(item)).ConfigureAwait(false);
            this.HandleTransaction(path);
        }

        /// <summary>
        /// Writes file with serialized content as Csv.
        /// </summary>
        /// <typeparam name="T">Items' type.</typeparam>
        /// <param name="path">File path.</param>
        /// <param name="items">Items to be serialized.</param>
        public void WriteCsv<T>(string path, IEnumerable<T> items)
        {
            var targetFolder = Path.GetDirectoryName(path);
            Directory.CreateDirectory(targetFolder);
            File.WriteAllText(path, this.conversions.ToCsv(items));
            this.HandleTransaction(path);
        }

        /// <summary>
        /// Writes file async with serialized content as Csv.
        /// </summary>
        /// <typeparam name="T">Items' type.</typeparam>
        /// <param name="path">File path.</param>
        /// <param name="items">Items to be serialized.</param>
        /// <returns>Task.</returns>
        public async Task WriteCsvAsync<T>(string path, IEnumerable<T> items)
        {
            var targetFolder = Path.GetDirectoryName(path);
            Directory.CreateDirectory(targetFolder);
            await File.WriteAllTextAsync(path, this.conversions.ToCsv(items)).ConfigureAwait(false);
            this.HandleTransaction(path);
        }

        /// <summary>
        /// Copies file disallowing overwriting.
        /// </summary>
        /// <param name="sourcePath">Source directory path.</param>
        /// <param name="targetPath">Target directory path.</param>
        /// <param name="overwrite">Overwrite existing file.</param>
        public void Copy(string sourcePath, string targetPath, bool overwrite = false)
        {
            var targetFolder = Path.GetDirectoryName(targetPath);
            Directory.CreateDirectory(targetFolder);
            if (!overwrite && File.Exists(targetPath))
            {
                throw new IOException("Cannot create a file when that file already exists");
            }

            File.WriteAllBytes(targetPath, File.ReadAllBytes(sourcePath));
            this.HandleTransaction(targetPath);
        }

        /// <summary>
        /// Copies file async disallowing overwriting.
        /// </summary>
        /// <param name="sourcePath">Source directory path.</param>
        /// <param name="targetPath">Target directory path.</param>
        /// <param name="overwrite">Overwrite existing file.</param>
        /// <returns>Task.</returns>
        public async Task CopyAsync(string sourcePath, string targetPath, bool overwrite = false)
        {
            var targetFolder = Path.GetDirectoryName(targetPath);
            Directory.CreateDirectory(targetFolder);
            if (!overwrite && File.Exists(targetPath))
            {
                throw new IOException("Cannot create a file when that file already exists");
            }

            await File.WriteAllBytesAsync(targetPath, await File.ReadAllBytesAsync(sourcePath).ConfigureAwait(false)).ConfigureAwait(false);
            this.HandleTransaction(targetPath);
        }

        /// <summary>
        /// Copies file with given buffer length disallowing overwriting.
        /// </summary>
        /// <param name="sourcePath">Source directory path.</param>
        /// <param name="targetPath">Target directory path.</param>
        /// <param name="bufferLength">Buffer length.</param>
        /// <param name="overwrite">Overwrite existing file.</param>
        public void Copy(string sourcePath, string targetPath, int bufferLength, bool overwrite = false)
        {
            var targetFolder = Path.GetDirectoryName(targetPath);
            Directory.CreateDirectory(targetFolder);
            using (var sourceStream = new FileStream(sourcePath, FileMode.Open))
            {
                var fileMode = overwrite ? FileMode.Create : FileMode.CreateNew;
                using (var destinationStream = new FileStream(targetPath, fileMode))
                {
                    this.HandleTransaction(targetPath);
                    var buffer = new byte[bufferLength];
                    int readCount = sourceStream.Read(buffer, 0, bufferLength);
                    while (readCount != 0)
                    {
                        destinationStream.Write(buffer, 0, readCount);
                        readCount = sourceStream.Read(buffer, 0, bufferLength);
                    }
                }
            }
        }

        /// <summary>
        /// Copies file async with given buffer length disallowing overwriting.
        /// </summary>
        /// <param name="sourcePath">Source directory path.</param>
        /// <param name="targetPath">Target directory path.</param>
        /// <param name="bufferLength">Buffer length.</param>
        /// <param name="overwrite">Overwrite existing file.</param>
        /// <returns>Task.</returns>
        public async Task CopyAsync(string sourcePath, string targetPath, int bufferLength, bool overwrite = false)
        {
            var targetFolder = Path.GetDirectoryName(targetPath);
            Directory.CreateDirectory(targetFolder);
            using (var sourceStream = new FileStream(sourcePath, FileMode.Open))
            {
                var fileMode = overwrite ? FileMode.Create : FileMode.CreateNew;
                using (var destinationStream = new FileStream(targetPath, fileMode))
                {
                    this.HandleTransaction(targetPath);
                    var buffer = new byte[bufferLength];
                    int readCount = await sourceStream.ReadAsync(buffer, 0, bufferLength).ConfigureAwait(false);
                    while (readCount != 0)
                    {
                        await destinationStream.WriteAsync(buffer, 0, readCount).ConfigureAwait(false);
                        readCount = await sourceStream.ReadAsync(buffer, 0, bufferLength).ConfigureAwait(false);
                    }
                }
            }
        }

        private void HandleTransaction(string path, [CallerMemberName] string caller = null)
        {
            var folder = Path.GetDirectoryName(path);
            this.transaction.PushUndoAction(
            () =>
            {
                if (Directory.Exists(folder))
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    if (!Directory.EnumerateFileSystemEntries(folder).Any())
                    {
                        Directory.Delete(folder);
                    }
                }
            }, caller);
        }
    }
}
