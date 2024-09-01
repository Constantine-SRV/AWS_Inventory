using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Services
{// not in use in the version
    public class XMLService
    {
        public async Task SaveToXMLAsync<T>(List<T> data, string filePath) where T : class
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));

            var xmlSerializer = new XmlSerializer(typeof(List<T>));

            await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                xmlSerializer.Serialize(fileStream, data);
            }
        }

        public async Task<List<T>> LoadFromXMLAsync<T>(string filePath) where T : class
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));

            var xmlSerializer = new XmlSerializer(typeof(List<T>));

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return (List<T>)xmlSerializer.Deserialize(fileStream);
            }
        }
    }
}
