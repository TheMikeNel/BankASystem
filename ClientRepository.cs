using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace BankASystem
{
    internal static class ClientRepository
    {
        private static string xmlPath = "ClientsRepository.xml";
        private static bool isFirstOpen = true;
        private static List<Client> clientsList = new List<Client>();

        public static void SerializeClientsList(List<Client> clients)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Client>));
            StreamWriter sw = new StreamWriter(xmlPath);
            serializer.Serialize(sw, clients);
            sw.Close();
        }

        public static List<Client> DeserializeClientsList()
        {
            isFirstOpen = false;

            if (File.Exists(xmlPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Client>));
                StreamReader sr = new StreamReader(xmlPath);
                clientsList = serializer.Deserialize(sr) as List<Client>;
                sr.Close();
            }
            return clientsList;
        }

        public static void AddClient(Client client)
        {
            if (isFirstOpen) DeserializeClientsList();

            clientsList.Add(client);

            SerializeClientsList(clientsList);
        }

        public static List<Client> FindClients(string searchByFIO, string searchByNumber)
        {
            if (isFirstOpen) DeserializeClientsList();

            List<Client> foundClients = new List<Client>();

            foreach (Client client in clientsList)
            {
                if (client.ToString().Contains(searchByFIO) && client.ToString().Contains(searchByNumber)) 
                    foundClients.Add(client);
            }
            return foundClients;
        }
    }
}
