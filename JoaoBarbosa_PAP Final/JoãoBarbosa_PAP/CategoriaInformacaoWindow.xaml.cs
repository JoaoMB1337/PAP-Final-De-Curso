using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
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
    /// Interaction logic for CategoriaInformacaoWindow.xaml
    /// </summary>
    public partial class CategoriaInformacaoWindow : Window
    {
        MySqlConnection ligacaoBD = new MySqlConnection(ConfigurationManager.ConnectionStrings["stringDeLigacaoDB"].ConnectionString);

        public CategoriaInformacaoWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregarCategoria();
        }

        public void CarregarCategoria()
        {
            try
            {
                string codigo = codigocategoria.Text;
                ligacaoBD.Open();
                string mycmd = string.Format("select NomeCategoria from categoria where codigocategoria='{0}'",codigo);
               MySqlCommand comandoMySQL = new MySqlCommand(mycmd, ligacaoBD);
                MySqlDataReader dr = comandoMySQL.ExecuteReader();
                while (dr.Read())
                {
                    NomeCategoria.Text = dr["NomeCategoria"].ToString();
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

        #region Imagem

        public byte[] GetJPGFromImageControl(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();

        }


        #endregion

        private void Apagar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ligacaoBD.Open();
                string mycmd = string.Format("select count(codigoproduto) from produto where categoria='{0}'",codigocategoria.Text);
                MySqlCommand comandoMySQL = new MySqlCommand(mycmd, ligacaoBD);
                int nrlinhas = Convert.ToInt32(comandoMySQL.ExecuteScalar());
                if (nrlinhas >= 1)
                {
                    MessageBox.Show("Não é possivel apagar a categoria porque existe produtos inseridos nessa categoria! ", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    string mycmd2 = string.Format("DELETE FROM categoria where codigocategoria = '{0}'", codigocategoria.Text);
                    MySqlCommand comandoMySQL2 = new MySqlCommand(mycmd2, ligacaoBD);
                    comandoMySQL2.ExecuteNonQuery();
                    MessageBox.Show("O Categoria selecionada foi apagada!", "Relatorio", MessageBoxButton.OK, MessageBoxImage.Information);
                    ((ProdutosWindow)this.Owner).CarregarCategoria();
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

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ligacaoBD.Open();
                string mycmd1 = string.Format("select count(NomeCategoria) from categoria where Nomecategoria ='{0}'",NomeCategoria.Text);
                MySqlCommand comandoMySQL1 = new MySqlCommand(mycmd1, ligacaoBD);
                int nrlinhas = Convert.ToInt32(comandoMySQL1.ExecuteScalar());
                if (nrlinhas >= 1)
                {
                    MessageBox.Show("Já existe uma categoria com mesmo nome inserida!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                   
                    string mycmd = string.Format("UPDATE categoria SET NomeCategoria='{0}' WHERE CodigoCategoria={1}", NomeCategoria.Text, codigocategoria.Text);
                    MySqlCommand comandoMySQL = new MySqlCommand(mycmd, ligacaoBD);
                    comandoMySQL.ExecuteNonQuery();
                    MessageBox.Show("A categoria foi alterado com sucesso.", "Relatório:", MessageBoxButton.OK, MessageBoxImage.Information);
                    ((ProdutosWindow)this.Owner).CarregarCategoria();
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

        private void cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
