using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestJob4
{
    internal class DataModel
    {
        /// <summary>
        /// Список клиентов.
        /// </summary>
        public List<Client> Clients = new List<Client>();
        /// <summary>
        /// График платежей.
        /// </summary>
        public List<PaymentSchedule> Schedules = new List<PaymentSchedule>();
        /// <summary>
        /// Список платежей.
        /// </summary>
        public List<Payment> Payments = new List<Payment>();
    }

    /// <summary>
    /// Клиент
    /// </summary>
    internal class Client
    {
        public Client() {}

        public Client(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }

    }

    /// <summary>
    /// Запись графика платежей
    /// </summary>
    internal class PaymentSchedule
    {
        public PaymentSchedule(int id, int clientId, DateTime date, double amount)
        {
            Id = id;
            ClientId = clientId;
            Date = date;
            Amount = amount;
        }

        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ИД клиента
        /// </summary>
        public int ClientId { get; set; }
        /// <summary>
        /// Палановая дата
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Плановая сумма
        /// </summary>
        public double Amount { get; set; }
    }

    /// <summary>
    /// Падатеж.
    /// </summary>
    internal class Payment
    {
        public Payment() {}

        public Payment(int id, int clientId, DateTime date, double amount, string status, double balance)
        {
            Id = id;
            ClientId = clientId;
            Date = date;
            Amount = amount;
            Status = status;
            Balance = balance;
        }

        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ИД клиента
        /// </summary>
        public int ClientId { get; set; }
        /// <summary>
        /// Фактическая дата
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Фактическая сумма
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// Статус
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Балданс
        /// </summary>
        public double Balance { get; set; }
    }


}
