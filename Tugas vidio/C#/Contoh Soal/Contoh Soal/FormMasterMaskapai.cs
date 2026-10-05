using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Contoh_Soal
{
    /// <summary>
    /// Form Master Maskapai: CRUD data maskapai. Urut A-Z berdasarkan Nama.
    /// </summary>
    public class FormMasterMaskapai : Form
    {
        private DataGridView dgv;
        private TextBox txtNama, txtPerusahaan, txtDeskripsi;
        private NumericUpDown nudKru;
        private Button btnBatal, btnSimpan;
        private Maskapai dataEdit = null;

        public FormMasterMaskapai()
        {
            InisialisasiKomponen();
            MuatData();
        }

        private void InisialisasiKomponen()
        {
            var lblJudul = new Label
            {
                Text = "Master Maskapai",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 40
            };

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
            dgv.Columns.Add("Perusahaan", "Perusahaan");
            dgv.Columns.Add("JumlahKru", "Jumlah Kru");
            dgv.Columns.Add("Deskripsi", "Deskripsi");
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

            var panelInput = new Panel { Dock = DockStyle.Fill };

            int w = 200;
            TextBox BarisTex(string label, int left, int top)
            {
                panelInput.Controls.Add(new Label { Text = label, Left = left, Top = top, AutoSize = true });
                var t = new TextBox { Left = left, Top = top + 22, Width = w };
                panelInput.Controls.Add(t);
                return t;
            }

            txtNama = BarisTex("Nama", 10, 10);
            txtPerusahaan = BarisTex("Perusahaan", 230, 10);

            panelInput.Controls.Add(new Label { Text = "Jumlah Kru", Left = 450, Top = 10, AutoSize = true });
            nudKru = new NumericUpDown { Left = 450, Top = 32, Width = w, Minimum = 0, Maximum = 10000, Value = 1 };
            panelInput.Controls.Add(nudKru);

            // Deskripsi memakai textbox besar (multiline)
            panelInput.Controls.Add(new Label { Text = "Deskripsi", Left = 10, Top = 65, AutoSize = true });
            txtDeskripsi = new TextBox
            {
                Left = 10, Top = 87, Width = 640, Height = 90,
                Multiline = true, ScrollBars = ScrollBars.Vertical
            };
            panelInput.Controls.Add(txtDeskripsi);

            btnBatal = new Button { Text = "Batal", Left = 10, Top = 190, Width = 120, Height = 30 };
            btnBatal.Click += (s, e) => BersihkanForm();
            btnSimpan = new Button
            {
                Text = "Simpan", Left = 140, Top = 190, Width = 120, Height = 30,
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

        private void MuatData()
        {
            dgv.Rows.Clear();
            foreach (var m in DataStore.MaskapaiList.OrderBy(m => m.Nama))
                dgv.Rows.Add(m.Nama, m.Perusahaan, m.JumlahKru, m.Deskripsi, "Ubah", "Hapus");
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var m = DataStore.MaskapaiList.OrderBy(x => x.Nama).ElementAt(e.RowIndex);

            if (dgv.Columns[e.ColumnIndex].Name == "btnUbah")
            {
                dataEdit = m;
                txtNama.Text = m.Nama;
                txtPerusahaan.Text = m.Perusahaan;
                nudKru.Value = m.JumlahKru;
                txtDeskripsi.Text = m.Deskripsi;
            }
            else if (dgv.Columns[e.ColumnIndex].Name == "btnHapus")
            {
                var hasil = MessageBox.Show($"Hapus maskapai '{m.Nama}'?", "Konfirmasi",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (hasil == DialogResult.Yes)
                {
                    DataStore.MaskapaiList.Remove(m);
                    MuatData();
                    BersihkanForm();
                }
            }
        }

        private void BtnSimpan_Click(object sender, EventArgs e)
        {
            string nama = txtNama.Text.Trim();
            string perusahaan = txtPerusahaan.Text.Trim();
            int kru = (int)nudKru.Value;
            string deskripsi = txtDeskripsi.Text.Trim();

            if (nama == "" || perusahaan == "")
            {
                MessageBox.Show("Nama dan Perusahaan wajib diisi!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi: jumlah kru minimal 1
            if (kru < 1)
            {
                MessageBox.Show("Jumlah Kru minimal 1!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dataEdit == null)
            {
                DataStore.MaskapaiList.Add(new Maskapai
                { Nama = nama, Perusahaan = perusahaan, JumlahKru = kru, Deskripsi = deskripsi });
            }
            else
            {
                dataEdit.Nama = nama;
                dataEdit.Perusahaan = perusahaan;
                dataEdit.JumlahKru = kru;
                dataEdit.Deskripsi = deskripsi;
            }

            MuatData();
            BersihkanForm();
        }

        private void BersihkanForm()
        {
            txtNama.Clear(); txtPerusahaan.Clear(); txtDeskripsi.Clear();
            nudKru.Value = 1;
            dataEdit = null;
        }
    }
}
