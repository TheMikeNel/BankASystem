﻿using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;

namespace BankASystem.Models
{
    // Общий абстрактный класс для всех сотрудников
    internal abstract class Employee : ClientsDataRepository
    {
        /// <summary>
        /// Получить список всех клиентов.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Client> GetAllClients()
        {
            ObservableCollection<Client> clientsCollection = new ObservableCollection<Client>(ClientsList);
            return clientsCollection;
        }

        /// <summary>
        /// Поиск клиента по ФИО и номеру телефона.
        /// </summary>
        /// <param name="fio">ФИО</param>
        /// <param name="phoneNumber">Номер телефона</param>
        /// <returns></returns>
        public new ObservableCollection<Client> FindClients(string fio, string phoneNumber)
        {
            ObservableCollection<Client> foundClients = new ObservableCollection<Client>(
                ClientsDataRepository.FindClients(fio, phoneNumber));

            return foundClients;
        }

        /// <summary>
        /// Удалить клиента.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public new bool RemoveClient(Client client)
        {
            return ClientsDataRepository.RemoveClient(client);
        }

        /// <summary>
        /// Изменить данные клиента
        /// </summary>
        /// <param name="defaultClient">Начальные данные клиента, данные которого будут изменены.</param>
        /// <param name="changedClient">Измененный клиент</param>
        /// <returns></returns>
        public bool ChangeClient(Client defaultClient, Client changedClient)
        {
            List<Change> changesList = new List<Change>();
            StringBuilder changes = new StringBuilder();

            if (changedClient.FIO != defaultClient.FIO)
            {
                changesList.Add(new Change(defaultClient.FIO, changedClient.FIO));
                changes.Append("ФИО\t");
            }
            if (changedClient.PhoneNumber != defaultClient.PhoneNumber)
            {
                changesList.Add(new Change(defaultClient.PhoneNumber, changedClient.PhoneNumber));
                changes.Append("Номер телефона\t");
            }
            if (changedClient.Passport != defaultClient.Passport)
            {
                changesList.Add(new Change(defaultClient.Passport, changedClient.Passport));
                changes.Append("Паспорт\t");
            }

            return ClientsDataRepository.ChangeClient(defaultClient, changedClient, new ChangesData(changesList, changes.ToString(), this.ToString()));
        }
    }

    // Класс менеджера
    internal class Manager : Employee
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


        public new bool AddClient(Client client)
        {
            client.ChangesHistory.Clear();
            client.AddChangesData(new ChangesData("Создание", this.ToString()));
            return ClientsDataRepository.AddClient(client);
        }

        public override string ToString() => "Менеджер";
    }

    // Класс консультанта
    internal class Consultant : Employee
    {
        public override string ToString() => "Консультант";
    }
}
