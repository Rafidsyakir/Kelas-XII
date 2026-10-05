using System;
using System.Drawing;
using System.Windows.Forms;

namespace Contoh_Soal
{
    /// <summary>
    /// Admin Main Form: dashboard dengan side navigation bar yang bisa
    /// dilebarkan/dilebar-kecilkan lewat tombol hamburger.
    /// </summary>
    public class FormAdminMain : Form
    {
        public event Action OnLogout; // event saat tombol Logout ditekan

        private Panel panelNav;       // panel navigasi samping
        private Panel panelContent;   // area konten tempat form dimuat
        private Button btnHamburger;  // tombol untuk lebarkan/kecilkan nav
        private bool navExpanded = true;
        private const int NAV_LEBAR = 220;
        private const int NAV_KECIL = 50;

        public FormAdminMain()
        {
            InisialisasiKomponen();
            this.Text = "Admin Dashboard - Bromo Airlines";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new Size(1000, 600);
            this.FormClosed += (s, e) =>
            {
                // Jika ditutup langsung (bukan via Logout), keluar dari aplikasi
                if (!_logoutDipanggil) Application.Exit();
            };
        }

        private bool _logoutDipanggil = false;

        private void InisialisasiKomponen()
        {
            // --- Panel konten (area form yang dimuat) ---
            panelContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            // --- Panel navigasi samping ---
            panelNav = new Panel
            {
                Dock = DockStyle.Left,
                Width = NAV_LEBAR,
                BackColor = Color.FromArgb(0, 51, 102)
            };

            // --- Tombol hamburger ---
            btnHamburger = new Button
            {
                Text = "☰",
                Font = new Font("Segoe UI", 14F),
                Dock = DockStyle.Top,
                Height = 45,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 38, 77)
            };
            btnHamburger.FlatAppearance.BorderSize = 0;
            btnHamburger.Click += (s, e) =>
            {
                // Toggle lebar navigasi
                navExpanded = !navExpanded;
                panelNav.Width = navExpanded ? NAV_LEBAR : NAV_KECIL;
            };
            panelNav.Controls.Add(btnHamburger);

            // --- Tombol-tombol menu ---
            string[] menu =
            {
                "Master Bandara",
                "Master Maskapai",
                "Master Jadwal Penerbangan",
                "Master Kode Promo",
                "Ubah Status Penerbangan",
                "Logout"
            };

            // Ditambahkan terbalik agar tombol pertama muncul paling atas
            for (int i = menu.Length - 1; i >= 0; i--)
            {
                string namaMenu = menu[i];
                var btn = new Button
                {
                    Text = "  " + namaMenu,
                    Dock = DockStyle.Top,
                    Height = 42,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(0, 51, 102),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 10F)
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 77, 153);
                btn.Click += (s, e) => KlikMenu(namaMenu);
                panelNav.Controls.Add(btn);
                btn.BringToFront(); // tombol hamburger tetap di atas
            }

            Controls.Add(panelContent);
            Controls.Add(panelNav);
        }

        // Memuat form sesuai menu yang ditekan ke panel konten
        private void KlikMenu(string namaMenu)
        {
            if (namaMenu == "Logout")
            {
                _logoutDipanggil = true;
                OnLogout?.Invoke();
                return;
            }

            Form form = null;
            switch (namaMenu)
            {
                case "Master Bandara": form = new FormMasterBandara(); break;
                case "Master Maskapai": form = new FormMasterMaskapai(); break;
                case "Master Jadwal Penerbangan": form = new FormMasterJadwal(); break;
                case "Master Kode Promo": form = new FormMasterPromo(); break;
                case "Ubah Status Penerbangan": form = new FormUbahStatus(); break;
            }

            if (form == null) return;

            // Hosting form di dalam panel konten
            panelContent.Controls.Clear();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            panelContent.Controls.Add(form);
            form.Show();
            form.BringToFront();
        }
    }
}
