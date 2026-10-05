using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Contoh_Soal
{
    /// <summary>
    /// Form Master Kode Promo: CRUD kode promo.
    /// </summary>
    public class FormMasterPromo : Form
    {
        private DataGridView dgv;
        private TextBox txtKode, txtDeskripsi;
        private NumericUpDown nudPersen, nudMaks;
        private DateTimePicker dtpBerlaku;
        private Button btnBatal, btnSimpan;
        private KodePromo dataEdit = null;

        public FormMasterPromo()
        {
            InisialisasiKomponen();
            MuatData();
        }

        private void InisialisasiKomponen()
        {
            var lblJudul = new Label
            {
                Text = "Master Kode Promo",
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
            dgv.Columns.Add("Kode", "Kode Promo");
            dgv.Columns.Add("Persen", "Persentase Diskon");
            dgv.Columns.Add("Maks", "Maksimum Diskon");
            dgv.Columns.Add("Berlaku", "Berlaku Sampai");
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
            int w = 200, y = 10;

            panelInput.Controls.Add(new Label { Text = "Kode Promo (huruf kapital)", Left = 10, Top = y, AutoSize = true });
            txtKode = new TextBox { Left = 10, Top = y + 22, Width = w };
            panelInput.Controls.Add(txtKode);

            panelInput.Controls.Add(new Label { Text = "Persentase Diskon (%)", Left = 230, Top = y, AutoSize = true });
            nudPersen = new NumericUpDown { Left = 230, Top = y + 22, Width = w, Minimum = 0, Maximum = 100, Value = 0 };
            panelInput.Controls.Add(nudPersen);

            panelInput.Controls.Add(new Label { Text = "Maksimum Diskon", Left = 450, Top = y, AutoSize = true });
            nudMaks = new NumericUpDown { Left = 450, Top = y + 22, Width = w, Minimum = 0, Maximum = 100000000, Value = 0 };
            panelInput.Controls.Add(nudMaks);
            y += 60;

            panelInput.Controls.Add(new Label { Text = "Berlaku Sampai", Left = 10, Top = y, AutoSize = true });
            dtpBerlaku = new DateTimePicker
            {
                Left = 10, Top = y + 22, Width = w,
                Format = DateTimePickerFormat.Custom, CustomFormat = "dd-MM-yyyy"
            };
            panelInput.Controls.Add(dtpBerlaku);

            panelInput.Controls.Add(new Label { Text = "Deskripsi", Left = 230, Top = y, AutoSize = true });
            txtDeskripsi = new TextBox { Left = 230, Top = y + 22, Width = 420 };
            panelInput.Controls.Add(txtDeskripsi);
            y += 60;

            btnBatal = new Button { Text = "Batal", Left = 10, Top = y, Width = 120, Height = 30 };
            btnBatal.Click += (s, e) => BersihkanForm();
            btnSimpan = new Button
            {
                Text = "Simpan", Left = 140, Top = y, Width = 120, Height = 30,
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
            foreach (var p in DataStore.PromoList)
                dgv.Rows.Add(p.Kode, p.PersentaseDiskon + "%", p.MaksimumDiskon.ToString("N0"),
                    p.BerlakuSampai.ToString("dd-MM-yyyy"), p.Deskripsi, "Ubah", "Hapus");
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var p = DataStore.PromoList[e.RowIndex];

            if (dgv.Columns[e.ColumnIndex].Name == "btnUbah")
            {
                dataEdit = p;
                txtKode.Text = p.Kode;
                nudPersen.Value = p.PersentaseDiskon;
                nudMaks.Value = p.MaksimumDiskon;
                dtpBerlaku.Value = p.BerlakuSampai;
                txtDeskripsi.Text = p.Deskripsi;
            }
            else if (dgv.Columns[e.ColumnIndex].Name == "btnHapus")
            {
                var hasil = MessageBox.Show($"Hapus kode promo '{p.Kode}'?", "Konfirmasi",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (hasil == DialogResult.Yes)
                {
                    DataStore.PromoList.Remove(p);
                    MuatData();
                    BersihkanForm();
                }
            }
        }

        private void BtnSimpan_Click(object sender, EventArgs e)
        {
            string kode = txtKode.Text.Trim();
            int persen = (int)nudPersen.Value;
            decimal maks = nudMaks.Value;
            string deskripsi = txtDeskripsi.Text.Trim();

            // Validasi: kode promo huruf kapital dan unik
            if (!Regex.IsMatch(kode, @"^[A-Z0-9]+$"))
            {
                MessageBox.Show("Kode promo harus huruf kapital (tanpa spasi/simbol)!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (DataStore.PromoList.Any(p => p != dataEdit &&
                p.Kode.Equals(kode, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Kode promo sudah digunakan!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi: persentase diskon minimal 1
            if (persen < 1)
            {
                MessageBox.Show("Persentase diskon minimal 1!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi: maksimum diskon minimal 1
            if (maks < 1)
            {
                MessageBox.Show("Maksimum diskon minimal 1!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dataEdit == null)
            {
                DataStore.PromoList.Add(new KodePromo
                {
                    Kode = kode, PersentaseDiskon = persen, MaksimumDiskon = maks,
                    BerlakuSampai = dtpBerlaku.Value.Date, Deskripsi = deskripsi
                });
            }
            else
            {
                dataEdit.Kode = kode;
                dataEdit.PersentaseDiskon = persen;
                dataEdit.MaksimumDiskon = maks;
                dataEdit.BerlakuSampai = dtpBerlaku.Value.Date;
                dataEdit.Deskripsi = deskripsi;
            }

            MuatData();
            BersihkanForm();
        }

        private void BersihkanForm()
        {
            txtKode.Clear(); txtDeskripsi.Clear();
            nudPersen.Value = 0; nudMaks.Value = 0;
            dtpBerlaku.Value = DateTime.Today;
            dataEdit = null;
        }
    }
}
