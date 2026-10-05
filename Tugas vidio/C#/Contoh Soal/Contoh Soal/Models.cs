using System;

namespace Contoh_Soal
{
    // ====== MODEL DATA ======
    // Semua data disimpan di memori (in-memory) melalui kelas DataStore.

    public class Akun
    {
        public string Username;
        public string Nama;
        public DateTime TanggalLahir;
        public string Telepon;
        public string Password;
        public bool MerupakanAdmin; // true = Admin, false = Customer
    }

    public class Bandara
    {
        public string Nama;
        public string KodeIATA;
        public string Kota;
        public string Negara;
        public int JumlahTerminal;
        public string Alamat;
    }

    public class Maskapai
    {
        public string Nama;
        public string Perusahaan;
        public int JumlahKru;
        public string Deskripsi;
    }

    public class JadwalPenerbangan
    {
        public string KodePenerbangan;
        public string BandaraKeberangkatan;
        public string BandaraTujuan;
        public string Maskapai;
        public DateTime TanggalKeberangkatan;
        public string WaktuKeberangkatan; // format HH:mm
        public string DurasiPenerbangan;  // format "XX jam YY menit"
        public decimal HargaPerTiket;
        public string StatusTerakhir;     // diisi lewat Form Ubah Status
        public string EstimasiDelay;      // diisi jika status Delay
        public DateTime? TerakhirDiubah;  // diisi saat status diubah
    }

    public class KodePromo
    {
        public string Kode;
        public int PersentaseDiskon;
        public decimal MaksimumDiskon;
        public DateTime BerlakuSampai;
        public string Deskripsi;
    }
}
