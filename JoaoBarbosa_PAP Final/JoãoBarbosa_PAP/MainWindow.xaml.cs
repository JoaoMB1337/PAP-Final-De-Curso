using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
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
using Microsoft.Win32;
using System.Drawing;


namespace JoãoBarbosa_PAP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MySqlConnection ligacaoBD = new MySqlConnection(ConfigurationManager.ConnectionStrings["stringDeLigacaoDB"].ConnectionString);
        public MainWindow()
        {
            InitializeComponent();
            CarregarDadosFuncionario();
            VerificarStock();

        }

        bool close = false;
        string privilegio;
        string codigouser;

       public void VerificarStock()
       {
            try
            {
                ligacaoBD.Open();
                MySqlCommand cm = new MySqlCommand("select count(codigoproduto) from produto where stockmin >= stockreal; ",ligacaoBD);
                int nrlinhas = Convert.ToInt32(cm.ExecuteScalar());
                if(nrlinhas>=1)
                {
                    StackPanelAvisos.Visibility = Visibility.Visible;
                }
                else
                {
                    StackPanelAvisos.Visibility = Visibility.Hidden;
                }
            }
            catch(MySqlException ex )
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                ligacaoBD.Close();
            }
       }

        public void CarregarDadosFuncionario()
        {
            try
            {
                ligacaoBD.Open();
                MySqlCommand cm = new MySqlCommand("select codigouser,Nome,userimage,CodigoPrivilegio from user where status = 1 ",ligacaoBD);
                MySqlDataReader dr = cm.ExecuteReader();
                while (dr.Read())
                {
                    NomeUser.Content = dr["Nome"].ToString();
                    privilegio = dr["CodigoPrivilegio"].ToString();
                    codigouser = dr["codigouser"].ToString();
                    byte[] imgg = (byte[])(dr["Userimage"]);
                    if (imgg == null)
                    {
                        ImagemUser.ImageSource = null;
                    }
                    else
                    {
                        MemoryStream mstream = new MemoryStream(imgg);
                        ImagemUser.ImageSource = BitmapImageFromBytes(imgg);
                    }
                }
                if(privilegio=="2")
                {
                    configBtn.Visibility = Visibility.Hidden;
                }

            }
            catch(MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                ligacaoBD.Close();
            }
        }

        #region Imagem
        public static BitmapImage BitmapImageFromBytes(byte[] bytes)
        {
            BitmapImage image = null;
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(bytes);
                stream.Seek(0, SeekOrigin.Begin);
                System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                image = new BitmapImage();
                image.BeginInit();
                MemoryStream ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                image.StreamSource = ms;
                image.StreamSource.Seek(0, SeekOrigin.Begin);
                image.EndInit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                stream.Close();
                stream.Dispose();
            }
            return image;
        }

        #endregion

        #region Botoes de navegação

        private void SairBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Deseja sair do programa?", "Aviso!", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    close = true;
                    try
                    {
                        ligacaoBD.Open();
                        MySqlCommand cm = new MySqlCommand("update user set status = 0  where  codigouser='" + codigouser + "' ", ligacaoBD);
                        cm.ExecuteNonQuery();
                    }
                    catch(MySqlException ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        ligacaoBD.Close();
                    }

                    Application.Current.Shutdown();
                    break;
            }
        }

        private void ProdutosBtn_Click(object sender, RoutedEventArgs e)
        {
            ProdutosWindow w2 = new ProdutosWindow();
            w2.Owner = this;
            w2.ShowDialog();
            
        }

        private void configBtn_Click(object sender, RoutedEventArgs e)
        {
            ConfiguracaoWindow w3 = new ConfiguracaoWindow();
            w3.Owner = this;
            w3.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (close)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void VenderBtn_Click(object sender, RoutedEventArgs e)
        {
            GestorDeVendasWindow w4 = new GestorDeVendasWindow();     
            w4.Show();
        }

        #endregion
    }
}