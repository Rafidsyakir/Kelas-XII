using System;
using System.Globalization;
using System.Windows.Forms;

namespace Calculator_sederhana
{
    public partial class Form1 : Form
    {
        // Variabel untuk menyimpan angka pertama, operator yang dipilih,
        // dan status apakah input berikutnya memulai angka baru.
        double firstNumber = 0;
        string operation = "";
        bool newInput = true;

        public Form1()
        {
            InitializeComponent();
        }

        // Event handler satu untuk semua tombol angka (0-9).
        // Teks tombol yang diklik langsung ditambahkan ke layar.
        private void btnNumber_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (newInput || txtDisplay.Text == "0")
            {
                // Mulai angka baru atau ganti angka 0 di layar
                txtDisplay.Text = btn.Text;
                newInput = false;
            }
            else
            {
                // Menambahkan digit ke angka yang sedang diketik
                txtDisplay.Text += btn.Text;
            }
        }

        // Event handler tombol titik desimal (.)
        private void btnDecimal_Click(object sender, EventArgs e)
        {
            if (newInput)
            {
                txtDisplay.Text = "0.";
                newInput = false;
            }
            else if (!txtDisplay.Text.Contains("."))
            {
                // Hanya boleh ada satu titik desimal
                txtDisplay.Text += ".";
            }
        }

        // Event handler satu untuk semua tombol operator (+, -, *, /).
        // Angka di layar disimpan sebagai angka pertama, lalu tunggu input kedua.
        private void btnOperator_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            firstNumber = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
            operation = btn.Text;
            newInput = true;
        }

        // Event handler tombol sama dengan (=):
        // menjalankan operasi sesuai operator yang dipilih (switch-case).
        private void btnEqual_Click(object sender, EventArgs e)
        {
            if (operation == "")
            {
                return; // Belum ada operator dipilih
            }

            double secondNumber = double.Parse(txtDisplay.Text, CultureInfo.InvariantCulture);
            double result = 0;

            switch (operation)
            {
                case "+":
                    result = firstNumber + secondNumber;
                    break;
                case "-":
                    result = firstNumber - secondNumber;
                    break;
                case "*":
                    result = firstNumber * secondNumber;
                    break;
                case "/":
                    if (secondNumber == 0)
                    {
                        MessageBox.Show("Tidak bisa membagi dengan nol!", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    result = firstNumber / secondNumber;
                    break;
            }

            txtDisplay.Text = result.ToString(CultureInfo.InvariantCulture);
            operation = "";
            newInput = true;
        }

        // Tombol C: menghapus semua, hasil perhitungan dan angka pertama direset.
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtDisplay.Text = "0";
            firstNumber = 0;
            operation = "";
            newInput = true;
        }

        // Tombol CE: hanya menghapus angka yang sedang diketik (current entry).
        private void btnClearEntry_Click(object sender, EventArgs e)
        {
            txtDisplay.Text = "0";
            newInput = true;
        }
    }
}
