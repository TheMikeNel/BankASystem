using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace BankASystem
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

            if (TryAddClient(client))
            {
                ClientsList.Add(client);

                return true;
            }

            return false;
        }

        protected static bool ChangeClient(Client defaultClient, Client changedClient)
        {
            // Для изменения данных о клиенте, необходимо найти его в общем списке.
            // Так как номер телефона, теоретически, не может повторяться, поиск производится по номеру телефона
            int clientInd = ClientsList.FindIndex(x => x.PhoneNumber == defaultClient.PhoneNumber);

            if (clientInd > -1 && TryChangeClientData(defaultClient, changedClient))
            {
                // Если изменения вносит консультант (паспортные данные указаны как "**** ******"), паспортные данные не изменяются.
                if (changedClient.Passport.Contains('*')) changedClient.Passport = ClientsList[clientInd].Passport; 

                ClientsList[clientInd] = changedClient;

                return true;
            }
            return false;
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

        /// <summary>
        /// Проверка на повторение индивидуальных данных при изменении конкретного клиента. 
        /// </summary>
        /// <param name="defaultClient"></param>
        /// <param name="changedClient"></param>
        /// <returns>falce - если данные повторяются / true - если нет</returns>
        private static bool TryChangeClientData(Client defaultClient, Client changedClient)
        {
            // Сравнение происходит по списку с исключенным defaultClient
            List<Client> clients = new List<Client>(ClientsList);
            int clientInd = clients.FindIndex(x => x.PhoneNumber == defaultClient.PhoneNumber);

            if (clientInd > -1) clients.RemoveAt(clientInd);
            else return true;

            foreach (Client client in clients)
            {
                if (changedClient.PhoneNumber == client.PhoneNumber || changedClient.Passport == client.Passport)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Проверка на повторение индивидуальных данных при добавлении конкретного клиента.
        /// </summary>
        /// <param name="tClient"></param>
        /// <returns>falce - если данные повторяются / true - если нет</returns>
        private static bool TryAddClient(Client tClient)
        {
            foreach (Client client in ClientsList)
            {
                if (tClient.PhoneNumber == client.PhoneNumber || tClient.Passport == client.Passport)
                {
                    return false;
                }
            }

            return true;
        }
    }

    interface IConsultant
    {
        ObservableCollection<Client> GetAllClients();
        ObservableCollection<Client> FindClients(string fio, string phoneNumber);
        bool ChangeClient(Client defoultClient, Client changedClient);
    }

    interface IManager : IConsultant
    {
        string ManagerPassword { get; set; }

        bool AddClient(Client client);
        bool RemoveClient(Client client);
    }

}
