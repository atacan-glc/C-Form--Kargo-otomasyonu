using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace sahalara_geri_döndüm_ye
{
    public partial class admin : Form
    {
        public admin()
        {
            InitializeComponent();
        }
        int i = 0;

        private static OleDbConnection GetBaglanti()
        {
            string baglantiyolu =
                "Provider=Microsoft.Jet.OLEDB.4.0;data source=" +
                Application.StartupPath + "\\kargo.mdb";

            OleDbConnection baglanti = new OleDbConnection(baglantiyolu);
            return baglanti;
        }

        private void admin_Load(object sender, EventArgs e)
        {
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OleDbConnection baglanti = new OleDbConnection("provider=microsoft.jet.oledb.4.0;Data Source=kargo_programi.mdb");
                baglanti.Open();

                DataTable dt = new DataTable();
                OleDbDataAdapter adap = new OleDbDataAdapter("SELECT * FROM [destek]", baglanti);
                adap.Fill(dt);
                dataGridView1.DataSource = dt;



                baglanti.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }


        private void admin_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
          }

        private void button3_Click(object sender, EventArgs e)
        {


            try
            {
                using (OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=kargo_programi.mdb"))
                {
                    baglanti.Open();

                    DataTable dt = new DataTable();
                    OleDbDataAdapter adap = new OleDbDataAdapter("SELECT * FROM kargo", baglanti);
                    adap.Fill(dt);

                    // Kolon adlarını görmek için (debug amaçlı):
                    string kolonlar = string.Join(", ", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                    

                    dataGridView1.AutoGenerateColumns = true; //  oluşturma zorla
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = dt;

                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                OleDbConnection baglanti = new OleDbConnection("provider=microsoft.jet.oledb.4.0;Data Source=kargo_programi.mdb");
                baglanti.Open();

                DataTable dt = new DataTable();
                OleDbDataAdapter adap = new OleDbDataAdapter("SELECT * FROM uyeler", baglanti);
                adap.Fill(dt);
                dataGridView1.DataSource = dt;

                dataGridView1.Columns[0].Visible = false; // ID  alanı gizliyoruz

                baglanti.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                using (OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=kargo_programi.mdb"))
                {
                    baglanti.Open();

                    DataTable dt = new DataTable();
                    OleDbDataAdapter adap = new OleDbDataAdapter("SELECT * FROM destek", baglanti);
                    adap.Fill(dt);

                    // Kolon adlarını görmek 
                    string kolonlar = string.Join(", ", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                   

                    dataGridView1.AutoGenerateColumns = true; //  oluşturma zorla
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = dt;

                    // Talep_ID  zorla görünür yap
                    if (dataGridView1.Columns.Contains("talep_id"))
                    {
                        dataGridView1.Columns["talep_id"].Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            string uyeID = textBox3.Text.Trim();

            if (string.IsNullOrWhiteSpace(uyeID))
            {
                MessageBox.Show("Lütfen bir Üye ID girin.");
                return;
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection("provider=microsoft.jet.oledb.4.0;Data Source=kargo_programi.mdb"))
                {
                    conn.Open();

                    string query = "SELECT * FROM uyeler WHERE uyeid = ?";
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.Add("?", OleDbType.Integer).Value = Convert.ToInt32(uyeID);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Düzenle
                                textBox4.Text = reader["adsoyad"].ToString();    
                                textBox5.Text = reader["mail"].ToString();  
                                textBox6.Text = reader["tcno"].ToString();  
                                textBox8.Text = reader["sifre"].ToString(); 
                                                                              
                            }
                            else
                            {
                                MessageBox.Show("Belirtilen ID ile eşleşen kullanıcı bulunamadı.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string takipNo = textBox9.Text.Trim();
            string yeniDurum = comboBox2.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(takipNo) || string.IsNullOrEmpty(yeniDurum))
            {
                MessageBox.Show("Takip numarası ve durum seçilmelidir.");
                return;
            }

            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=kargo_programi.mdb";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "UPDATE kargo SET durum = ? WHERE takip_no = ?";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", yeniDurum);
                        cmd.Parameters.AddWithValue("?", takipNo);

                        int affectedRows = cmd.ExecuteNonQuery();

                        if (affectedRows > 0)
                        {
                            MessageBox.Show("Durum başarıyla güncellendi.");
                        }
                        else
                        {
                            MessageBox.Show("Takip numarası bulunamadı.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata oluştu: " + ex.Message);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox11.Text))
            {
                MessageBox.Show("Lütfen bir ID giriniz.");
                return;
            }

            int id;
            if (!int.TryParse(textBox11.Text.Trim(), out id))
            {
                MessageBox.Show("ID sayısal olmalıdır.");
                return;
            }

            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=kargo_programi.mdb";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT konu, mesaj FROM destek WHERE talep_id = ?";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", id);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                textBox1.Text = reader["konu"].ToString();
                                textBox2.Text = reader["mesaj"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Bu ID'ye ait destek kaydı bulunamadı.");
                                textBox1.Text = "";
                                textBox2.Text = "";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı hatası: " + ex.Message);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {

        {
            string uyeID = textBox3.Text.Trim();
            if (string.IsNullOrWhiteSpace(uyeID))
            {
                MessageBox.Show("Lütfen güncellemek için bir Üye ID girin.");
                return;
            }

            List<string> alanlar = new List<string>();
            List<object> degerler = new List<object>();

            if (!string.IsNullOrWhiteSpace(textBox4.Text))
            {
                alanlar.Add("adsoyad = ?");
                degerler.Add(textBox4.Text.Trim());
            }
            if (!string.IsNullOrWhiteSpace(textBox5.Text))
            {
                alanlar.Add("mail = ?");
                degerler.Add(textBox5.Text.Trim());
            }
            if (!string.IsNullOrWhiteSpace(textBox6.Text))
            {
                alanlar.Add("Tc = ?");
                degerler.Add(textBox6.Text.Trim());
            }
            if (!string.IsNullOrWhiteSpace(textBox8.Text))
            {
                alanlar.Add("telefon = ?");
                degerler.Add(textBox8.Text.Trim());
            }

            if (alanlar.Count == 0)
            {
                MessageBox.Show("Güncellemek için en az bir alan doldurun.");
                return;
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection("provider=microsoft.jet.oledb.4.0;Data Source=kargo_programi.mdb"))
                {
                    conn.Open();
                    string sorgu = $"UPDATE uyeler SET {string.Join(", ", alanlar)} WHERE uyeid = ?";
                    using (OleDbCommand cmd = new OleDbCommand(sorgu, conn))
                    {
                        foreach (var deger in degerler)
                        {
                            cmd.Parameters.AddWithValue("?", deger);
                        }
                        cmd.Parameters.AddWithValue("?", Convert.ToInt32(uyeID));

                        int sonuc = cmd.ExecuteNonQuery();

                        if (sonuc > 0)
                            MessageBox.Show("Bilgiler başarıyla güncellendi.");
                        else
                            MessageBox.Show("Güncellenecek kullanıcı bulunamadı.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        }

        private void button9_Click(object sender, EventArgs e)
        {
            string uyeID = textBox3.Text.Trim();

            if (string.IsNullOrWhiteSpace(uyeID))
            {
                MessageBox.Show("Lütfen silmek için bir Üye ID girin.");
                return;
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection("provider=microsoft.jet.oledb.4.0;Data Source=kargo_programi.mdb"))
                {
                    conn.Open();
                    string query = "DELETE FROM uyeler WHERE uyeid = ?";
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.Add("?", OleDbType.Integer).Value = Convert.ToInt32(uyeID);
                        int silinen = cmd.ExecuteNonQuery();

                        if (silinen > 0)
                        {
                            MessageBox.Show("Kullanıcı başarıyla silindi.");
                            // TextBox'ları temizle
                            textBox4.Text = "";
                            textBox5.Text = "";
                            textBox6.Text = "";
                            textBox8.Text = "";
                            textBox3.Text = "";
                        }
                        else
                        {
                            MessageBox.Show("Silinecek kullanıcı bulunamadı.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
    }
}

