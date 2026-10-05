using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Contoh_Soal
{
    /// <summary>
    /// Form Login: meminta username & password.
    /// Jika berhasil, menampilkan FormAdminMain.
    /// </summary>
    public class FormLogin : Form
    {
        private Label lblLogo;      // logo pesawat
        private Label lblJudul;     // "BROMO AIRLINES"
        private Label lblUsername;
        private Label lblPassword;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private LinkLabel lnkDaftar;

        public FormLogin()
        {
            InisialisasiKomponen();
            this.Text = "Login - Bromo Airlines";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormClosed += (s, e) => Application.Exit();
        }

        private void InisialisasiKomponen()
        {
            // --- Logo & judul ---
            lblLogo = new Label
            {
                Text = "✈",
                Font = new Font("Segoe UI", 40F),
                ForeColor = Color.FromArgb(0, 102, 204),
                AutoSize = false,
                Width = 300,
                Height = 70,
                Top = 20,
                Left = 50,
                TextAlign = ContentAlignment.MiddleCenter
            };
            lblJudul = new Label
            {
                Text = "BROMO AIRLINES",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 51, 102),
                AutoSize = false,
                Width = 300,
                Height = 30,
                Top = 90,
                Left = 50,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // --- Input username ---
            lblUsername = new Label { Text = "Username", Top = 140, Left = 50, AutoSize = true };
            txtUsername = new TextBox { Top = 160, Left = 50, Width = 300 };

            // --- Input password ---
            lblPassword = new Label { Text = "Password", Top = 195, Left = 50, AutoSize = true };
            txtPassword = new TextBox { Top = 215, Left = 50, Width = 300, PasswordChar = '●' };

            // --- Tombol Login ---
            btnLogin = new Button
            {
                Text = "Login",
                Top = 255,
                Left = 50,
                Width = 300,
                Height = 32,
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += BtnLogin_Click;

            // --- Link daftar akun: "Belum punya akun? Daftar Sekarang" ---
            var lblBelum = new Label { Text = "Belum punya akun?", Top = 300, Left = 50, AutoSize = true };
            Controls.Add(lblBelum);
            lnkDaftar = new LinkLabel
            {
                Text = "Daftar Sekarang",
                Top = 298,
                Left = 165,
                AutoSize = true
            };
            lnkDaftar.LinkClicked += (s, e) =>
            {
                // Buka form daftar akun secara dialog.
                // Saat form daftar ditutup (tombol X atau link 'Login'), user kembali ke login.
                using (var daftar = new FormDaftar())
                {
                    // Setelah form daftar ditutup (berhasil daftar / tombol X / link 'Login'),
                    // user kembali ke Login Form untuk login.
                    daftar.ShowDialog(this);
                }
            };

            Controls.AddRange(new Control[] { lblLogo, lblJudul, lblUsername, txtUsername,
                                              lblPassword, txtPassword, btnLogin, lblBelum, lnkDaftar });
            this.ClientSize = new Size(400, 340);
            this.MaximizeBox = false;
        }

        // Validasi login: username & password harus cocok dengan data akun
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (username == "" || password == "")
            {
                MessageBox.Show("Username dan Password wajib diisi!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Akun akun = DataStore.AkunList.FirstOrDefault(a =>
                a.Username == username && a.Password == password);

            if (akun == null)
            {
                MessageBox.Show("Username atau Password salah!", "Login Gagal",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Berhasil login -> arahkan langsung ke Admin Main Form
            var admin = new FormAdminMain();
            admin.OnLogout += () =>
            {
                admin.Close();   // tutup admin
                this.Show();     // kembali ke login
                txtPassword.Clear();
            };
            admin.Show();
            this.Hide();
        }
    }
}
