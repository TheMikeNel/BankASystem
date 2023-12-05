using System.Collections.ObjectModel;
using System.Windows;

namespace BankASystem
{
    // Общий класс для всех сотрудников
    internal abstract class Employee : DataRepository
    {
        public abstract ObservableCollection<Client> GetAllClients();

        public new abstract ObservableCollection<Client> FindClients(string fio, string phoneNumber);

        public new bool ChangeClient(Client defaultClient, Client changedClient)
        {
            return DataRepository.ChangeClient(defaultClient, changedClient);
        }

        public new bool RemoveClient(Client client)
        {
            return DataRepository.RemoveClient(client);
        }
    }

    // Класс менеджера
    internal class Manager : Employee, IManagerWorkingWithRepository
    {
        private string _managerPassword = "1111";

        public string ManagerPassword
        {
            get => _managerPassword;
            set
            {
                _managerPassword = value;
            }
        }

        public Manager(string password)
        {
            ManagerPassword = password;
        }

        public override ObservableCollection<Client> GetAllClients()
        {
            ObservableCollection<Client> clientsCollection = new ObservableCollection<Client>(ClientsList);
            return clientsCollection;
        }

        public override ObservableCollection<Client> FindClients(string fio, string phoneNumber)
        {
            ObservableCollection<Client> foundClients = new ObservableCollection<Client>(
                DataRepository.FindClients(fio, phoneNumber));

            return foundClients;
        }

        public new void AddClient(Client client)
        {
            DataRepository.AddClient(client);
        }
    }

    // Класс консультанта
    internal class Consultant : Employee, IConsultantWorkingWithRepository
    {
        public override ObservableCollection<Client> GetAllClients()
        {
            ObservableCollection<Client> clientsList = new ObservableCollection<Client>(ClientsList);

            for (int i = 0; i < clientsList.Count; i++)
            {
                Client client = clientsList[i];
                client.Passport = "**** ******";
                clientsList[i] = client;
            }
            return clientsList;
        }

        public override ObservableCollection<Client> FindClients(string fio, string phoneNumber)
        {
            ObservableCollection<Client> foundClients = new ObservableCollection<Client>(
                DataRepository.FindClients(fio, phoneNumber));

            if (foundClients.Count > 0)
            {
                for (int i = 0; i < foundClients.Count; i++)
                {
                    Client client = foundClients[i];
                    client.Passport = "**** ******";
                    foundClients[i] = client;
                }
            }

            return foundClients;
        }
    }
}
