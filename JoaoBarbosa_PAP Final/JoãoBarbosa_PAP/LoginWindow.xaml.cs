using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;


namespace JoãoBarbosa_PAP
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        MySqlConnection ligacaoBD = new MySqlConnection(ConfigurationManager.ConnectionStrings["stringDeLigacaoDB"].ConnectionString);

        public LoginWindow()
        {
            InitializeComponent();
        }
        string codigouser;

        private void botaologin_Click(object sender, RoutedEventArgs e)
        {
           
            try
            {
                ligacaoBD.Open();
                MySqlCommand cm = new MySqlCommand("select codigouser,nome,password from user where nome='" + UserTxt.Text + "' and password='"+PasswordTxt.Password +"';",ligacaoBD);
                cm.ExecuteNonQuery();
                DataTable dt = new DataTable();
                MySqlDataAdapter adp = new MySqlDataAdapter(cm);
                adp.Fill(dt);
                MySqlDataReader dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    codigouser = dr["codigouser"].ToString();
                }
                ligacaoBD.Close();
                
                int i = Convert.ToInt32(dt.Rows.Count.ToString());
                if(i==0)
                {
                    MessageBox.Show("Os Dados nao foram corretamente preenchidos");
                }
                else
                {
                    ligacaoBD.Open();
                    MySqlCommand cm2 = new MySqlCommand( "Update user set status=1 where codigouser='" + codigouser + "'", ligacaoBD);
                    cm2.ExecuteNonQuery();
                    ligacaoBD.Close();
                    MainWindow w1 = new MainWindow();
                    w1.Show();
                    this.Close();
                }
               
               
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}