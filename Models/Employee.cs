﻿using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;

namespace BankASystem.Models
{
    // Общий абстрактный класс для всех сотрудников
    internal abstract class Employee : DataRepository
    {
        /// <summary>
        /// Получить список всех клиентов.
        /// </summary>
        /// <returns></returns>
        public abstract ObservableCollection<Client> GetAllClients();

        /// <summary>
        /// Поиск клиента по ФИО и номеру телефона.
        /// </summary>
        /// <param name="fio">ФИО</param>
        /// <param name="phoneNumber">Номер телефона</param>
        /// <returns></returns>
        public new abstract ObservableCollection<Client> FindClients(string fio, string phoneNumber);

        /// <summary>
        /// Удалить клиента.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public new bool RemoveClient(Client client)
        {
            return DataRepository.RemoveClient(client);
        }

        /// <summary>
        /// Изменить данные клиента
        /// </summary>
        /// <param name="defaultClient">Начальные данные клиента, данные которого будут изменены.</param>
        /// <param name="changedClient">Измененный клиент</param>
        /// <returns></returns>
        protected new bool ChangeClient(Client defaultClient, Client changedClient) // Не абстрактный, так как общий для всех дочерних классов
        {
            List<Change> changesList = new List<Change>();
            StringBuilder changes = new StringBuilder();

            if (changedClient.FIO != defaultClient.FIO)
            {
                changesList.Add(new Change(defaultClient.FIO, changedClient.FIO));
                changes.Append(" -ФИО- ");
            }
            if (changedClient.PhoneNumber != defaultClient.PhoneNumber)
            {
                changesList.Add(new Change(defaultClient.PhoneNumber, changedClient.PhoneNumber));
                changes.Append(" -Номер телефона- ");
            }
            if (changedClient.Passport != defaultClient.Passport)
            {
                changesList.Add(new Change(defaultClient.Passport, changedClient.Passport));
                changes.Append(" -Паспорт- ");
            }

            changedClient.AddChangesData(new ChangesData(changesList, changes.ToString(), this.ToString()));
            return DataRepository.ChangeClient(defaultClient, changedClient);
        }
    }

    // Класс менеджера
    internal class Manager : Employee, IManager
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

        public new bool AddClient(Client client)
        {
            client.ChangesHistory.Clear();
            client.AddChangesData(new ChangesData("Создание", this.ToString()));
            return DataRepository.AddClient(client);
        }

        public new bool ChangeClient(Client defaultClient, Client changeClient)
        {
            return base.ChangeClient(defaultClient, changeClient);
        }

        public override string ToString() => "Менеджер";
    }

    // Класс консультанта
    internal class Consultant : Employee, IConsultant
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

        public new bool ChangeClient(Client defaultClient, Client changeClient)
        {
            return base.ChangeClient(defaultClient, changeClient);
        }

        public override string ToString() => "Консультант";
    }
}
