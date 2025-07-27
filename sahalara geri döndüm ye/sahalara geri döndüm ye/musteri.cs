using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sahalara_geri_döndüm_ye
{
    public partial class musteri : Form
    {
        public musteri()
        {
            InitializeComponent();
        }
        public musteri(string kullaniciMail)
        {
            InitializeComponent();
            girisYapanMail = kullaniciMail;
        }
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        private void Form1_Load(object sender, EventArgs e)

        {
            comboBox2.Items.Add("Standart Teslimat");
            comboBox2.Items.Add("Hızlı Teslimat");
           
            comboBox2.SelectedIndex = 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog1 = new ColorDialog();

            colorDialog1.FullOpen = true;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {

                this.BackColor = colorDialog1.Color;
            }
        }
        private string YeniKargoNoUret()
        {
            Random rnd = new Random();
            int sayi = rnd.Next(1000000, 9999999); // 7 haneli sayı üretir
            return sayi.ToString();
        }
        
        private void button2_Click(object sender, EventArgs e)
        {


            try
            {
                using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=kargo_programi.mdb"))
                {
                    conn.Open();

                    string takipNo = YeniKargoNoUret();

                    OleDbCommand cmd = new OleDbCommand("INSERT INTO kargo (takip_no, teslim_tipi, gonderilen_tarih, odeme_tipi, a_adsoyad, a_telefon, alici_adresi, g_adsoyad, g_telefon, gonderen_adres, mail) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", conn);

                    cmd.Parameters.AddWithValue("?", takipNo);
                    cmd.Parameters.AddWithValue("?", comboBox2.Text);
                    cmd.Parameters.AddWithValue("?", dateTimePicker1.Value);
                    cmd.Parameters.AddWithValue("?", comboBox3.Text);
                    cmd.Parameters.AddWithValue("?", textBox6.Text);
                    cmd.Parameters.AddWithValue("?", textBox7.Text);
                    cmd.Parameters.AddWithValue("?", textBox2.Text);
                    cmd.Parameters.AddWithValue("?", textBox4.Text);
                    cmd.Parameters.AddWithValue("?", textBox3.Text);
                    cmd.Parameters.AddWithValue("?", textBox5.Text);
                    cmd.Parameters.AddWithValue("?", girisYapanMail ?? "");

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Kargo gönderildi! Takip No: " + takipNo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }


        }

        public partial class Form1 : Form
        {
            private static Form1 instance;

            public static Form1 GetInstance()
            {
                if (instance == null || instance.IsDisposed)
                    instance = new Form1();
                return instance;
            }
        }

      

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker1.MinDate = DateTime.Today;
        }

        private void destekMesajıToolStripMenuItem_Click(object sender, EventArgs e)
        {
            groupBox5.Visible = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string konu = textBox1.Text.Trim();
            string mesaj = richTextBox1.Text.Trim();
            string gonderen_mail = girisYapanMail;

            if (string.IsNullOrWhiteSpace(konu) || string.IsNullOrWhiteSpace(mesaj))
            {
                MessageBox.Show("Lütfen konu ve mesaj alanlarını doldurun.");
                return;
            }

            try
            {
                using (OleDbConnection baglanti = new OleDbConnection("provider=microsoft.jet.oledb.4.0;Data Source=kargo_programi.mdb"))
                {
                    baglanti.Open();

                    string query = "INSERT INTO destek (konu, mesaj, gonderen_mail, d_tarih) VALUES (?, ?, ?, ?)";

                    using (OleDbCommand cmd = new OleDbCommand(query, baglanti))
                    {
                        cmd.Parameters.Add("konu", OleDbType.VarChar).Value = konu;
                        cmd.Parameters.Add("mesaj", OleDbType.VarChar).Value = mesaj;
                        cmd.Parameters.Add("gonderen_mail", OleDbType.VarChar).Value = gonderen_mail;
                        cmd.Parameters.Add("d_tarih", OleDbType.Date).Value = DateTime.Now;

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Destek talebiniz başarıyla gönderildi.");
                }

                // Temizle ve gizle
                textBox1.Text = "";
                richTextBox1.Text = "";
                groupBox5.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }


        }
        string girisYapanMail;



       

        private void musteri_Load(object sender, EventArgs e)
        {
            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=kargo_programi.mdb"))
                {
                    conn.Open();

                    string query = "SELECT * FROM kargo WHERE mail = ?";
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", girisYapanMail); // Giriş yapan maili kullanıyoruz

                        OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }

        private void bilgilerimiGüncelleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            groupBox6.Visible = true; // GroupBox görünür hale gelir

            string baglantiYolu = "Provider=Microsoft.JET.OLEDB.4.0;Data Source=kargo_programi.mdb";
            using (OleDbConnection baglanti = new OleDbConnection(baglantiYolu))
            {
                try
                {
                    baglanti.Open();

                    string sorgu = "SELECT uyeid FROM uyeler WHERE mail = ?";
                    using (OleDbCommand komut = new OleDbCommand(sorgu, baglanti))
                    {
                        komut.Parameters.AddWithValue("?", girisYapanMail); // Giriş yapan mail

                        using (OleDbDataReader reader = komut.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                textBox12.Text = reader["uyeid"].ToString(); // Sadece ID yazdır
                            }
                            else
                            {
                                MessageBox.Show("Kullanıcı bulunamadı.");
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

        private void button8_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox12.Text))
            {
                MessageBox.Show("Lütfen geçerli bir Üye ID giriniz.");
                return;
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;Data Source=kargo_programi.mdb"))
                {
                    conn.Open();

                    string query = "UPDATE uyeler SET adsoyad = ?, tcno = ?, mail = ?, sifre = ? WHERE uyeid = ?";
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", textBox11.Text.Trim());
                        cmd.Parameters.AddWithValue("?", textBox9.Text.Trim());
                        cmd.Parameters.AddWithValue("?", textBox10.Text.Trim());
                        cmd.Parameters.AddWithValue("?", textBox8.Text.Trim());
                        cmd.Parameters.AddWithValue("?", Convert.ToInt32(textBox12.Text));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Bilgiler başarıyla güncellendi.");
                        }
                        else
                        {
                            MessageBox.Show("Güncelleme başarısız. Belirtilen ID bulunamadı.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox12.Text))
            {
                MessageBox.Show("Silinecek kullanıcıya ait Üye ID bulunamadı.");
                return;
            }

            DialogResult onay = MessageBox.Show("Bu üyeliği silmek istediğinize emin misiniz?", "Üyelik Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (onay != DialogResult.Yes) return;

            try
            {
                using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;Data Source=kargo_programi.mdb"))
                {
                    conn.Open();

                    string silSorgu = "DELETE FROM uyeler WHERE uyeid = ?";
                    using (OleDbCommand cmd = new OleDbCommand(silSorgu, conn))
                    {
                        cmd.Parameters.AddWithValue("?", Convert.ToInt32(textBox12.Text));
                        int silinen = cmd.ExecuteNonQuery();

                        if (silinen > 0)
                        {
                            MessageBox.Show("Üyeliğiniz silinmiştir. Uygulama kapatılıyor...");
                            Application.Exit(); // Programı tamamen kapat
                        }
                        else
                        {
                            MessageBox.Show("Silinecek kayıt bulunamadı.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }
    }
    }
    
