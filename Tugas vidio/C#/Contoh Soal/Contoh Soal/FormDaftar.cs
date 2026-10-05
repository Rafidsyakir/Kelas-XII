using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Contoh_Soal
{
    /// <summary>
    /// Form Daftar Akun: membuat akun baru.
    /// Validasi: username unik, telepon 10-15 digit angka, password >= 8 karakter.
    /// </summary>
    public class FormDaftar : Form
    {
        private TextBox txtUsername, txtNama, txtTelepon, txtPassword;
        private DateTimePicker dtpTanggalLahir;
        private Button btnDaftar;
        private LinkLabel lnkLogin;


        public FormDaftar()
        {
            InisialisasiKomponen();
            this.Text = "Daftar Akun - Bromo Airlines";
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InisialisasiKomponen()
        {
            int y = 20;
            TextBox TambahBaris(string label, bool password = false)
            {
                var lbl = new Label { Text = label, Top = y, Left = 30, AutoSize = true };
                var txt = new TextBox { Top = y + 22, Left = 30, Width = 340 };
                if (password) txt.PasswordChar = '●';
                Controls.Add(lbl);
                Controls.Add(txt);
                y += 55;
                return txt;
            }

            txtUsername = TambahBaris("Username (unik)");
            txtNama = TambahBaris("Nama");

            var lblTgl = new Label { Text = "Tanggal Lahir", Top = y, Left = 30, AutoSize = true };
            dtpTanggalLahir = new DateTimePicker
            {
                Top = y + 22,
                Left = 30,
                Width = 340,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MM-yyyy" // format tanggal default DD-MM-YYYY
            };
            Controls.Add(lblTgl);
            Controls.Add(dtpTanggalLahir);
            y += 55;

            txtTelepon = TambahBaris("Nomor Telepon (10-15 digit angka)");
            txtPassword = TambahBaris("Password (minimal 8 karakter)", true);

            btnDaftar = new Button
            {
                Text = "Daftar",
                Top = y,
                Left = 30,
                Width = 340,
                Height = 32,
                BackColor = Color.FromArgb(0, 102, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnDaftar.FlatAppearance.BorderSize = 0;
            btnDaftar.Click += BtnDaftar_Click;
            Controls.Add(btnDaftar);
            y += 45;

            lnkLogin = new LinkLabel { Text = "Login", Top = y, Left = 30, Width = 340, TextAlign = ContentAlignment.MiddleCenter };
            lnkLogin.LinkClicked += (s, e) => this.Close(); // kembali ke form login
            Controls.Add(lnkLogin);

            this.ClientSize = new Size(400, y + 35);
            this.MaximizeBox = false;
        }

        private void BtnDaftar_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string nama = txtNama.Text.Trim();
            string telepon = txtTelepon.Text.Trim();
            string password = txtPassword.Text;

            // Validasi kelengkapan
            if (username == "" || nama == "" || telepon == "" || password == "")
            {
                MessageBox.Show("Semua field wajib diisi!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi username unik
            if (DataStore.AkunList.Any(a => a.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Username sudah digunakan!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi nomor telepon: hanya angka, 10-15 digit
            if (!Regex.IsMatch(telepon, @"^\d{10,15}$"))
            {
                MessageBox.Show("Nomor Telepon harus berupa angka dan 10-15 digit!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi password minimal 8 karakter
            if (password.Length < 8)
            {
                MessageBox.Show("Password minimal 8 karakter!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Simpan akun baru sebagai Customer (MerupakanAdmin = false)
            DataStore.AkunList.Add(new Akun
            {
                Username = username,
                Nama = nama,
                TanggalLahir = dtpTanggalLahir.Value.Date,
                Telepon = telepon,
                Password = password,
                MerupakanAdmin = false
            });

            MessageBox.Show("Pendaftaran berhasil! Silakan login.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close(); // tutup form daftar, kembali ke Login Form
        }
    }
}
