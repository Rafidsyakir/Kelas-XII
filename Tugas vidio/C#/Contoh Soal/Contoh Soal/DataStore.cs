using System;
using System.Collections.Generic;

namespace Contoh_Soal
{
    /// <summary>
    /// Penyimpanan data bersama (in-memory) untuk seluruh form.
    public static class DataStore
    {
        // Daftar akun terdaftar
        public static List<Akun> AkunList = new List<Akun>();

        // Data master
        public static List<Bandara> BandaraList = new List<Bandara>();
        public static List<Maskapai> MaskapaiList = new List<Maskapai>();
        public static List<JadwalPenerbangan> JadwalList = new List<JadwalPenerbangan>();
        public static List<KodePromo> PromoList = new List<KodePromo>();

        // Daftar nama negara (dipakai pada combobox Negara di Master Bandara)
        public static List<string> Countries = new List<string>
        {
            "Afrika Selatan", "Albania", "Aljazair", "Amerika Serikat", "Arab Saudi", "Argentina",
            "Australia", "Austria", "Azerbaijan", "Bahrain", "Bangladesh", "Belanda", "Belarus",
            "Belgia", "Bolivia", "Brazil", "Brunei", "Bulgaria", "Kamboja", "Chili", "Cina",
            "Denmark", "Ethiopia", "Filipina", "Finlandia",
            "Inggris", "Irak", "Iran", "Irlandia", "Islandia", "Israel", "Italia", "Jepang",
            "Jerman", "Kanada", "Kazakhstan", "Kenya", "Kolombia", "Korea Selatan",
            "Korea Utara", "Kroasia", "Kuwait", "Laos", "Lebanon", "Malaysia", "Maladewa",
            "Maroko", "Mesir", "Meksiko", "Myanmar", "Nepal", "Nigeria", "Norwegia", "Pakistan",
            "Palestina", "Panama", "Peru", "Polandia", "Portugal", "Qatar", "Rusia", "Selandia Baru",
            "Serbia", "Singapura", "Slovakia", "Slovenia", "Spanyol", "Sri Lanka", "Sudan",
            "Swedia", "Swiss", "Taiwan", "Thailand", "Timor Leste", "Turki",
            "Ukraina", "Uni Emirat Arab", "Uruguay", "Uzbekistan", "Venezuela", "Vietnam",
            "Yunani", "Indonesia"
        };

        static DataStore()
        {
            // Akun admin bawaan untuk simulasi login
            AkunList.Add(new Akun
            {
                Username = "admin",
                Nama = "Administrator",
                TanggalLahir = new DateTime(2000, 1, 1),
                Telepon = "081234567890",
                Password = "admin123",
                MerupakanAdmin = true
            });

            // Contoh data bandara (untuk pilihan dari/ke pada jadwal)
            BandaraList.Add(new Bandara { Nama = "Bandara Internasional Juanda", KodeIATA = "SUB", Kota = "Surabaya", Negara = "Indonesia", JumlahTerminal = 2, Alamat = "Jl. Ir. Soekarno, Sedati, Sidoarjo" });
            BandaraList.Add(new Bandara { Nama = "Bandara Internasional Soekarno-Hatta", KodeIATA = "CGK", Kota = "Tangerang", Negara = "Indonesia", JumlahTerminal = 3, Alamat = "Jl. Raya Bandara, Benda, Tangerang" });
            BandaraList.Add(new Bandara { Nama = "Bandara Internasional Ngurah Rai", KodeIATA = "DPS", Kota = "Denpasar", Negara = "Indonesia", JumlahTerminal = 2, Alamat = "Jl. Wirasaba, Tuban, Badung" });
            BandaraList.Add(new Bandara { Nama = "Changi Airport", KodeIATA = "SIN", Kota = "Singapura", Negara = "Singapura", JumlahTerminal = 4, Alamat = "Airport Blvd, Singapura" });

            // Contoh data maskapai
            MaskapaiList.Add(new Maskapai { Nama = "Garuda Indonesia", Perusahaan = "PT Garuda Indonesia Tbk", JumlahKru = 25, Deskripsi = "Maskapai nasional Indonesia, penerbangan penuh pelayanan." });
            MaskapaiList.Add(new Maskapai { Nama = "Lion Air", Perusahaan = "Lion Group", JumlahKru = 18, Deskripsi = "Maskapai bertarif rendah terbesar di Indonesia." });
            MaskapaiList.Add(new Maskapai { Nama = "Batik Air", Perusahaan = "Lion Group", JumlahKru = 15, Deskripsi = "Maskapai layanan penuh dengan harga terjangkau." });

            // Contoh data jadwal (durasi berformat "Xjam Ymenit")
            JadwalList.Add(new JadwalPenerbangan
            {
                KodePenerbangan = "GA-1234",
                BandaraKeberangkatan = "Bandara Internasional Juanda",
                BandaraTujuan = "Bandara Internasional Soekarno-Hatta",
                Maskapai = "Garuda Indonesia",
                TanggalKeberangkatan = DateTime.Today.AddDays(1),
                WaktuKeberangkatan = "08:30",
                DurasiPenerbangan = "1jam 30menit",
                HargaPerTiket = 500000,
                StatusTerakhir = "Sesuai Jadwal"
            });
        }
    }
}
