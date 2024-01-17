using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace BankASystem.Models
{
    internal abstract class DataRepository
    {
        private static readonly string _xmlPath = "ClientsRepository.xml";
        private static bool _isFirstOpen = true;

        private static List<Client> _clientsList = new List<Client>();

        protected static List<Client> ClientsList
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

        public static void SerializeClientsList()
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
                if (client.FIO.ToLower().Contains(fio.ToLower()) && client.PhoneNumber.Contains(phoneNumber))
                    foundClients.Add(client);
            }
            return foundClients;
        }

        /// <summary>
        /// Добавление нового клиента.
        /// </summary>
        /// <param name="client"></param>
        /// <returns>true - если успешно добавлен / falce - если данные повторяются</returns>
        protected static bool AddClient(Client client)
        {
            if (_isFirstOpen) DeserializeClientsList();

            if (!CheckRepetitions(client))
            {
                _clientsList.Add(client);

                return true;
            }

            return false;
        }

        protected static bool ChangeClient(Client defaultClient, Client changedClient, ChangesData changesData)
        {
            if (!CheckRepetitions(changedClient, true, defaultClient))
            {
                defaultClient.FIO = changedClient.FIO;
                defaultClient.PhoneNumber = changedClient.PhoneNumber;
                defaultClient.Passport = changedClient.Passport;
                defaultClient.AddChangesData(changesData);
                return true;
            }
            return false;
        }

        protected static bool RemoveClient(Client client) => _clientsList.Remove(client);

        /// <summary>
        /// Проверка на повторение индивидуальных данных при добавлении / изменении данных клиента.
        /// </summary>
        /// <param name="vCient">Проверяемый клиент</param>
        /// <param name="isChanging">Добавление клиента / Изменение данных (Добавление по умолчанию - falce)</param>
        /// <param name="defaultChangingClient">Ссылка на клиента, данные которого будут изменены</param>
        /// <returns>true - если данные повторяются / falce - если нет</returns>
        private static bool CheckRepetitions(Client vCient, bool isChanging = false, Client defaultChangingClient = null)
        {
            foreach (Client client in ClientsList)
            {
                if (isChanging && defaultChangingClient != null && defaultChangingClient == client) continue;

                if (vCient.PhoneNumber == client.PhoneNumber || vCient.Passport == client.Passport) return true;
            }
            return false;
        }
    }
}
