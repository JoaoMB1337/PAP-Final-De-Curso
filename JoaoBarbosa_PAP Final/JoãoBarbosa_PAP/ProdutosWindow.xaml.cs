using Microsoft.Win32;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JoãoBarbosa_PAP
{
    /// <summary>
    /// Interaction logic for ProdutosWindow.xaml
    /// </summary>
    public partial class ProdutosWindow : Window
    {
        MySqlConnection ligacaoBD = new MySqlConnection(ConfigurationManager.ConnectionStrings["stringDeLigacaoDB"].ConnectionString);
        public ProdutosWindow()
        {
            InitializeComponent();
            CarregarStock();
            CarregarCategoria();
            VerificarPrivilegioUser();
        }



        string privilegio;

        public void CarregarStock()
        {
            try
            {
                ligacaoBD.Open();
                MySqlCommand comandoMySQL = new MySqlCommand("select produto.imagemproduto,produto.codigoproduto,produto.NomeProduto, categoria.NomeCategoria,produto.StockReal,produto.StockMin,produto.StockMax " +
                    "from produto inner join categoria on produto.categoria = categoria.CodigoCategoria; ", ligacaoBD);
                MySqlDataAdapter adp = new MySqlDataAdapter(comandoMySQL);
                DataSet ds = new DataSet();
                adp.Fill(ds, "carregarstock");
                DataGridStock.DataContext = ds;
                DataGridEditar.DataContext = ds;
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

        public void CarregarCategoria()
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
                DataGridCategoria.DataContext = ds;
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

        private void VerificarPrivilegioUser()
        {
            try
            {
                ligacaoBD.Open();
                MySqlCommand cm = new MySqlCommand("select codigoprivilegio from user where status = 1", ligacaoBD);
                MySqlDataReader dr = cm.ExecuteReader();
                while(dr.Read())
                {
                    privilegio = dr["codigoprivilegio"].ToString();
                }
                if(privilegio=="2")
                {
                    AdicionarProdutoBTN.Visibility = Visibility.Hidden;
                    AdicionarCategoriaBTN.Visibility = Visibility.Hidden;
                    EditarProdutosBTN.Visibility = Visibility.Hidden;
                    EditarCategoriaBTN.Visibility = Visibility.Hidden;

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

        public byte[] GetJPGFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();

        }

        #region Stock
        private void PesquisarProduto_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ligacaoBD.Open();
                MySqlCommand comandoMySQL = new MySqlCommand("select produto.imagemproduto,produto.codigoproduto,produto.NomeProduto, categoria.NomeCategoria,produto.StockReal,produto.StockMin,produto.StockMax " +
                    "from produto inner join categoria on produto.categoria = categoria.CodigoCategoria where produto.NomeProduto like '" + PesquisarProduto.Text + "%';", ligacaoBD);
                MySqlDataAdapter adp = new MySqlDataAdapter(comandoMySQL);
                DataSet ds = new DataSet();
                adp.Fill(ds, "carregarstock");
                DataGridStock.DataContext = ds;
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
        #endregion

        #region Criar Produtos

        private void BtnImagemProduto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofdImage = new OpenFileDialog();
            ofdImage.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (ofdImage.ShowDialog() == true)
            {
                PreviewImagemProduto.Source = new BitmapImage(new Uri(ofdImage.FileName));
            }
        }

        private void BtnGuardarProduto_Click(object sender, RoutedEventArgs e)
        {
            if(NomeProduto.Text.Length >= 1 && PrecoProduto.Text.Length >= 1 && QuantidadeDiponivel.Text.Length >= 1 && QuantidadeMaxima.Text.Length >= 1 && QuantidadeMinima.Text.Length >= 1 && ComboBoxCategoria.SelectedItem != null)
            {
                int stockreal = Convert.ToInt32(QuantidadeDiponivel.Text);
                int stockmax = Convert.ToInt32(QuantidadeMaxima.Text);
                int stockmin = Convert.ToInt32(QuantidadeMinima.Text);
                if(stockmax>=stockreal && stockreal>=stockmin)
                {
                    try
                    {
                        ligacaoBD.Open();
                        string mycmd = string.Format("select count(NomeProduto) from produto where NomeProduto='{0}' and categoria = '{1}'", NomeProduto.Text, ComboBoxCategoria.SelectedValue);
                        MySqlCommand comandoMySQL1 = new MySqlCommand(mycmd, ligacaoBD);
                        int nrlinhas = Convert.ToInt32(comandoMySQL1.ExecuteScalar());
                        if (nrlinhas >= 1)
                        {
                            MessageBox.Show("Já existe um produto com mesmo nome na mesma categoria inserida!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            PrecoProduto.Text = PrecoProduto.Text.Replace(",", ".");
                            byte[] ImageData = GetJPGFromImageControl(PreviewImagemProduto.Source as BitmapImage);
                            MySqlCommand comandoMySQL = new MySqlCommand(string.Format("INSERT INTO produto (NomeProduto,PrecoProduto,categoria,StockReal,StockMax,StockMin,ImagemProduto) " +
                                "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}',?image);", NomeProduto.Text, PrecoProduto.Text, ComboBoxCategoria.SelectedValue, QuantidadeDiponivel.Text, QuantidadeMaxima.Text, QuantidadeMinima.Text), ligacaoBD);
                            MySqlParameter parImage = new MySqlParameter();
                            parImage.ParameterName = "?image";
                            parImage.MySqlDbType = MySqlDbType.LongBlob;
                            parImage.Value = ImageData;
                            comandoMySQL.Parameters.Add(parImage);
                            comandoMySQL.ExecuteNonQuery();
                            MessageBox.Show("Foi adicionado um novo produto", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);

                            PreviewImagemProduto.Source = (BitmapImage)Application.Current.Resources["ImagemNaoDisponivel"];
                            NomeProduto.Text = null;
                            ComboBoxCategoria.SelectedValue = null;
                            PrecoProduto.Text = null;
                            QuantidadeDiponivel.Text = null;
                            QuantidadeMaxima.Text = null;
                            QuantidadeMinima.Text = null;
                        }

                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        ligacaoBD.Close();
                        CarregarStock();
                    }
                }
                else
                {
                    MessageBox.Show("Verifique a quantidade de stock inserido","Aviso!",MessageBoxButton.OK,MessageBoxImage.Information);
                }
                
            }
            else
            {
                MessageBox.Show("Preencha todos os campos", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);

            }

        }

        #endregion

        #region Adicionar Categoria


        private void BtnGuardarCategoria_Click(object sender, RoutedEventArgs e)
        {
            if(NomeCategoria.Text.Length>=1)
            {
                try
                {
                    ligacaoBD.Open();
                    string mycmd1 = string.Format("select count(NomeCategoria) from categoria where Nomecategoria ='{0}'", NomeCategoria.Text);
                    MySqlCommand comandoMySQL1 = new MySqlCommand(mycmd1, ligacaoBD);
                    int nrlinhas = Convert.ToInt32(comandoMySQL1.ExecuteScalar());
                    if (nrlinhas >= 1)
                    {
                        MessageBox.Show("Já existe uma categoria com mesmo nome inserida!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MySqlCommand comandoMySQL = new MySqlCommand(string.Format("INSERT INTO Categoria (NomeCategoria) " +
                            "VALUES ('{0}');", NomeCategoria.Text), ligacaoBD);
                        comandoMySQL.ExecuteNonQuery();
                        MessageBox.Show("Foi adicionado uma nova Categoria", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                        NomeCategoria.Text = null;
                    }
                    
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    ligacaoBD.Close();
                    CarregarCategoria();
                }
            }
            else
            {
                MessageBox.Show("Preencha todos os campos", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }




        #endregion

        #region Alterar Produtos
        private void DataGridEditar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ProdutosInformacaoWindow InfProd = new ProdutosInformacaoWindow();
            string id;
            DataRowView selectedRecord = (DataRowView)DataGridEditar.SelectedItem;
            if(DataGridEditar.SelectedIndex >=0)
            {
                id = selectedRecord.Row.ItemArray[1].ToString();
                InfProd.codigoproduto.Text = id.ToString();
                InfProd.Owner = this;
                InfProd.ShowDialog();
            }
            else
            {
                MessageBox.Show("Não tem nenhum registo selecionado", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
        private void PesquisarProdutoeditar_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ligacaoBD.Open();
                MySqlCommand comandoMySQL = new MySqlCommand("select produto.NomeProduto, categoria.NomeCategoria,produto.StockReal,produto.StockMin " +
                    "from produto inner join categoria on produto.categoria = categoria.CodigoCategoria where produto.NomeProduto like '" + PesquisarProdutoeditar.Text + "%';", ligacaoBD);
                MySqlDataAdapter adp = new MySqlDataAdapter(comandoMySQL);
                DataSet ds = new DataSet();
                adp.Fill(ds, "carregarstock");
                DataGridEditar.DataContext = ds;
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


        #endregion

        #region Alterar categoria
        private void DataGridCategoria_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            CategoriaInformacaoWindow InfCat = new CategoriaInformacaoWindow();
            string id;
            DataRowView selectedRecord = (DataRowView)DataGridCategoria.SelectedItem;
            if (DataGridCategoria.SelectedIndex >= 0)
            {
                id = selectedRecord.Row.ItemArray[0].ToString();
                InfCat.codigocategoria.Text = id.ToString();
                InfCat.Owner = this;
                InfCat.ShowDialog();
            }
            else
            {
                MessageBox.Show("Não tem nenhum registo selecionado", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
        #endregion

        #region Botoes De Navegaçao

        private void StockBTN_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => TabControlMain.SelectedIndex = 0));
            CarregarStock();
        }

        private void AdicionarProdutoBTN_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => TabControlMain.SelectedIndex = 1));
        }

        private void AdicionarCategoriaBTN_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => TabControlMain.SelectedIndex = 2));
        }

        private void EditarProdutosBTN_Click(object sender, RoutedEventArgs e)
        {
            CarregarStock();
            Dispatcher.BeginInvoke((Action)(() => TabControlMain.SelectedIndex = 3));
        }

        private void EditarCategoriaBTN_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => TabControlMain.SelectedIndex = 4));
        }

        private void SairBTN_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)this.Owner).VerificarStock();

            this.Close();

        }

        #endregion

        #region Validação Das TextBox 

        private void QuantidadeDiponivel_TextChanged(object sender, TextChangedEventArgs e)
        {
            QuantidadeDiponivel.Text = Regex.Replace(QuantidadeDiponivel.Text, "[^0-9]", "");
        }

        private void QuantidadeMaxima_TextChanged(object sender, TextChangedEventArgs e)
        {
            QuantidadeMaxima.Text = Regex.Replace(QuantidadeMaxima.Text, "[^0-9]", "");
        }

        private void QuantidadeMinima_TextChanged(object sender, TextChangedEventArgs e)
        {
            QuantidadeMinima.Text = Regex.Replace(QuantidadeMinima.Text, "[^0-9]", "");

        }

        private void PrecoProduto_TextChanged(object sender, TextChangedEventArgs e)
        {
            PrecoProduto.Text = Regex.Replace(PrecoProduto.Text, "[^0-9.]", "");
        }

        #endregion



    }
}
