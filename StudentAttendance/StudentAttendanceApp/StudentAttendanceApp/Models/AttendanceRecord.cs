using System;

namespace StudentAttendanceApp.Models
{
    /// <summary>
    /// Model satu baris data absensi mahasiswa.
    /// </summary>
    public class AttendanceRecord
    {
        public int Id { get; set; }
        public string Nim { get; set; }
        public string Nama { get; set; }
        public string MataKuliah { get; set; }
        public DateTime Tanggal { get; set; }
        public string Status { get; set; }      // Hadir / Izin / Sakit / Alpha
        public string Keterangan { get; set; }

        /// <summary>Format tanggal untuk ditampilkan di DataGrid.</summary>
        public string TanggalText
        {
            get { return Tanggal.ToString("dd-MM-yyyy"); }
        }
    }
}
