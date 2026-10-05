using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Contoh_Soal
{
    /// <summary>
    /// Form Master Jadwal Penerbangan: CRUD jadwal.
    /// Diurutkan menurun berdasarkan tanggal & waktu keberangkatan.
    /// </summary>
    public class FormMasterJadwal : Form
    {
        private DataGridView dgv;
        private TextBox txtKode, txtWaktu, txtDurasi;
        private ComboBox cmbAsal, cmbTujuan, cmbMaskapai;
        private DateTimePicker dtpTanggal;
        private NumericUpDown nudHarga;
        private Button btnBatal, btnSimpan;
        private JadwalPenerbangan dataEdit = null;

        public FormMasterJadwal()
        {
            InisialisasiKomponen();
            IsiCombo();
            MuatData();
        }

        private void InisialisasiKomponen()
        {
            var lblJudul = new Label
            {
                Text = "Master Jadwal Penerbangan",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 40
            };

            dgv = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 230,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            dgv.Columns.Add("KodePenerbangan", "Kode Penerbangan");
            dgv.Columns.Add("BandaraKeberangkatan", "Bandara Keberangkatan");
            dgv.Columns.Add("BandaraTujuan", "Bandara Tujuan");
            dgv.Columns.Add("Maskapai", "Maskapai");
            dgv.Columns.Add("Tanggal", "Tanggal Keberangkatan");
            dgv.Columns.Add("Waktu", "Waktu Keberangkatan");
            dgv.Columns.Add("Durasi", "Durasi Penerbangan");
            dgv.Columns.Add("Harga", "Harga Per Tiket");
            dgv.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnUbah", HeaderText = "Aksi", Text = "Ubah",
                UseColumnTextForButtonValue = true, Width = 55
            });
            dgv.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnHapus", HeaderText = "Aksi", Text = "Hapus",
                UseColumnTextForButtonValue = true, Width = 55
            });
            dgv.CellClick += Dgv_CellClick;

            var panelInput = new Panel { Dock = DockStyle.Fill };

            int w = 200, y = 10;
            // helper membuat label + textbox
            TextBox TB(string label, int left, int topPos)
            {
                panelInput.Controls.Add(new Label { Text = label, Left = left, Top = topPos, AutoSize = true });
                var t = new TextBox { Left = left, Top = topPos + 22, Width = w };
                panelInput.Controls.Add(t);
                return t;
            }
            // helper membuat label + combobox
            ComboBox CB(string label, int left, int topPos)
            {
                panelInput.Controls.Add(new Label { Text = label, Left = left, Top = topPos, AutoSize = true });
                var c = new ComboBox { Left = left, Top = topPos + 22, Width = w, DropDownStyle = ComboBoxStyle.DropDownList };
                panelInput.Controls.Add(c);
                return c;
            }

            txtKode = TB("Kode Penerbangan (XX-1234)", 10, y);
            cmbAsal = CB("Bandara Keberangkatan", 230, y);
            cmbTujuan = CB("Bandara Tujuan", 450, y);
            y += 60;
            cmbMaskapai = CB("Maskapai", 10, y);

            panelInput.Controls.Add(new Label { Text = "Tanggal Keberangkatan", Left = 230, Top = y, AutoSize = true });
            dtpTanggal = new DateTimePicker
            {
                Left = 230, Top = y + 22, Width = w,
                Format = DateTimePickerFormat.Custom, CustomFormat = "dd-MM-yyyy"
            };
            panelInput.Controls.Add(dtpTanggal);

            txtWaktu = TB("Waktu Keberangkatan (HH:MM)", 450, y);
            y += 60;
            txtDurasi = TB("Durasi (contoh: 2jam 30menit)", 10, y);

            panelInput.Controls.Add(new Label { Text = "Harga Per Tiket", Left = 230, Top = y, AutoSize = true });
            nudHarga = new NumericUpDown { Left = 230, Top = y + 22, Width = w, Minimum = 0, Maximum = 100000000, Value = 0 };
            panelInput.Controls.Add(nudHarga);
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

        // Isi combobox dari data master
        private void IsiCombo()
        {
            cmbAsal.DataSource = DataStore.BandaraList.Select(b => b.Nama).OrderBy(x => x).ToList();
            cmbTujuan.DataSource = DataStore.BandaraList.Select(b => b.Nama).OrderBy(x => x).ToList();
            cmbMaskapai.DataSource = DataStore.MaskapaiList.Select(m => m.Nama).OrderBy(x => x).ToList();
        }

        private void MuatData()
        {
            dgv.Rows.Clear();
            // Urut menurun berdasarkan tanggal lalu waktu keberangkatan
            var data = DataStore.JadwalList
                .OrderByDescending(j => j.TanggalKeberangkatan.Date)
                .ThenByDescending(j => TimeSpan.TryParse(j.WaktuKeberangkatan, out var t) ? t : TimeSpan.Zero);

            foreach (var j in data)
            {
                dgv.Rows.Add(j.KodePenerbangan, j.BandaraKeberangkatan, j.BandaraTujuan, j.Maskapai,
                    j.TanggalKeberangkatan.ToString("dd-MM-yyyy"), j.WaktuKeberangkatan,
                    j.DurasiPenerbangan, j.HargaPerTiket.ToString("N0"), "Ubah", "Hapus");
            }
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var j = DataStore.JadwalList
                .OrderByDescending(x => x.TanggalKeberangkatan.Date)
                .ThenByDescending(x => TimeSpan.TryParse(x.WaktuKeberangkatan, out var t) ? t : TimeSpan.Zero)
                .ElementAt(e.RowIndex);

            if (dgv.Columns[e.ColumnIndex].Name == "btnUbah")
            {
                dataEdit = j;
                txtKode.Text = j.KodePenerbangan;
                cmbAsal.SelectedItem = j.BandaraKeberangkatan;
                cmbTujuan.SelectedItem = j.BandaraTujuan;
                cmbMaskapai.SelectedItem = j.Maskapai;
                dtpTanggal.Value = j.TanggalKeberangkatan;
                txtWaktu.Text = j.WaktuKeberangkatan;
                txtDurasi.Text = j.DurasiPenerbangan;
                nudHarga.Value = j.HargaPerTiket;
            }
            else if (dgv.Columns[e.ColumnIndex].Name == "btnHapus")
            {
                var hasil = MessageBox.Show($"Hapus jadwal '{j.KodePenerbangan}'?", "Konfirmasi",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (hasil == DialogResult.Yes)
                {
                    DataStore.JadwalList.Remove(j);
                    MuatData();
                    BersihkanForm();
                }
            }
        }

        private void BtnSimpan_Click(object sender, EventArgs e)
        {
            string kode = txtKode.Text.Trim().ToUpper();
            string asal = cmbAsal.SelectedItem?.ToString();
            string tujuan = cmbTujuan.SelectedItem?.ToString();
            string maskapai = cmbMaskapai.SelectedItem?.ToString();
            string waktu = txtWaktu.Text.Trim();
            string durasi = txtDurasi.Text.Trim();
            decimal harga = nudHarga.Value;

            // Validasi: format kode penerbangan "XX-1234"
            if (!Regex.IsMatch(kode, @"^[A-Z]{2}-\d{4}$"))
            {
                MessageBox.Show("Format Kode Penerbangan harus dua huruf - empat angka (cth: XX-1234)!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi: bandara keberangkatan dan tujuan tidak boleh sama
            if (asal != null && tujuan != null && asal == tujuan)
            {
                MessageBox.Show("Bandara keberangkatan dan tujuan tidak boleh sama!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi: waktu format 24 jam HH:MM
            if (!Regex.IsMatch(waktu, @"^([01]\d|2[0-3]):[0-5]\d$"))
            {
                MessageBox.Show("Format Waktu harus HH:MM (00:00 - 23:59)!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi: format durasi "XX jam YY menit" + kevalidan (menit 0-59)
            var matchDurasi = Regex.Match(durasi, @"^(\d+)jam\s*(\d+)menit$", RegexOptions.IgnoreCase);
            if (!matchDurasi.Success || int.Parse(matchDurasi.Groups[2].Value) > 59)
            {
                MessageBox.Show("Format Durasi harus \"Xjam Ymenit\" (cth: 2jam 30menit) dengan menit 0-59!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validasi: harga tiket minimal 1
            if (harga < 1)
            {
                MessageBox.Show("Harga tiket minimal 1!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (asal == null || tujuan == null || maskapai == null)
            {
                MessageBox.Show("Pilih bandara keberangkatan, tujuan, dan maskapai terlebih dahulu!",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dataEdit == null)
            {
                DataStore.JadwalList.Add(new JadwalPenerbangan
                {
                    KodePenerbangan = kode, BandaraKeberangkatan = asal, BandaraTujuan = tujuan,
                    Maskapai = maskapai, TanggalKeberangkatan = dtpTanggal.Value.Date,
                    WaktuKeberangkatan = waktu, DurasiPenerbangan = durasi, HargaPerTiket = harga,
                    StatusTerakhir = "Sesuai Jadwal"
                });
            }
            else
            {
                dataEdit.KodePenerbangan = kode;
                dataEdit.BandaraKeberangkatan = asal;
                dataEdit.BandaraTujuan = tujuan;
                dataEdit.Maskapai = maskapai;
                dataEdit.TanggalKeberangkatan = dtpTanggal.Value.Date;
                dataEdit.WaktuKeberangkatan = waktu;
                dataEdit.DurasiPenerbangan = durasi;
                dataEdit.HargaPerTiket = harga;
            }

            MuatData();
            BersihkanForm();
        }

        private void BersihkanForm()
        {
            txtKode.Clear(); txtWaktu.Clear(); txtDurasi.Clear();
            nudHarga.Value = 0;
            if (cmbAsal.Items.Count > 0) cmbAsal.SelectedIndex = 0;
            if (cmbTujuan.Items.Count > 0) cmbTujuan.SelectedIndex = 0;
            if (cmbMaskapai.Items.Count > 0) cmbMaskapai.SelectedIndex = 0;
            dtpTanggal.Value = DateTime.Today;
            dataEdit = null;
        }
    }
}
