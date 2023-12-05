using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace BankASystem
{
    internal abstract class DataRepository
    {
        private static readonly string _xmlPath = "ClientsRepository.xml";
        private static bool _isFirstOpen = true;

        private static List<Client> _clientsList = new List<Client>();

        public static List<Client> ClientsList
        {
            get
            {
                if (_isFirstOpen) DeserializeClientsList();
                return _clientsList;
            }

            private set 
            {
                if (_isFirstOpen) DeserializeClientsList();
                _clientsList = value; 
            }
        }

        private static void SerializeClientsList()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Client>));
            StreamWriter sw = new StreamWriter(_xmlPath);
            serializer.Serialize(sw, ClientsList);
            sw.Close();
        }

        private static void DeserializeClientsList()
        {
            _isFirstOpen = false;

            if (File.Exists(_xmlPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Client>));
                StreamReader sr = new StreamReader(_xmlPath);
                ClientsList = serializer.Deserialize(sr) as List<Client>;
                sr.Close();
            }
        }

        protected static List<Client> FindClients(string fio, string phoneNumber)
        {
            List<Client> foundClients = new List<Client>();

            foreach (Client client in ClientsList)
            {
                if (client.ToString().Contains(fio) && client.ToString().Contains(phoneNumber))
                    foundClients.Add(client);
            }
            return foundClients;
        }

        protected static void AddClient(Client client)
        {
            if (_isFirstOpen) DeserializeClientsList();

            ClientsList.Add(client);

            SerializeClientsList();
        }

        protected static bool ChangeClient(Client defaultClient, Client changedClient)
        {
            int clientInd = ClientsList.FindIndex(x => x.PhoneNumber == defaultClient.PhoneNumber);

            if (clientInd > -1)
            {
                // Если изменения вносит консультант (паспортные данные указаны как "**** ******"), паспортные данные не изменяются.
                if (changedClient.Passport.Contains('*')) changedClient.Passport = ClientsList[clientInd].Passport; 

                ClientsList[clientInd] = changedClient;
                SerializeClientsList();
                return true;
            }
            else return false;
        }

        protected static bool RemoveClient(Client client)
        {
            for (int i = 0; i < _clientsList.Count; i++)
            {
                if (_clientsList[i].PhoneNumber == client.PhoneNumber)
                {
                    _clientsList.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
    }

    interface IManagerWorkingWithRepository
    {
        string ManagerPassword { get; set; }

        ObservableCollection<Client> GetAllClients();
        ObservableCollection<Client> FindClients(string fio, string phoneNumber);
        void AddClient(Client client);
        bool ChangeClient(Client defoultClient, Client changedClient);
        bool RemoveClient(Client client);
    }

    interface IConsultantWorkingWithRepository
    {
        ObservableCollection<Client> GetAllClients();
        ObservableCollection<Client> FindClients(string fio, string phoneNumber);
        bool ChangeClient(Client defoultClient, Client changedClient);
    }
}
