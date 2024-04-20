using BankASystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BankASystem.Services
{
    internal class Serializer<TData> where TData : class
    {
        public void SerializeData(TData data, string xmlPath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TData));
            StreamWriter streamWriter = new StreamWriter(xmlPath);
            serializer.Serialize(streamWriter, data);
            streamWriter.Close();
        }

        public bool TryDeserializeData(out TData outData, string xmlPath)
        {
            if (File.Exists(xmlPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TData));
                StreamReader sr = new StreamReader(xmlPath);
                outData = serializer.Deserialize(sr) as TData;
                sr.Close();
                return true;
            }
            else
            {
                outData = default;
                return false;
            }
        }
    }
}
