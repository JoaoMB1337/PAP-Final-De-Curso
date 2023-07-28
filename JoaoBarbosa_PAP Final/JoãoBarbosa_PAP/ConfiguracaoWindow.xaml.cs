using System.Windows;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.ComponentModel;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing;
using System;

namespace JoãoBarbosa_PAP
{
    /// <summary>
    /// Interaction logic for ConfiguracaoWindow.xaml
    /// </summary>
    public partial class ConfiguracaoWindow : Window
    {
        public ConfiguracaoWindow()
        {
            InitializeComponent();
            CarregarPrivilegio();
            CarregarFuncionarios();
            
        }

        MySqlConnection ligacaoBD = new MySqlConnection(ConfigurationManager.ConnectionStrings["stringDeLigacaoDB"].ConnectionString);

        public void CarregarFuncionarios()
        {
            try
            {
                ligacaoBD.Open();
                MySqlCommand comandoMySQL = new MySqlCommand("select user.Userimage,user.codigouser,user.nome,user.contacto,user.email,privilegios.NomePrivilegio " +
                    "from user inner join privilegios on user.codigoprivilegio = privilegios.IdPrivilegios; ", ligacaoBD);
                MySqlDataAdapter adp = new MySqlDataAdapter(comandoMySQL);
                DataSet ds = new DataSet();
                adp.Fill(ds, "CarregarFuncionarios");
                DataGridFuncionarios.DataContext = ds;
                DataGridEditarFuncionarios.DataContext = ds;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                ligacaoBD.Close();
            }
        }

        public void CarregarPrivilegio()
        {
            try
            {
                ligacaoBD.Open();
                var dataAdapter = new MySqlDataAdapter("select IdPrivilegios, nomeprivilegio from privilegios;", ligacaoBD);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds, "Categoria");
                DataTable dt = ds.Tables[0];
                ComboBoxPrivilegio.ItemsSource = ((IListSource)dt).GetList();
                ComboBoxPrivilegio.DisplayMemberPath = "nomeprivilegio";
                ComboBoxPrivilegio.SelectedValuePath = "IdPrivilegios";

            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                ligacaoBD.Close();
            }
        }

        public byte[] GetJPGFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();

        }
       

        #region Adicionar Usuario
        private void BtnImagemUser_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofdImage = new OpenFileDialog();
            ofdImage.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (ofdImage.ShowDialog() == true)
            {
                PreviewImagemUser.Source = new BitmapImage(new Uri(ofdImage.FileName));
            }
        }
        private void BtnGuardarUser_Click(object sender, RoutedEventArgs e)
        {
            if (NomeUser.Text.Length >= 1 && PasswordUser.Password.Length >= 1 && ConfirmarPasswordUser.Password.Length >= 1  && ContactoUser.Text.Length >= 1 && EmailUser.Text.Length >= 1 && MoradaUser.Text.Length>=1 && ComboBoxPrivilegio.SelectedItem != null) 
            {
                if(PasswordUser.Password == ConfirmarPasswordUser.Password)
                {
                    try
                    {
                        //Inserir o registo na base de dados
                        ligacaoBD.Open();
                        byte[] ImageData = GetJPGFromImageControl(PreviewImagemUser.Source as BitmapImage);
                        MySqlCommand comandoMySQL = new MySqlCommand(string.Format("INSERT INTO user (Nome,Password,Contacto,Email,morada,CodigoPrivilegio,Status,UserImage)" +
                        " VALUES ('{0}','{1}','{2}','{3}','{4}',{5},{6},?image);", NomeUser.Text, PasswordUser.Password,ContactoUser.Text, EmailUser.Text,MoradaUser.Text,ComboBoxPrivilegio.SelectedValue,"0"), ligacaoBD);
                        MySqlParameter parImage = new MySqlParameter();
                        parImage.ParameterName = "?image";
                        parImage.MySqlDbType = MySqlDbType.LongBlob;
                        parImage.Value = ImageData;
                        comandoMySQL.Parameters.Add(parImage);
                        comandoMySQL.ExecuteNonQuery();
                        MessageBox.Show("Foi adicionado um novo usuario!", "Aviso!", MessageBoxButton.OK, MessageBoxImage.Information);
                        PreviewImagemUser.Source = (BitmapImage)Application.Current.Resources["UserLoginIcon"];
                        NomeUser.Text = null;
                        PasswordUser.Password = null;
                        ConfirmarPasswordUser.Password = null;
                        ContactoUser.Text = null;
                        EmailUser.Text = null;
                        ComboBoxPrivilegio.SelectedItem = null;
                        MoradaUser.Text = null;
                        

                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        ligacaoBD.Close();
                    }
                }
                else
                {
                    MessageBox.Show("A palavra-passe não coincidem", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                
            }
            else
            {
                MessageBox.Show("Preencha todos os campos ", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        
        }

        #endregion

        #region Alterar/Apagar Usario
        private void DataGridEditarFuncionarios_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InformacaoFuncionarios EDITFun = new InformacaoFuncionarios();
            string id;
            DataRowView selectedRecord = (DataRowView)DataGridEditarFuncionarios.SelectedItem;
            if (DataGridEditarFuncionarios.SelectedIndex >= 0)
            {
                id = selectedRecord.Row.ItemArray[1].ToString();
                EDITFun.codigoFuncionario.Text = id.ToString();
                EDITFun.Owner = this;
                EDITFun.ShowDialog();
            }
            else
            {
                MessageBox.Show("Não tem nenhum registo selecionado", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion

        #region Botoes De Navegação

        private void SairBTN_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            ((MainWindow)this.Owner).CarregarDadosFuncionario();
        }

        private void Funcionarios_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => TabControlMain.SelectedIndex = 0));
            CarregarFuncionarios();
        }

        private void AdicionarFunconarios_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => TabControlMain.SelectedIndex = 1));
        }

        private void EditarFuncionarios_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => TabControlMain.SelectedIndex = 2));
            CarregarFuncionarios();
        }

        #endregion


    }
}
