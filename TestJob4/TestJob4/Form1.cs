using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestJob4
{
    public partial class Form1 : Form
    {

        #region Методы формы

        public Form1()
        {
            InitializeComponent();
            FillTestData();
        }

        private DataModel TestDataModel = new DataModel();

        private void button1_Click(object sender, EventArgs e)
        {
            int clientId = 0;
            if (!int.TryParse(clientIdTextBox.Text, out clientId))
            {
                MessageBox.Show("Ошибка в введенном ИД клиента. Проверете введенные данные.");
                return;
            }

            double amount = 0;
            if (!double.TryParse(AmountTextBox.Text.Replace(".", ","), out amount) || amount <= 0)
            {
                MessageBox.Show("Ошибка в веденной сумме платежа. Проверете введенные данные.");
                return;
            }

            if (AddPayment(clientId, amount, DateTime.Today))
                UpdatePaymentInfo(clientId);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int clientId = 0;
            if (!int.TryParse(clientIdTextBox.Text, out clientId))
            {
                MessageBox.Show("Ошибка в введенном ИД клиента. Проверете введенные данные.");
                return;
            }
            UpdatePaymentInfo(clientId);
        }

        #endregion

        #region Методы

        /// <summary>
        /// Заполнить тестовые данные.
        /// </summary>
        private void FillTestData()
        {
            // Клиент
            var clients = TestDataModel.Clients;
            var client = new Client(1, "Test1");
            clients.Add(client);

            // График платежей
            var schedules = TestDataModel.Schedules;
            var sheduleRow = new PaymentSchedule(1, client.Id, DateTime.Today.AddMonths(-3), 5000);
            schedules.Add(sheduleRow);
            sheduleRow = new PaymentSchedule(2, client.Id, DateTime.Today.AddMonths(-2), 5000);
            schedules.Add(sheduleRow);
            sheduleRow = new PaymentSchedule(3, client.Id, DateTime.Today.AddMonths(-1), 5000);
            schedules.Add(sheduleRow);
            sheduleRow = new PaymentSchedule(4, client.Id, DateTime.Today, 5000);
            schedules.Add(sheduleRow);
            sheduleRow = new PaymentSchedule(5, client.Id, DateTime.Today.AddMonths(1), 5000);
            schedules.Add(sheduleRow);
            sheduleRow = new PaymentSchedule(6, client.Id, DateTime.Today.AddMonths(2), 4945.11);
            schedules.Add(sheduleRow);

            // Платежи
            var payments = TestDataModel.Payments;
            var payment = new Payment(1, client.Id, DateTime.Today.AddMonths(-3).AddDays(-10), 5000, "Оплачено", 0);
            payments.Add(payment);
            payment = new Payment(2, client.Id, DateTime.Today.AddMonths(-2).AddDays(-10), 6000, "Переплата", 1000);
            payments.Add(payment);
            payment = new Payment(3, client.Id, DateTime.Today.AddMonths(-1).AddDays(-10), 3500, "Недоплата", -500);
            payments.Add(payment);
        }

        /// <summary>
        /// Внести платеж.
        /// </summary>
        /// <param name="clientId">Ид клиента.</param>
        /// <param name="Amount">Сумма платежа.</param>
        /// <param name="date">Дата палтежа.</param>
        public bool AddPayment(int clientId, double amount, DateTime date)
        {
            var result = true;
            var client = TestDataModel.Clients.Where(x => x.Id == clientId).FirstOrDefault();
            if (client != null) 
            {
                var clientShedulePayment = TestDataModel.Schedules.Where(x => x.ClientId == client.Id).ToList();
                var clientPayments = TestDataModel.Payments.Where(x => x.ClientId == client.Id);
                
                var currentSheduleItem = clientShedulePayment.FirstOrDefault(s => s.Date >= date);
                var currentPayments = clientPayments.Where(p => p.Date > currentSheduleItem.Date.AddMonths(-1) && p.Date <= currentSheduleItem.Date).ToList();

                var payment = new Payment();
                payment.Id = TestDataModel.Payments.Select(x => x.Id).LastOrDefault() + 1;
                payment.ClientId = client.Id;
                payment.Date = date;
                payment.Amount = amount;

                double currentBalance = 0;
                var newBalance = currentBalance;
                if (currentPayments.Any())
                {
                    currentBalance = currentPayments.Last().Balance;
                    newBalance = payment.Amount + currentBalance;
                }
                else
                {
                    var lastPayment = clientPayments.LastOrDefault();
                    if (lastPayment != null)
                        currentBalance = lastPayment.Balance;
                    
                    if (currentSheduleItem != null)
                        newBalance = payment.Amount + currentBalance - currentSheduleItem.Amount;
                    else
                        newBalance = payment.Amount + currentBalance;
                }

                payment.Balance = newBalance;
                if (newBalance == 0)
                    payment.Status = "Оплачено";
                else if (newBalance > 0)
                    payment.Status = "Переплата";
                else if (newBalance < 0)
                    payment.Status = "Недоплата";

                TestDataModel.Payments.Add(payment);
            }
            else
            {
             
                MessageBox.Show($"Клиент с Id = {clientId} не найден. Проверьте введенные данные");
                result = false;
            }
            return result;
        }

        private void UpdatePaymentInfo(int clientId)
        {
            var client = TestDataModel.Clients.Where(x => x.Id == clientId).FirstOrDefault();
            if (client != null)
            {
                var clientShedulePayment = TestDataModel.Schedules.Where(x => x.ClientId == client.Id).ToList();
                var clientPayments = TestDataModel.Payments.Where(x => x.ClientId == client.Id);

                dataGridView1.Rows.Clear();
                foreach (var shedulePayment in clientShedulePayment)
                {
                    var date = shedulePayment.Date;
                    var payments = clientPayments.Where(x => x.Date <= date && x.Date > date.AddMonths(-1));

                    if (payments.Any())
                    {
                        foreach (var payment in payments)
                        {
                            dataGridView1.Rows.Add(shedulePayment.Date, shedulePayment.Amount, payment.Amount, payment.Status, payment.Balance);
                        }
                    }
                    else
                    {
                        dataGridView1.Rows.Add(shedulePayment.Date, shedulePayment.Amount, null, null, null);
                    }
                }
            }
            else
            {
                MessageBox.Show($"Клиент с Id = {clientId} не найден. Проверьте введенные данные");
            }
        }

        #endregion

    }
}
