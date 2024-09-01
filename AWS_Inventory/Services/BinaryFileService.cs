using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Services
{
    public class BinaryFileService
    {
        public async Task SaveToBinaryFileAsync<T>(List<T> data, string filePath) where T : class
        {
            await using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                var writer = new BinaryWriter(stream);
                writer.Write(data.Count);
                foreach (var item in data)
                {
                    writer.Write(item.ToString()); // Предполагается, что у типа T есть метод ToString() для сериализации в строку
                }
            }
        }

        public async Task<List<T>> LoadFromBinaryFileAsync<T>(string filePath, Func<string, T> factory) where T : class
        {
            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            var result = new List<T>();

            await using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var reader = new BinaryReader(stream);
                var count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    var serializedItem = reader.ReadString();
                    result.Add(factory(serializedItem));
                }
            }

            return result;
        }
    }
}
