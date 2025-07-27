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

namespace sahalara_geri_döndüm_ye
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private static Form1 instance;

        public static Form1 GetInstance()
        {
            if (instance == null || instance.IsDisposed)
                instance = new Form1();
            return instance;
        }
        int kontrol = 0;
        public static string girismail;
        string girissifre;
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            groupbox_teslimat_bilgileri.Hide();
            apanel_resim.Hide();
            groupbox_uye_olma.Show();
        }
        public void kaydet()
        {
            string baglantiyolu = "Provider=Microsoft.JET.OLEDB.4.0;Data Source=kargo_programi.mdb";
            using (OleDbConnection baglanti = new OleDbConnection(baglantiyolu))
            {

                {
                    try
                    {
                        baglanti.Open();

                        // TC 11 haneli ve sayı mı?
                        if (uye_ol_tc.Text.Length != 11 || !long.TryParse(uye_ol_tc.Text, out _))
                        {
                            MessageBox.Show("TC kimlik numarası 11 haneli ve sadece sayılardan oluşmalıdır.");
                            return;
                        }

                        // Kayıt işlemi
                        string ekleSorgu = "INSERT INTO uyeler(mail, adsoyad, tcno, dogum_yili, sifre) VALUES (?, ?, ?, ?, ?)";
                        OleDbCommand ekleKomut = new OleDbCommand(ekleSorgu, baglanti);
                        ekleKomut.Parameters.AddWithValue("?", uye_ol_mail.Text.Trim());
                        ekleKomut.Parameters.AddWithValue("?", uye_ol_ad_soyad.Text.Trim());
                        ekleKomut.Parameters.AddWithValue("?", uye_ol_tc.Text.Trim());
                        ekleKomut.Parameters.AddWithValue("?", uye_ol_dogum_yili.Text.Trim());
                        ekleKomut.Parameters.AddWithValue("?", uye_sifre.Text.Trim());

                        ekleKomut.ExecuteNonQuery();

                        OleDbCommand idKomut = new OleDbCommand("SELECT @@IDENTITY", baglanti);
                        object idSonuc = idKomut.ExecuteScalar();

                        if (idSonuc != null && long.TryParse(idSonuc.ToString(), out long yeniUyeID))
                        {
                            MessageBox.Show("Üye kaydı başarıyla tamamlandı! Size Ait Üye ID: " + yeniUyeID);
                        }
                        else
                        {
                            MessageBox.Show("Üye kaydı yapıldı ancak ID alınamadı.");
                        }

                        groupbox_uye_olma.Hide();
                        apanel_resim.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata oluştu: " + ex.Message);
                    }
                    finally
                    {
                        baglanti.Close();

                        // Formu temizle
                        uye_ol_ad_soyad.Text = "";
                        uye_ol_dogum_yili.Text = "";
                        uye_ol_mail.Text = "";
                        uye_ol_mail_tekrar.Text = "";
                        uye_ol_tc.Text = "";
                        uye_sifre.Text = "";
                        uye_sifre_tekrar.Text = "";
                    }
                }   }
            }
            

        private void button2_Click(object sender, EventArgs e)
        {
            string kontrolmail = e_mail_uye.Text.Trim();
            string kontrolsifre = sifre_uye.Text.Trim();

            // Boş giriş kontrolü
            if (string.IsNullOrEmpty(kontrolmail) || string.IsNullOrEmpty(kontrolsifre))
            {
                label17.Text = "Lütfen e-posta ve şifrenizi girin.";
                return;
            }

            bool girisBasarili = false;

            using (OleDbConnection baglanti = new OleDbConnection("Provider=Microsoft.JET.OLEDB.4.0;Data Source=kargo_programi.mdb"))
            {
                baglanti.Open();

                string sorgu = "SELECT * FROM uyeler WHERE mail = ? AND sifre = ?";
                OleDbCommand komut = new OleDbCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("?", kontrolmail);
                komut.Parameters.AddWithValue("?", kontrolsifre);

                OleDbDataReader reader = komut.ExecuteReader();
                if (reader.Read())
                {
                    girisBasarili = true;
                }

                reader.Close();
                baglanti.Close();
            }

            if (girisBasarili)
            {
                this.Hide();
                musteri frm = new musteri(kontrolmail);
                frm.ShowDialog();
                this.Close();
            }
            else
            {
                label17.Text = "Hatalı e-posta veya şifre!";
            }


        }
        

        private void button8_Click(object sender, EventArgs e)
        {
            Form1 frm1 = new Form1();
            string kontrolmail = textBox3.Text;
            string kontrolsifre = textBox2.Text;
            OleDbConnection baglanti = new OleDbConnection("provider = microsoft.jet.oledb.4.0; Data Source = kargo_programi.mdb");
            baglanti.Open();
            OleDbCommand okuma = new OleDbCommand("select * from admin where username= '" + textBox3.Text + "' ", baglanti);
            OleDbDataReader reader = okuma.ExecuteReader();
            while (reader.Read())
            {
                girismail = reader["username"].ToString();
                girissifre = reader["password"].ToString();
            }
            baglanti.Close();
            if (girismail == kontrolmail && girissifre == kontrolsifre)
            {
                this.Hide();
                admin must = new admin();
                must.Show();
            }
            else
            {
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
            groupbox_teslimat_bilgileri.Visible = true;
            apanel_resim.Hide();
            groupbox_uye_olma.Show();

            try
            {
                OleDbConnection baglanti = new OleDbConnection("provider=microsoft.jet.oledb.4.0;Data Source=kargo_programi.mdb");
                baglanti.Open();

                DataTable dt = new DataTable();
                OleDbCommand komut = new OleDbCommand("SELECT * FROM kargo WHERE takip_no = ?", baglanti);
                komut.Parameters.AddWithValue("?", textBox1.Text);

                OleDbDataAdapter adap = new OleDbDataAdapter(komut);
                adap.Fill(dt);
                dataGridView1.DataSource = dt;

                dataGridView1.Columns[0].Visible = false;

                if (dt.Rows.Count == 0) // Eğer veri yoksa
                {
                    MessageBox.Show("Takip numarasına ait kayıt bulunamadı.");
                }

                baglanti.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Lütfen iki şehir seçiniz.");
                return;
            }

            if (comboBox1.Text == comboBox2.Text)
            {
                MessageBox.Show("Aynı şehirler arasında gönderim yapılamaz.");
                return;
            }

            Random rnd = new Random();
            int standartSure = rnd.Next(3, 6);  // 3 - 5 gün
            int hizliSure = rnd.Next(1, 3);     // 1 - 2 gün

            string mesaj = $"Standart Teslimat Süresi: {standartSure} gün\n" +
                           $"Hızlı Teslimat Süresi: {hizliSure} gün";

            MessageBox.Show(mesaj, "Teslimat Süresi", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void button5_Click(object sender, EventArgs e)
        {

            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Lütfen iki şehir seçiniz.");
                return;
            }

            if (comboBox1.Text == comboBox2.Text)
            {
                MessageBox.Show("Aynı şehirler arasında gönderim yapılamaz.");
                return;
            }

            Random rnd = new Random();
            int standartFiyat = rnd.Next(50, 101);  // 50 - 100 TL
            int hizliFiyat = rnd.Next(100, 151);    // 100 - 150 TL

            string mesaj = $"Standart Teslimat Fiyatı: {standartFiyat} TL\n" +
                           $"Hızlı Teslimat Fiyatı: {hizliFiyat} TL";

            MessageBox.Show(mesaj, "Fiyat Bilgisi", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        

        private void button6_Click(object sender, EventArgs e)
        {
            if (uye_ol_mail.Text.Contains("@") && uye_ol_mail.Text.Contains(".com"))
            {
                if (uye_ol_mail.Text==uye_ol_mail_tekrar.Text&&uye_sifre.Text==uye_sifre_tekrar.Text)
                {
                    kaydet();
                }
                else
                {
                    MessageBox.Show("Lütfen Mail ve Şifrelerinizi Kontrol Ediniz!");
                }
            }
            else
            {
                MessageBox.Show("Lütfen Mail Adresinizi Doğru Giriniz!");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                uye_sifre.PasswordChar = '\0';
                uye_sifre_tekrar.PasswordChar = '\0';
            }
            else
            {
                uye_sifre.PasswordChar = '*';
                uye_sifre_tekrar.PasswordChar = '*';

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

          
            groupbox_teslimat_bilgileri.Hide();
          
            groupbox_uye_olma.Hide();

            foreach (var il in iller)
            {
                comboBox1.Items.Add(il);
                comboBox2.Items.Add(il);
            }

        }
        private static Form1 _instance;

     
        List<string> iller = new List<string>()
        {
    "İstanbul", "Ankara", "İzmir", "Bursa", "Antalya",
    "Konya", "Adana", "Gaziantep", "Şanlıurfa", "Kocaeli",
    "Mersin", "Kayseri", "Diyarbakır", "Samsun", "Eskişehir",
    "Denizli", "Erzurum", "Malatya", "Balıkesir", "Manisa"
        };

        private void button7_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = true;
        }
    }
}
