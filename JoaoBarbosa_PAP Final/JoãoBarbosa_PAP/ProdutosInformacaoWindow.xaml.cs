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
using System.Text.RegularExpressions;

namespace JoãoBarbosa_PAP
{
    /// <summary>
    /// Interaction logic for ProdutosInformacaoWindow.xaml
    /// </summary>
    public partial class ProdutosInformacaoWindow : Window
    {
        MySqlConnection ligacaoBD = new MySqlConnection(ConfigurationManager.ConnectionStrings["stringDeLigacaoDB"].ConnectionString);

        public ProdutosInformacaoWindow()
        {
            InitializeComponent();
        }
        
        public void CarregarProd()
        {
            try
            {
                ligacaoBD.Open();
                string mycmd = string.Format("select produto.codigoproduto,produto.NomeProduto, produto.Categoria,produto.StockReal,produto.StockMin,produto.stockmax,produto.precoproduto,produto.imagemproduto " +
                     "from produto  where codigoproduto='{0}' ", codigoproduto.Text);
                MySqlCommand comandoMySQL = new MySqlCommand (mycmd, ligacaoBD);
                MySqlDataReader dr = comandoMySQL.ExecuteReader();
                while (dr.Read())
                {
                    NomeProduto.Text = dr["NomeProduto"].ToString();
                    PrecoProduto.Text = dr["precoproduto"].ToString();
                    StockDisponivel.Text = dr["StockReal"].ToString();
                    StockMax.Text = dr["Stockmax"].ToString();
                    StockMin.Text = dr["stockmin"].ToString();
                    ComboBoxCategoria.SelectedValue = dr["categoria"].ToString();
                    byte[] imgg = (byte[])(dr["imagemproduto"]);
                    if (imgg == null)
                    {
                        PreviewImagemProduto.Source = null;
                    }
                    else
                    {
                        MemoryStream mstream = new MemoryStream(imgg);
                        PreviewImagemProduto.Source = BitmapImageFromBytes(imgg);
                    }
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

        private void CarregarCategoria()
        {
            try
            {
                ligacaoBD.Open();
                var dataAdapter = new MySqlDataAdapter("select CodigoCategoria,NomeCategoria from categoria;", ligacaoBD);
                DataSet ds = new DataSet();
                dataAdapter.Fill(ds, "Categoria");
                DataTable dt = ds.Tables[0];
                ComboBoxCategoria.ItemsSource = ((IListSource)dt).GetList();
                ComboBoxCategoria.DisplayMemberPath = "NomeCategoria";
                ComboBoxCategoria.SelectedValuePath = "CodigoCategoria";

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

        #region Imagens

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
            catch (Exception)
            {
                throw;
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
                PreviewImagemProduto.Source = new BitmapImage(new Uri(ofdImage.FileName));
            }
        }

        #endregion

        private void Apagar_Click(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show("Tem a certeza que deseja eliminar o produto?", "ATENÇÃO!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    ligacaoBD.Open();
                    MySqlCommand comandoMySQL = new MySqlCommand(string.Format("DELETE FROM produto WHERE codigoproduto = '{0}';", codigoproduto.Text), ligacaoBD);
                    comandoMySQL.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    ligacaoBD.Close();
                    MessageBox.Show("O produto foi eliminado.", "Relatório:", MessageBoxButton.OK, MessageBoxImage.Information);
                    ((ProdutosWindow)this.Owner).CarregarStock();
                    this.Close();

                }
                
            }
            else
            {
                MessageBox.Show("O produto não foi eliminado.", "Relatório:", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            if(NomeProduto.Text.Length>=1 && PrecoProduto.Text.Length >=1 && StockDisponivel.Text.Length>=1 && StockMin.Text.Length >= 1 && StockMax.Text.Length >= 1)
            {
                int stockreal = Convert.ToInt32(StockDisponivel.Text);
                int stockmax = Convert.ToInt32(StockMax.Text);
                int stockmin = Convert.ToInt32(StockMin.Text);
                if (stockmax >= stockreal && stockreal >= stockmin)
                {
                    try
                    {
                        ligacaoBD.Open();
                        string mycmd = string.Format("select count(NomeProduto) from produto where NomeProduto='{0}' and categoria = '{1}' and codigoproduto <> '{2}'", NomeProduto.Text, ComboBoxCategoria.SelectedValue, codigoproduto.Text);
                        MySqlCommand comandoMySQL1 = new MySqlCommand(mycmd, ligacaoBD);
                        int nrlinhas = Convert.ToInt32(comandoMySQL1.ExecuteScalar());
                        if (nrlinhas >= 1)
                        {
                            MessageBox.Show("Já existe um produto com mesmo nome na mesma categoria inserida!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            byte[] ImageData = GetJPGFromImageControl(PreviewImagemProduto.Source as BitmapImage);
                            PrecoProduto.Text = PrecoProduto.Text.Replace(",", ".");
                            MySqlCommand comandoMySQL = new MySqlCommand(string.Format("UPDATE produto SET NomeProduto='{0}',Precoproduto='{1}',Categoria='{2}',StockReal='{3}',StockMin='{4}',StockMax='{5}',ImagemProduto={6} " +
                                "where codigoproduto ='{7}'", NomeProduto.Text, PrecoProduto.Text, ComboBoxCategoria.SelectedValue, StockDisponivel.Text, StockMin.Text, StockMax.Text, "?image", codigoproduto.Text), ligacaoBD);
                            MySqlParameter parImage = new MySqlParameter();
                            parImage.ParameterName = "?image";
                            parImage.MySqlDbType = MySqlDbType.LongBlob;
                            parImage.Value = ImageData;
                            comandoMySQL.Parameters.Add(parImage);
                            comandoMySQL.ExecuteNonQuery();
                            MessageBox.Show("O produto foi alterado com sucesso.", "Relatório:", MessageBoxButton.OK, MessageBoxImage.Information);
                            ((ProdutosWindow)this.Owner).CarregarStock();
                            this.Close();
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
                else
                {
                    MessageBox.Show("Verifique a quantidade de stock inserido", "Aviso!", MessageBoxButton.OK, MessageBoxImage.Information);

                }

            }
            else
            {
                MessageBox.Show("Preencha todas as caixas de texto!", "Aviso!", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            
        }

        private void cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregarCategoria();
            CarregarProd();
        }

        #region Validadção das textbox  

        private void StockDisponivel_TextChanged(object sender, TextChangedEventArgs e)
        {
            StockDisponivel.Text = Regex.Replace(StockDisponivel.Text, "[^0-9]", "");
        }

        private void StockMin_TextChanged(object sender, TextChangedEventArgs e)
        {
            StockMin.Text = Regex.Replace(StockMin.Text, "[^0-9]", "");

        }

        private void StockMax_TextChanged(object sender, TextChangedEventArgs e)
        {
            StockMax.Text = Regex.Replace(StockMax.Text, "[^0-9]", "");
        }

        private void PrecoProduto_TextChanged(object sender, TextChangedEventArgs e)
        {
            StockMax.Text = Regex.Replace(StockMax.Text, "[^0-9.]", "");
        }

        #endregion
    }
}
