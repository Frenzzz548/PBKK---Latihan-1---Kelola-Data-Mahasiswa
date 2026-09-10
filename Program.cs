using System;
using System.Collections.Generic;

class Mahasiswa
{
    public string NRP { get; set; } = "";
    public string Nama { get; set; } = "";
    public string Jurusan { get; set; } = "";
}

class Program
{
    static readonly List<Mahasiswa> daftarMahasiswa = new();

    static void Main()
    {
        while (true)
        {
            TampilkanMenu();
            Console.Write("Pilihan: ");
            string pilihan = Console.ReadLine() ?? "";

            Console.Clear();

            switch (pilihan)
            {
                case "1":
                    TambahMahasiswa();
                    break;
                case "2":
                    TampilkanMahasiswa();
                    break;
                case "3":
                    CariMahasiswa();
                    break;
                case "4":
                    HapusMahasiswa();
                    break;
                case "5":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Terima kasih. Program selesai.");
                    Console.ResetColor();
                    return;
                default:
                    TampilkanPesan("Pilihan tidak tersedia.", ConsoleColor.Red);
                    break;
            }

            Console.WriteLine();
            Console.Write("Tekan ENTER untuk kembali ke menu...");
            Console.ReadLine();
            Console.Clear();
        }
    }

    static void TampilkanMenu()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("============================================================");
        Console.WriteLine("              SISTEM DATA MAHASISWA");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("          KELOLA DATA MAHASISWA DENGAN MUDAH");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("============================================================");
        Console.ResetColor();
        Console.WriteLine();

        TampilkanPilihan("1", "Tambah Mahasiswa", ConsoleColor.Green);
        TampilkanPilihan("2", "Tampilkan Mahasiswa", ConsoleColor.Blue);
        TampilkanPilihan("3", "Cari Mahasiswa", ConsoleColor.Yellow);
        TampilkanPilihan("4", "Hapus Mahasiswa", ConsoleColor.Red);
        TampilkanPilihan("5", "Keluar", ConsoleColor.Gray);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("============================================================");
        Console.ResetColor();
    }

    static void TampilkanPilihan(string nomor, string teks, ConsoleColor warna)
    {
        Console.ForegroundColor = warna;
        Console.Write($"  [{nomor}] ");
        Console.ResetColor();
        Console.WriteLine(teks);
    }

    static void TambahMahasiswa()
    {
        TampilkanJudul("TAMBAH MAHASISWA");

        Console.Write("NRP     : ");
        string nrp = Console.ReadLine()?.Trim() ?? "";

        if (daftarMahasiswa.Exists(mahasiswa => mahasiswa.Nrp.Equals(nrp, StringComparison.OrdinalIgnoreCase)))
        {
            TampilkanPesan("NRP sudah terdaftar.", ConsoleColor.Red);
            return;
        }

        Console.Write("Nama    : ");
        string nama = Console.ReadLine()?.Trim() ?? "";
        Console.Write("Jurusan : ");
        string jurusan = Console.ReadLine()?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(nrp) || string.IsNullOrWhiteSpace(nama) || string.IsNullOrWhiteSpace(jurusan))
        {
            TampilkanPesan("Semua data harus diisi.", ConsoleColor.Red);
            return;
        }

        daftarMahasiswa.Add(new Mahasiswa { Nrp = nrp, Nama = nama, Jurusan = jurusan });
        TampilkanPesan("Data mahasiswa berhasil ditambahkan.", ConsoleColor.Green);
    }

    static void TampilkanMahasiswa()
    {
        TampilkanJudul("DAFTAR MAHASISWA");

        if (daftarMahasiswa.Count == 0)
        {
            TampilkanPesan("Belum ada data mahasiswa.", ConsoleColor.Yellow);
            return;
        }

        Console.WriteLine("No  NRP              Nama                 Jurusan");
        Console.WriteLine("------------------------------------------------------------");

        for (int i = 0; i < daftarMahasiswa.Count; i++)
        {
            Mahasiswa mahasiswa = daftarMahasiswa[i];
            Console.WriteLine($"{i + 1,-3} {mahasiswa.Nrp,-16} {mahasiswa.Nama,-20} {mahasiswa.Jurusan}");
        }
    }

    static void CariMahasiswa()
    {
        TampilkanJudul("CARI MAHASISWA");
        Console.Write("Masukkan NRP atau nama: ");
        string kataKunci = Console.ReadLine()?.Trim() ?? "";

        List<Mahasiswa> hasil = daftarMahasiswa.FindAll(mahasiswa =>
            mahasiswa.Nrp.Contains(kataKunci, StringComparison.OrdinalIgnoreCase) ||
            mahasiswa.Nama.Contains(kataKunci, StringComparison.OrdinalIgnoreCase));

        if (hasil.Count == 0)
        {
            TampilkanPesan("Mahasiswa tidak ditemukan.", ConsoleColor.Yellow);
            return;
        }

        Console.WriteLine();
        foreach (Mahasiswa mahasiswa in hasil)
        {
            Console.WriteLine($"NRP : {mahasiswa.Nrp} | Nama: {mahasiswa.Nama} | Jurusan: {mahasiswa.Jurusan}");
        }
    }

    static void HapusMahasiswa()
    {
        TampilkanJudul("HAPUS MAHASISWA");
        Console.Write("Masukkan NRP yang akan dihapus: ");
        string nrp = Console.ReadLine()?.Trim() ?? "";
        Mahasiswa? mahasiswa = daftarMahasiswa.Find(data => data.Nrp.Equals(nrp, StringComparison.OrdinalIgnoreCase));

        if (mahasiswa is null)
        {
            TampilkanPesan("Mahasiswa tidak ditemukan.", ConsoleColor.Yellow);
            return;
        }

        daftarMahasiswa.Remove(mahasiswa);
        TampilkanPesan("Data mahasiswa berhasil dihapus.", ConsoleColor.Green);
    }

    static void TampilkanJudul(string judul)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"--- {judul} ---");
        Console.ResetColor();
        Console.WriteLine();
    }

    static void TampilkanPesan(string pesan, ConsoleColor warna)
    {
        Console.ForegroundColor = warna;
        Console.WriteLine(pesan);
        Console.ResetColor();
    }
}
