using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using DGVPrinterHelper;




namespace JoãoBarbosa_PAP
{
    public partial class GestorDeVendasWindow : Form
    {
        double _tot;
        MySqlConnection cn;
        MySqlCommand cm;
        MySqlDataReader dr;
        private PictureBox img;
        private Label preco;
        private Label nome;
        private Button categoria;
        public GestorDeVendasWindow()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = "server = localhost; user id = root; password = aedas; database = 06_joaobarbosa_pap; ";
        }

        private void GestorDeVendasWindow_Load(object sender, EventArgs e)
        {
            carregarprodutos();
            carregarcategorias();
            label2.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }
        private void Limparflowpanel ()
        {
            List<Control> listControls = new List<Control>();

            foreach (Control control in flowLayoutPanel1.Controls)
            {
                listControls.Add(control);
            }

            foreach (Control control in listControls)
            {
                flowLayoutPanel1.Controls.Remove(control);
                control.Dispose();
            }
        }

        #region produtos

        private void carregarprodutos()
        {
            cn.Open();
            cm = new MySqlCommand("select ImagemProduto,CodigoProduto,NomeProduto,PrecoProduto,Categoria from produto where StockReal >= 1;", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                long len = dr.GetBytes(0, 0, null, 0, 0);
                byte[] array = new byte[Convert.ToInt32(len) + 1];
                dr.GetBytes(0, 0, array, 0, Convert.ToInt32(len));
                img = new PictureBox();
                img.Width = 130;
                img.Height = 130;
                img.BackgroundImageLayout = ImageLayout.Stretch;
                img.BorderStyle = BorderStyle.FixedSingle;
                img.Tag = dr["CodigoProduto"].ToString();

                preco = new Label();
                preco.Text = dr["PrecoProduto"].ToString();
                preco.Text = preco.Text + "‎€";
                preco.Width = 50;
                preco.BackColor = Color.FromArgb(46, 134, 222);
                preco.ForeColor = Color.White;
                preco.TextAlign = ContentAlignment.MiddleCenter;

                nome = new Label();
                nome.Text = dr["NomeProduto"].ToString();
                nome.BackColor = Color.FromArgb(255, 121, 121);
                nome.TextAlign = ContentAlignment.MiddleCenter;
                nome.Dock = DockStyle.Bottom;

                MemoryStream ms = new MemoryStream(array);
                Bitmap bitmap = new Bitmap(ms);
                img.BackgroundImage = bitmap;

                img.Controls.Add(nome);
                img.Controls.Add(preco);
                flowLayoutPanel1.Controls.Add(img);
                img.Cursor = Cursors.Hand;

                img.Click += new EventHandler(OnClick);
            }
            dr.Close();
            cn.Close();
        }
        public void OnClick(object sender, EventArgs e)
        {
            double quantidade = 1;
            string tag = ((PictureBox)sender).Tag.ToString();
            cn.Open();
            cm = new MySqlCommand("select * from produto where CodigoProduto = '" + tag + "'", cn);
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                _tot += double.Parse(dr["PrecoProduto"].ToString());
                dataGridView1.Rows.Add(dr["NomeProduto"].ToString(), dr["CodigoProduto"].ToString(), quantidade.ToString(), double.Parse(dr["PrecoProduto"].ToString()).ToString("#,##0.00"));
                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
            }
            dr.Close();
            cn.Close();
            label1.Text = _tot.ToString("#,##0.00") + "€";
        }

        #endregion

        #region categoria
        private void carregarcategorias()
        {
            cn.Open();
            cm = new MySqlCommand("select CodigoCategoria,NomeCategoria from categoria", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                categoria = new Button();
                categoria.Width = 180;
                categoria.Height = 60;
                categoria.Text = dr["NomeCategoria"].ToString();
                categoria.FlatStyle = FlatStyle.Flat;
                categoria.BackColor = Color.FromArgb(41, 128, 185);
                categoria.ForeColor = Color.White;
                categoria.BackgroundImageLayout = ImageLayout.Stretch;
                flowLayoutPanel2.Controls.Add(categoria);
                categoria.Tag = dr["CodigoCategoria"].ToString();
                categoria.Cursor = Cursors.Hand;


                categoria.Click += new EventHandler(OnClick2);
            }
            dr.Close();
            cn.Close();
        }
        public void OnClick2(object sender, EventArgs e)
        {
            Limparflowpanel();
            string tag = ((Button)sender).Tag.ToString();
            cn.Open();
            cm = new MySqlCommand("select ImagemProduto,CodigoProduto,NomeProduto,PrecoProduto,Categoria from produto where categoria ='" + tag + "'and StockReal >=1", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                long len = dr.GetBytes(0, 0, null, 0, 0);
                byte[] array = new byte[Convert.ToInt32(len) + 1];
                dr.GetBytes(0, 0, array, 0, Convert.ToInt32(len));
                img = new PictureBox();
                img.Width = 130;
                img.Height = 130;
                img.BackgroundImageLayout = ImageLayout.Stretch;
                img.BorderStyle = BorderStyle.FixedSingle;
                img.Tag = dr["CodigoProduto"].ToString();

                preco = new Label();
                preco.Text = dr["PrecoProduto"].ToString();
                preco.Text = preco.Text + "‎€";
                preco.Width = 50;
                preco.BackColor = Color.FromArgb(46, 134, 222);
                preco.ForeColor = Color.White;
                preco.TextAlign = ContentAlignment.MiddleCenter;
                preco.Tag = dr["CodigoProduto"].ToString();

                nome = new Label();
                nome.Text = dr["NomeProduto"].ToString();
                nome.BackColor = Color.FromArgb(255, 121, 121);
                nome.TextAlign = ContentAlignment.MiddleCenter;
                nome.Dock = DockStyle.Bottom;
                nome.Tag = dr["CodigoProduto"].ToString();

                MemoryStream ms = new MemoryStream(array);
                Bitmap bitmap = new Bitmap(ms);
                img.BackgroundImage = bitmap;

                img.Controls.Add(nome);
                img.Controls.Add(preco);
                flowLayoutPanel1.Controls.Add(img);
                img.Cursor = Cursors.Hand;

                img.Click += new EventHandler(OnClick);
            }
            dr.Close();
            cn.Close();
        }

        #endregion

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            if (dataGridView1.CurrentCell.Value !=null)
            {
                string preco = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                _tot -= double.Parse(preco);
                dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
                label1.Text = _tot.ToString("#,##0.00") + "€";
            }
            else
            {
                System.Windows.MessageBox.Show("Não tem nenhum item selecionado! ", "Aviso");
            }
            

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label3.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void SairBTN_Click(object sender, EventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Deseja sair do modulo de venda?", "Aviso!", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    this.Close();
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                cn.Open();
                string codigoproduto;
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        codigoproduto = dataGridView1.Rows[i].Cells["codigoproduto"].Value.ToString();
                        cm = new MySqlCommand("select sum(stockreal) -1 from produto where codigoproduto='" + codigoproduto + "';", cn);
                        int resultado = Convert.ToInt32(cm.ExecuteScalar());
                        cm = new MySqlCommand("update produto set stockreal = '" + resultado + "' where codigoproduto = '"+ codigoproduto+"' ",cn);
                        cm.ExecuteScalar();
                        
                    }
                cn.Close();
                DGVPrinter printer = new DGVPrinter();
                    printer.Title = "Fatura";
                    printer.SubTitle = string.Format("Dia: {0} Hora: {1}",label2.Text, label3.Text);
                    printer.SubTitleFormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
                    printer.PageNumbers = true;
                    printer.PageNumberInHeader = false;
                    printer.PorportionalColumns = true;
                    printer.HeaderCellAlignment = StringAlignment.Near;
                    printer.Footer = "Total: "+ label1.Text + "\r\n"+" Obrigado por a sua compra!" + "\r\n "+ "Fatura processada por Stocker!";
                    printer.FooterSpacing=15;
                    printer.PrintDataGridView(dataGridView1);                 
                    _tot = 0.00;
                    label1.Text = _tot.ToString("#,##0.00") + "€";
                    dataGridView1.Rows.Clear();
                    Limparflowpanel();
                    carregarprodutos();
                
            }
            catch (Exception ex )
            {
                System.Windows.MessageBox.Show(ex.ToString());
            }
            
        }

    }
}
