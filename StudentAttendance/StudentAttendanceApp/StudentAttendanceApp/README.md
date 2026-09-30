# Student Attendance App (WPF + C# + SQLite)

Mini project HOL — .NET Framework Programming (WPF & GUI). Tema: **Student Attendance**.

## Struktur Project

```
StudentAttendanceApp
├── StudentAttendanceApp.csproj     -> konfigurasi project + package SQLite
├── App.xaml / App.xaml.cs          -> konfigurasi aplikasi
├── MainWindow.xaml                 -> desain UI (XAML)
├── MainWindow.xaml.cs              -> event, validasi, logic CRUD
├── Models/
│   └── AttendanceRecord.cs         -> model data absensi
└── Data/
    └── DatabaseHelper.cs           -> akses database SQLite + 20 data dummy
```

## Cara Menjalankan (Visual Studio 2022 Community, gratis)

1. Install **Visual Studio 2022 Community** dan centang workload **.NET desktop development**.
2. `Create a new project` -> **WPF Application** (C#) -> nama: `StudentAttendanceApp` -> Framework **.NET 8.0**.
3. Ganti / tambahkan file sesuai struktur di atas (buat folder `Models` dan `Data` lewat Solution Explorer: klik kanan project -> Add -> New Folder).
4. Install package database: klik kanan project -> **Manage NuGet Packages** -> tab Browse -> cari `Microsoft.Data.Sqlite` -> Install.
   (Atau cukup pakai `StudentAttendanceApp.csproj` dari folder ini, package sudah tercantum.)
5. Tekan **F5**. Database `attendance.db` dibuat otomatis dan terisi 20 data dummy.

## Lokasi & Melihat Database

- File ada di `bin\Debug\net8.0-windows\attendance.db`.
- Untuk melihat isi tabel: install **DB Browser for SQLite** (gratis, https://sqlitebrowser.org), buka file `attendance.db`.
- Reset data dummy: tutup aplikasi, hapus `attendance.db`, jalankan lagi.

## Checklist Minimal Requirement

| Requirement | Pemenuhan |
|---|---|
| Minimal 5 jenis Control | TextBox, ComboBox, DatePicker, RadioButton, Button, DataGrid, TextBlock |
| Minimal 3 Event | Click, Loaded, SelectionChanged, TextChanged |
| Validasi input | NRP (wajib, 8-12 digit angka), nama, mata kuliah, tanggal (tidak boleh masa depan), status, keterangan wajib jika Izin/Sakit, cegah data duplikat |
| Tambah / Hapus | Simpan (INSERT), Hapus (DELETE + konfirmasi), bonus Update |
| Tampilan data | DataGrid + pencarian + filter status + counter ringkasan |

## Skenario Pengujian

| No | Skenario | Hasil yang diharapkan |
|---|---|---|
| 1 | Semua field kosong lalu Simpan | Pesan "NRP harus diisi!" |
| 2 | NRP berisi huruf | Pesan NRP harus angka 8-12 digit |
| 3 | Nama kosong | Pesan nama harus diisi |
| 4 | Mata kuliah belum dipilih | Pesan pilih mata kuliah |
| 5 | Status Izin/Sakit tanpa keterangan | Pesan keterangan wajib diisi |
| 6 | Data sama (NRP + matkul + tanggal) | Pesan data sudah ada |
| 7 | Semua data benar | Data masuk DataGrid, counter bertambah |
| 8 | Klik Reset | Form kembali kosong |
| 9 | Pilih baris + Hapus + Yes | Data terhapus dari tabel dan database |
| 10 | Pilih baris, ubah data, Update | Data berubah |
| 11 | Ketik di kotak cari | Tabel terfilter langsung |
| 12 | Filter Status = Alpha | Hanya data Alpha tampil |

## Catatan

- Jika dosen mewajibkan **.NET Framework klasik (4.8)**: buat project **WPF App (.NET Framework)**, lalu ganti `TargetFramework` menjadi `net48`, install NuGet `Microsoft.Data.Sqlite`, dan ganti tuple seed di `DatabaseHelper.cs` bila ada error bahasa C#.
