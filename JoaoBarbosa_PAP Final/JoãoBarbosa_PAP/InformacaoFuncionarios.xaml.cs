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
    /// Interaction logic for InformacaoFuncionarios.xaml
    /// </summary>
    public partial class InformacaoFuncionarios : Window
    {
        MySqlConnection ligacaoBD = new MySqlConnection(ConfigurationManager.ConnectionStrings["stringDeLigacaoDB"].ConnectionString);
        public InformacaoFuncionarios()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregarFuncionarios();
            CarregarPrivilegio();
        }

        public void CarregarFuncionarios()
        {
            try
            {
                ligacaoBD.Open();
                string mycmd = string.Format("select Nome,Password,Userimage,morada,Contacto,Email,CodigoPrivilegio from user where codigouser = '{0}'", codigoFuncionario.Text);
                MySqlCommand comandoMySQL = new MySqlCommand(mycmd, ligacaoBD);
                MySqlDataReader dr = comandoMySQL.ExecuteReader();
                while (dr.Read())
                {
                    NomeUser.Text = dr["nome"].ToString();
                    Pass.Text = dr["password"].ToString();
                    byte[] imgg = (byte[])(dr["userimage"]);
                    if (imgg == null)
                    {
                        PreviewImagemUser.ImageSource = null;
                    }
                    else
                    {
                        MemoryStream mstream = new MemoryStream(imgg);
                        PreviewImagemUser.ImageSource = BitmapImageFromBytes(imgg);
                    }
                    MoradaUser.Text = dr["morada"].ToString();
                    ContactoUser.Text = dr["contacto"].ToString();            
                    MailUser.Text = dr["Email"].ToString();
                    ComboBoxPrivilegio.SelectedValue = dr["CodigoPrivilegio"].ToString();                       
                    
                }

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

        #region Imagem

        public byte[] GetJPGFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();

        }

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

        private void EditarImagem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofdImage = new OpenFileDialog();
            ofdImage.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (ofdImage.ShowDialog() == true)
            {
                PreviewImagemUser.ImageSource = new BitmapImage(new Uri(ofdImage.FileName));
            }
        }

        #endregion

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ligacaoBD.Open();
                 byte[] ImageData = GetJPGFromImageControl(PreviewImagemUser.ImageSource as BitmapImage);
                string query = "update user set nome=@nome,userimage=?image,contacto=@contacto,email=@email,morada=@morada,CodigoPrivilegio=@codigoprivilegio where codigouser= @codigouser";      
                MySqlCommand cm = new MySqlCommand(query, ligacaoBD);
                cm.Parameters.Add(new MySqlParameter("@nome", NomeUser.Text));
                cm.Parameters.Add(new MySqlParameter("@contacto", ContactoUser.Text));
                cm.Parameters.Add(new MySqlParameter("@email", MailUser.Text));
                cm.Parameters.Add(new MySqlParameter("@morada", MoradaUser.Text));
                cm.Parameters.Add(new MySqlParameter("@codigouser", codigoFuncionario.Text));
                cm.Parameters.Add(new MySqlParameter("@codigoprivilegio", ComboBoxPrivilegio.SelectedValue));
                MySqlParameter parImage = new MySqlParameter();
                parImage.ParameterName = "?image";
                parImage.MySqlDbType = MySqlDbType.LongBlob;
                parImage.Value = ImageData;
                cm.Parameters.Add(parImage);
                cm.ExecuteNonQuery();
                MessageBox.Show("Os dados do funcionario foram atualizados com sucesso!", "Relatório:", MessageBoxButton.OK, MessageBoxImage.Information);
                ((ConfiguracaoWindow)this.Owner).CarregarFuncionarios();
                    this.Close();

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

        private void cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Apagar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show("Deseja eliminar o funcionario?", "Aviso!", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        ligacaoBD.Open();
                        MySqlCommand cm = new MySqlCommand("Delete from user where codigouser = '" + codigoFuncionario.Text + "';", ligacaoBD);
                        cm.ExecuteNonQuery();
                        MessageBox.Show("O funcionario foi removido com sucesso!", "Relatorio:", MessageBoxButton.OK, MessageBoxImage.Information);
                        ((ConfiguracaoWindow)this.Owner).CarregarFuncionarios();
                        this.Close();
                        break;
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

    }
}
