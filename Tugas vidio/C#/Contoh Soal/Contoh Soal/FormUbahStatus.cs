using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Contoh_Soal
{
    /// <summary>
    /// Form Ubah Status Penerbangan.
    /// Menampilkan data jadwal + Status Terakhir + Terakhir Diubah.
    /// </summary>
    public class FormUbahStatus : Form
    {
        private DataGridView dgv;
        private Panel panelEdit;
        private ComboBox cmbStatus;
        private Label lblEstimasi;
        private TextBox txtEstimasi;
        private Button btnBatal, btnSimpan;
        private JadwalPenerbangan dataEdit = null;

        public FormUbahStatus()
        {
            InisialisasiKomponen();
            MuatData();
        }

        private void InisialisasiKomponen()
        {
            var lblJudul = new Label
            {
                Text = "Ubah Status Penerbangan",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 40
            };

            dgv = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 300,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            dgv.Columns.Add("Kode", "Kode Penerbangan");
            dgv.Columns.Add("Maskapai", "Maskapai");
            dgv.Columns.Add("Asal", "Bandara Keberangkatan");
            dgv.Columns.Add("Tujuan", "Bandara Tujuan");
            dgv.Columns.Add("Tanggal", "Tanggal Keberangkatan");
            dgv.Columns.Add("Waktu", "Waktu Keberangkatan");
            dgv.Columns.Add("Durasi", "Durasi Penerbangan");
            dgv.Columns.Add("Status", "Status Terakhir");
            dgv.Columns.Add("TerakhirDiubah", "Terakhir Diubah");
            dgv.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnUbah", HeaderText = "Aksi", Text = "Ubah",
                UseColumnTextForButtonValue = true, Width = 55
            });
            dgv.CellClick += Dgv_CellClick;

            // --- Panel input status (tersembunyi sampai tombol Ubah ditekan) ---
            panelEdit = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(240, 245, 255), Visible = false };

            panelEdit.Controls.Add(new Label { Text = "Status:", Left = 15, Top = 15, AutoSize = true });
            cmbStatus = new ComboBox
            {
                Left = 70, Top = 12, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(new object[] { "Sesuai Jadwal", "Delay", "Dibatalkan", "Tiba" });
            cmbStatus.SelectedIndexChanged += (s, e) =>
            {
                // Jika Delay, tampilkan input estimasi durasi delay
                bool delay = cmbStatus.Text == "Delay";
                lblEstimasi.Visible = delay;
                txtEstimasi.Visible = delay;
            };
            panelEdit.Controls.Add(cmbStatus);

            lblEstimasi = new Label { Text = "Estimasi Delay (Xjam Ymenit):", Left = 15, Top = 50, AutoSize = true, Visible = false };
            txtEstimasi = new TextBox { Left = 260, Top = 47, Width = 200, Visible = false };
            panelEdit.Controls.Add(lblEstimasi);
            panelEdit.Controls.Add(txtEstimasi);

            btnBatal = new Button { Text = "Batal", Left = 15, Top = 85, Width = 120, Height = 30 };
            btnBatal.Click += (s, e) => { panelEdit.Visible = false; dataEdit = null; };
            btnSimpan = new Button
            {
                Text = "Simpan", Left = 145, Top = 85, Width = 120, Height = 30,
                BackColor = Color.FromArgb(0, 102, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat
            };
            btnSimpan.FlatAppearance.BorderSize = 0;
            btnSimpan.Click += BtnSimpan_Click;
            panelEdit.Controls.Add(btnBatal);
            panelEdit.Controls.Add(btnSimpan);

            Controls.Add(panelEdit);
            Controls.Add(dgv);
            Controls.Add(lblJudul);
        }

        private void MuatData()
        {
            dgv.Rows.Clear();
            var data = DataStore.JadwalList
                .OrderByDescending(j => j.TanggalKeberangkatan.Date)
                .ThenByDescending(j => TimeSpan.TryParse(j.WaktuKeberangkatan, out var t) ? t : TimeSpan.Zero);

            foreach (var j in data)
            {
                // Status: jika Delay tampilkan perkiraan durasi, jika null tampilkan "Sesuai Jadwal"
                string status;
                if (string.IsNullOrEmpty(j.StatusTerakhir))
                    status = "Sesuai Jadwal";
                else if (j.StatusTerakhir == "Delay" && !string.IsNullOrEmpty(j.EstimasiDelay))
                    status = $"Delay (selama ±{j.EstimasiDelay})";
                else
                    status = j.StatusTerakhir;

                dgv.Rows.Add(j.KodePenerbangan, j.Maskapai, j.BandaraKeberangkatan, j.BandaraTujuan,
                    j.TanggalKeberangkatan.ToString("dd-MM-yyyy"), j.WaktuKeberangkatan,
                    j.DurasiPenerbangan, status,
                    j.TerakhirDiubah.HasValue ? j.TerakhirDiubah.Value.ToString("dd-MM-yyyy HH:mm:ss") : "-",
                    "Ubah");
            }
        }

        private void Dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgv.Columns[e.ColumnIndex].Name != "btnUbah") return;

            var j = DataStore.JadwalList
                .OrderByDescending(x => x.TanggalKeberangkatan.Date)
                .ThenByDescending(x => TimeSpan.TryParse(x.WaktuKeberangkatan, out var t) ? t : TimeSpan.Zero)
                .ElementAt(e.RowIndex);

            // Tampilkan panel input status
            dataEdit = j;
            // Jika status belum pernah diubah, tampilkan "Sesuai Jadwal"
            cmbStatus.Text = string.IsNullOrEmpty(j.StatusTerakhir) ? "Sesuai Jadwal" : j.StatusTerakhir;
            if (cmbStatus.Text == "" || cmbStatus.Items.Count == 0) cmbStatus.SelectedIndex = 0;
            txtEstimasi.Text = j.EstimasiDelay;
            bool delay = cmbStatus.Text == "Delay";
            lblEstimasi.Visible = delay;
            txtEstimasi.Visible = delay;
            panelEdit.Visible = true;
        }

        private void BtnSimpan_Click(object sender, EventArgs e)
        {
            if (dataEdit == null) return;

            string status = cmbStatus.Text;
            string estimasi = txtEstimasi.Text.Trim();

            // Jika status Delay, validasi format estimasi "XX jam YY menit" (+ menit 0-59)
            var matchDelay = Regex.Match(estimasi, @"^(\d+)jam\s*(\d+)menit$", RegexOptions.IgnoreCase);
            if (status == "Delay" && (!matchDelay.Success || int.Parse(matchDelay.Groups[2].Value) > 59))
            {
                MessageBox.Show("Estimasi durasi delay harus berformat \"Xjam Ymenit\" (cth: 1jam 15menit) dengan menit 0-59!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Simpan status & waktu perubahan
            dataEdit.StatusTerakhir = status;
            dataEdit.EstimasiDelay = status == "Delay" ? estimasi : null;
            dataEdit.TerakhirDiubah = DateTime.Now;

            MuatData();
            panelEdit.Visible = false;
            dataEdit = null;

            MessageBox.Show("Status penerbangan berhasil diubah.", "Sukses",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
