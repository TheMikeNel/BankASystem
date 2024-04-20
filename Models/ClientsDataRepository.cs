using BankASystem.Services;
using System.Collections.Generic;

namespace BankASystem.Models
{
    internal abstract class ClientsDataRepository
    {
        private static readonly string _xmlPath = "ClientsRepository.xml";
        private static bool _isInit = false;
        private static Serializer<List<Client>> _serializer = new Serializer<List<Client>>();
        private static List<Client> _clientsList = new List<Client>();
        protected static List<Client> ClientsList
        {
            get
            {
                if (!_isInit)
                {
                    if (!_serializer.TryDeserializeData(out _clientsList, _xmlPath)) _clientsList = new List<Client>();
                    _isInit = true;
                }
                return _clientsList;
            }

            private set 
            {
                _clientsList = value; 
            }
        }

        public static void SaveClientsList()
        {
            _serializer.SerializeData(ClientsList, _xmlPath);
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
            if (!CheckRepetitions(client))
            {
                ClientsList.Add(client);

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
