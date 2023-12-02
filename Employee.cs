using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankASystem
{
    static class Employee
    {
        public static bool IsManager { get; set; }

        public static ObservableCollection<Client> ClientsCollection
        {
            get
            {
                Client[] clients = ClientRepository.DeserializeClientsList().ToArray();

                if (!IsManager)
                {
                    for (int i = 0; i < clients.Length; i++)
                    {
                        clients[i].Passport = "**** ******";
                    }
                }
                ObservableCollection<Client> clientsCollection = new ObservableCollection<Client>(clients);
                return clientsCollection;
            }
        }

        public static string password = "1111";
    }
}
