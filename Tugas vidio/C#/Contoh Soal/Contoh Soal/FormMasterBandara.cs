using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Contoh_Soal
{
    /// <summary>
    /// Form Master Bandara: CRUD data bandara.
    /// Data diurutkan A-Z berdasarkan Nama.
    /// </summary>
    public class FormMasterBandara : Form
    {
        private DataGridView dgv;
        private TextBox txtNama, txtKodeIATA, txtKota, txtAlamat;
        private ComboBox cmbNegara;
        private NumericUpDown nudTerminal;
        private Button btnBatal, btnSimpan;
        private Bandara dataEdit = null; // null = mode tambah, terisi = mode ubah

        public FormMasterBandara()
        {
            InisialisasiKomponen();
            MuatData();
        }

        private void InisialisasiKomponen()
        {
            // --- Judul ---
            var lblJudul = new Label
            {
                Text = "Master Bandara",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // --- Tabel data ---
            dgv = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 250,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            dgv.Columns.Add("Nama", "Nama");
            dgv.Columns.Add("KodeIATA", "Kode IATA");
            dgv.Columns.Add("Kota", "Kota");
            dgv.Columns.Add("Negara", "Negara");
            dgv.Columns.Add("JumlahTerminal", "Jumlah Terminal");
            dgv.Columns.Add("Alamat", "Alamat");
            dgv.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnUbah", HeaderText = "Aksi", Text = "Ubah",
                UseColumnTextForButtonValue = true, Width = 60
            });
            dgv.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnHapus", HeaderText = "Aksi", Text = "Hapus",
                UseColumnTextForButtonValue = true, Width = 60
            });
            dgv.CellClick += Dgv_CellClick;

            // --- Panel input ---
            var panelInput = new Panel { Dock = DockStyle.Fill, Padding = new Padding(5) };

            int x1 = 10, x2 = 230, x3 = 450;
            int w = 200, y = 10;
            TextBox TextRow(Panel p, string label, int left, int top)
            {
                p.Controls.Add(new Label { Text = label, Left = left, Top = top, AutoSize = true });
                var t = new TextBox { Left = left, Top = top + 22, Width = w };
                p.Controls.Add(t);
                return t;
            }

            txtNama = TextRow(panelInput, "Nama", x1, y);
            txtKodeIATA = TextRow(panelInput, "Kode IATA (3 huruf)", x2, y);
            txtKota = TextRow(panelInput, "Kota", x3, y);
            y += 55;
            cmbNegara = new ComboBox
            {
                Left = x1, Top = y + 22, Width = w, DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbNegara.DataSource = new List<string>(DataStore.Countries);
            panelInput.Controls.Add(new Label { Text = "Negara", Left = x1, Top = y, AutoSize = true });
            panelInput.Controls.Add(cmbNegara);

            panelInput.Controls.Add(new Label { Text = "Jumlah Terminal", Left = x2, Top = y, AutoSize = true });
            nudTerminal = new NumericUpDown { Left = x2, Top = y + 22, Width = w, Minimum = 0, Maximum = 100, Value = 1 };
            panelInput.Controls.Add(nudTerminal);

            // Alamat memakai textbox besar (multiline)
            panelInput.Controls.Add(new Label { Text = "Alamat", Left = x3, Top = y, AutoSize = true });
            txtAlamat = new TextBox
            {
                Left = x3, Top = y + 22, Width = w, Height = 90,
                Multiline = true, ScrollBars = ScrollBars.Vertical
            };
            panelInput.Controls.Add(txtAlamat);
            y += 125;

            btnBatal = new Button { Text = "Batal", Left = x1, Top = y, Width = 120, Height = 30 };
            btnBatal.Click += (s, e) => BersihkanForm();
            btnSimpan = new Button
            {
                Text = "Simpan", Left = x1 + 130, Top = y, Width = 120, Height = 30,
                BackColor = Color.FromArgb(0, 102, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat
            };
            btnSimpan.FlatAppearance.BorderSize = 0;
            btnSimpan.Click += BtnSimpan_Click;
            panelInput.Controls.Add(btnBatal);
            panelInput.Controls.Add(btnSimpan);

            Controls.Add(panelInput);
            Controls.Add(dgv);
            Controls.Add(lblJudul);
        }

        // Muat ulang tabel, diurutkan A-Z berdasarkan Nama
        private void MuatData()
        {
            dgv.Rows.Clear();
            foreach (var b in DataStore.BandaraList.OrderBy(b => b.Nama))
            {
                dgv.Rows.Add(b.Nama, b.KodeIATA, b.Kota, b.Negara, b.JumlahTerminal, b.Alamat, "Ubah", "Hapus");
            }
        }

        // Klik tombol Ubah / Hapus pada sebuah baris
        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var b = DataStore.BandaraList.OrderBy(x => x.Nama).ElementAt(e.RowIndex);

            if (dgv.Columns[e.ColumnIndex].Name == "btnUbah")
            {
                // Muat data ke kolom input untuk diedit
                dataEdit = b;
                txtNama.Text = b.Nama;
                txtKodeIATA.Text = b.KodeIATA;
                txtKota.Text = b.Kota;
                cmbNegara.Text = b.Negara;
                nudTerminal.Value = b.JumlahTerminal;
                txtAlamat.Text = b.Alamat;
            }
            else if (dgv.Columns[e.ColumnIndex].Name == "btnHapus")
            {
                // Konfirmasi hapus
                var hasil = MessageBox.Show($"Hapus bandara '{b.Nama}'?", "Konfirmasi",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (hasil == DialogResult.Yes)
                {
                    DataStore.BandaraList.Remove(b);
                    MuatData();
                    BersihkanForm();
                }
            }
        }

        private void BtnSimpan_Click(object sender, EventArgs e)
        {
            // Ambil & bersihkan input
            string nama = txtNama.Text.Trim();
            string kode = txtKodeIATA.Text.Trim().ToUpper();
            string kota = txtKota.Text.Trim();
            string negara = cmbNegara.Text.Trim();
            int terminal = (int)nudTerminal.Value;
            string alamat = txtAlamat.Text.Trim();

            // Validasi kelengkapan
            if (nama == "" || kode == "" || kota == "" || negara == "" || alamat == "")
            {
                MessageBox.Show("Semua field wajib diisi!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi: nama unik
            if (DataStore.BandaraList.Any(b => b != dataEdit &&
                b.Nama.Equals(nama, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Nama bandara sudah ada!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi: kode IATA 3 huruf & unik
            if (!Regex.IsMatch(kode, @"^[A-Z]{3}$"))
            {
                MessageBox.Show("Kode IATA harus 3 huruf!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (DataStore.BandaraList.Any(b => b != dataEdit &&
                b.KodeIATA.Equals(kode, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Kode IATA sudah digunakan!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi: jumlah terminal minimal 1
            if (terminal < 1)
            {
                MessageBox.Show("Jumlah Terminal minimal 1!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dataEdit == null)
            {
                // Tambah data baru
                DataStore.BandaraList.Add(new Bandara
                {
                    Nama = nama, KodeIATA = kode, Kota = kota,
                    Negara = negara, JumlahTerminal = terminal, Alamat = alamat
                });
            }
            else
            {
                // Ubah data yang sedang diedit
                dataEdit.Nama = nama;
                dataEdit.KodeIATA = kode;
                dataEdit.Kota = kota;
                dataEdit.Negara = negara;
                dataEdit.JumlahTerminal = terminal;
                dataEdit.Alamat = alamat;
            }

            MuatData();
            BersihkanForm();
        }

        // Reset form input ke keadaan awal
        private void BersihkanForm()
        {
            txtNama.Clear(); txtKodeIATA.Clear(); txtKota.Clear();
            cmbNegara.SelectedIndex = -1; txtAlamat.Clear();
            nudTerminal.Value = 1;
            dataEdit = null;
        }
    }
}
