using System;                          // [MATERI 1] Import namespace bawaan .NET untuk Console
using System.Collections.Generic;      // [MATERI 5] Import namespace untuk menggunakan List<T>

namespace KasirMini                    // [MATERI 1] Namespace: wadah/pengelompokan kode program
{
    class Program                      // [MATERI 1] Class: blueprint/cetak biru objek, semua kode ada di dalamnya
    {
        // [MATERI 6] Method dengan parameter dan nilai balik (return)
        static int HitungTotal(int harga, int jumlah)
        {
            return harga * jumlah;     // [MATERI 2] Operasi sederhana: perkalian
        }

        // [MATERI 6] Method tanpa parameter dan tanpa nilai balik
        static void TampilMenu()
        {
            Console.WriteLine("\n=== KASIR MINI ===");
            Console.WriteLine("1. Lihat Barang");
            Console.WriteLine("2. Beli Barang");
            Console.WriteLine("3. Lihat Keranjang");
            Console.WriteLine("4. Bayar");
            Console.WriteLine("0. Keluar");
        }

        // [MATERI 1] Main Method: titik awal program dieksekusi
        static void Main(string[] args)
        {
            // [MATERI 2] Deklarasi variabel dan tipe data
            string namaToko = "Toko Serba Ada";   // tipe string
            int pilihan, jumlahBeli;              // tipe int
            double diskon = 0.1;                  // tipe double (0.1 = 10%)
            bool lanjut = true;                   // tipe bool

            // [MATERI 5] Array: kumpulan data dengan ukuran tetap
            string[] daftarBarang = { "Pensil", "Buku", "Penghapus" };
            int[] daftarHarga = { 2000, 5000, 1000 };

            // [MATERI 5] List: kumpulan data yang ukurannya bisa berubah
            List<string> keranjang = new List<string>();
            int totalBelanja = 0;

            Console.WriteLine("Selamat datang di " + namaToko);   // [MATERI 2] operasi string +

            // [MATERI 4] Perulangan While: ulang selama kondisi true
            while (lanjut)
            {
                TampilMenu();                                     // [MATERI 6] memanggil method
                Console.Write("Pilih menu: ");
                pilihan = Convert.ToInt32(Console.ReadLine());     // input diubah ke int

                // [MATERI 3] Switch: memilih blok kode berdasarkan nilai variabel
                switch (pilihan)
                {
                    case 1:
                        // [MATERI 4] Perulangan For: perulangan dengan jumlah yang diketahui
                        for (int i = 0; i < daftarBarang.Length; i++)
                        {
                            Console.WriteLine((i + 1) + ". " + daftarBarang[i] + " = Rp" + daftarHarga[i]);
                        }
                        break;

                    case 2:
                        Console.Write("Nomor barang: ");
                        int nomor = Convert.ToInt32(Console.ReadLine()) - 1;

                        // [MATERI 3] IF-ELSE: validasi nomor barang
                        if (nomor >= 0 && nomor < daftarBarang.Length)
                        {
                            Console.Write("Jumlah beli: ");
                            jumlahBeli = Convert.ToInt32(Console.ReadLine());

                            int total = HitungTotal(daftarHarga[nomor], jumlahBeli); // [MATERI 6]
                            keranjang.Add(daftarBarang[nomor] + " x" + jumlahBeli);  // [MATERI 5] List.Add
                            totalBelanja += total;                                     // [MATERI 2] operasi +=
                            Console.WriteLine("Ditambahkan! Subtotal: Rp" + total);
                        }
                        else
                        {
                            Console.WriteLine("Barang tidak ditemukan!");
                        }
                        break;

                    case 3:
                        // [MATERI 4] Perulangan Foreach: untuk setiap item dalam kumpulan data
                        Console.WriteLine("Isi keranjang:");
                        foreach (string item in keranjang)
                        {
                            Console.WriteLine("- " + item);
                        }
                        Console.WriteLine("Total sementara: Rp" + totalBelanja);
                        break;

                    case 4:
                        // [MATERI 3] IF-ELSE IF-ELSE: cek kondisi berjenjang
                        if (totalBelanja >= 50000)
                        {
                            double bayar = totalBelanja - (totalBelanja * diskon);
                            Console.WriteLine("Diskon 10%! Total bayar: Rp" + bayar);
                        }
                        else if (totalBelanja >= 20000)
                        {
                            Console.WriteLine("Total bayar: Rp" + totalBelanja + " (belum dapat diskon)");
                        }
                        else
                        {
                            Console.WriteLine("Total bayar: Rp" + totalBelanja);
                        }
                        lanjut = false; // menghentikan while
                        break;

                    case 0:
                        lanjut = false; // menghentikan while
                        break;

                    default:
                        Console.WriteLine("Pilihan tidak valid!");
                        break;
                }
            }

            Console.WriteLine("Terima kasih sudah berbelanja!");
        }
    }
}
