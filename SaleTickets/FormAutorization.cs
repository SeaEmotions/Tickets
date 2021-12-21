using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SaleTickets
{
    public partial class FormAutorization : Form
    {
        string LoginString;
        string PasswordString;

        TicketSystemDataSet.UsersDataTable dataUsers;
        TicketSystemDataSet.LoginHistoryDataTable dataLoginHistory;

        TimeSpan timeString;
        public FormAutorization()
        {
            InitializeComponent();
        }

        private void EnterButton_Click(object sender, EventArgs e)
        {
            if (LoginTextBox.Text == string.Empty && PasswordTextBox.Text == string.Empty)
            {
                MessageBox.Show("Заполните все поля", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            LoginString = LoginTextBox.Text.ToString();
            PasswordString = PasswordTextBox.Text.ToString();

            timeString = DateTime.Now.TimeOfDay;

            dataUsers = this.usersTableAdapter.GetData(); //пользователи
            dataLoginHistory = this.loginHistoryTableAdapter.GetData(); //время входа

            var filter = dataUsers.Where(rec => rec.Email == LoginString && rec.Password == PasswordString);
            if (filter.Count() == 0)    //Нет записей – совпадение логина+пароля не найдено
            {
                MessageBox.Show("Неверный логин или пароль");
                try
                {                    
                    loginHistoryTableAdapter.Insert(LoginString, timeString, false);//Добавить в таблицу истории запись с неудачным входом
                }
                catch
                {
                    MessageBox.Show("Ошибка в истории входа");
                }
            }
            else
            {
                ClassTotal.idUser = filter.ElementAt(0).ID;
                ClassTotal.idRole = filter.ElementAt(0).RoleID;
                ClassTotal.login = filter.ElementAt(0).Email;

                try
                {
                    loginHistoryTableAdapter.Insert(LoginString, timeString, true);//Добавить в таблицу истории запись с удачным входом
                }
                catch
                {
                    MessageBox.Show("Ошибка в истории входа");
                    return;
                }

                switch (ClassTotal.idRole) //Переход к формам в зависимости от роли
                {
                    case 1:
                        MessageBox.Show("Вы успешно авторизовались как администратор");
                        FormAdministrator fa = new FormAdministrator();
                        this.Hide();
                        fa.ShowDialog();
                        this.Show();
                        break;
                    case 2:
                        MessageBox.Show("Вы успешно авторизовались как организатор");
                        FormOrganizer fo = new FormOrganizer();
                        this.Hide();
                        fo.ShowDialog();
                        this.Show();
                        break;
                    case 3:
                        MessageBox.Show("Вы успешно авторизовались как продавец");
                        FormSeller fs = new FormSeller();
                        this.Hide();
                        fs.ShowDialog();
                        this.Show();
                        break;
                    case 4:
                        MessageBox.Show("Вы успешно авторизовались как покупатель");
                        FormBuyer fb = new FormBuyer();
                        this.Hide();
                        fb.ShowDialog();
                        this.Show();
                        break;
                }
            }
        }
    }
}
