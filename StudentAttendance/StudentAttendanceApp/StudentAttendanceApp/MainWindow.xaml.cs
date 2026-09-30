using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using StudentAttendanceApp.Data;
using StudentAttendanceApp.Models;

namespace StudentAttendanceApp
{
    /// <summary>
    /// Logic untuk MainWindow.xaml (event handler, validasi, dan CRUD ke database).
    /// </summary>
    public partial class MainWindow : Window
    {
        // Id data yang sedang dipilih di DataGrid (null = tidak ada yang dipilih)
        private int? _selectedId;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>Kumpulan RadioButton status agar mudah di-loop.</summary>
        private RadioButton[] StatusButtons
        {
            get { return new[] { rbHadir, rbIzin, rbSakit, rbAlpha }; }
        }

        // ============================================================
        // EVENT 1: Window Loaded
        // ============================================================
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DatabaseHelper.Initialize();   // buat tabel + 20 data dummy (jika kosong)
            }
            catch (Exception ex)
            {
                ShowError("Gagal menyiapkan database.", ex);
                return;
            }

            cmbMatkul.ItemsSource = DatabaseHelper.Courses;
            dpTanggal.DisplayDateEnd = DateTime.Today;
            ClearForm();
            LoadData();
        }

        // ============================================================
        // EVENT 2: Click tombol (Simpan, Update, Reset, Hapus)
        // ============================================================
        private void BtnSimpan_Click(object sender, RoutedEventArgs e)
        {
            var record = BuildRecordFromForm(null);
            if (record == null) return;

            try
            {
                DatabaseHelper.Insert(record);
            }
            catch (Exception ex)
            {
                ShowError("Gagal menyimpan data.", ex);
                return;
            }

            MessageBox.Show("Data absensi berhasil disimpan!", "Informasi",
                MessageBoxButton.OK, MessageBoxImage.Information);

            ClearForm();
            LoadData();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == null)
            {
                MessageBox.Show("Pilih data di tabel yang ingin diubah!", "Validasi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var record = BuildRecordFromForm(_selectedId);
            if (record == null) return;
            record.Id = _selectedId.Value;

            try
            {
                DatabaseHelper.Update(record);
            }
            catch (Exception ex)
            {
                ShowError("Gagal mengubah data.", ex);
                return;
            }

            MessageBox.Show("Data absensi berhasil diperbarui!", "Informasi",
                MessageBoxButton.OK, MessageBoxImage.Information);

            ClearForm();
            LoadData();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void BtnHapus_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == null)
            {
                MessageBox.Show("Pilih data yang ingin dihapus!", "Validasi",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Yakin ingin menghapus data absensi ini?", "Konfirmasi Hapus",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                DatabaseHelper.Delete(_selectedId.Value);
            }
            catch (Exception ex)
            {
                ShowError("Gagal menghapus data.", ex);
                return;
            }

            ClearForm();
            LoadData();
        }

        // ============================================================
        // EVENT 3: SelectionChanged (DataGrid dan filter status)
        // ============================================================
        private void DgAttendance_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;

            var record = dgAttendance.SelectedItem as AttendanceRecord;
            if (record != null)
            {
                _selectedId = record.Id;
                txtNim.Text = record.Nim;
                txtNama.Text = record.Nama;
                cmbMatkul.SelectedItem = record.MataKuliah;
                dpTanggal.SelectedDate = record.Tanggal;
                SetSelectedStatus(record.Status);
                txtKeterangan.Text = record.Keterangan;
            }
            else
            {
                _selectedId = null;
            }

            btnUpdate.IsEnabled = _selectedId != null;
            btnHapus.IsEnabled = _selectedId != null;
        }

        private void CmbFilterStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;   // hindari event saat InitializeComponent
            LoadData();
        }

        // ============================================================
        // EVENT 4: TextChanged (pencarian langsung)
        // ============================================================
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded) return;
            LoadData();
        }

        // ============================================================
        // HELPER: load data, ringkasan, form
        // ============================================================

        /// <summary>Ambil data dari database sesuai pencarian + filter, lalu tampilkan di DataGrid.</summary>
        private void LoadData()
        {
            string status = GetFilterStatus();
            try
            {
                List<AttendanceRecord> data = DatabaseHelper.GetAll(txtSearch.Text.Trim(), status);
                dgAttendance.ItemsSource = data;
                UpdateSummary(data);
            }
            catch (Exception ex)
            {
                ShowError("Gagal memuat data.", ex);
            }
        }

        private string GetFilterStatus()
        {
            var item = cmbFilterStatus.SelectedItem as ComboBoxItem;
            string text = item == null ? "" : item.Content.ToString();
            return text == "Semua Status" ? "" : text;
        }

        /// <summary>Counter jumlah data dan persentase kehadiran.</summary>
        private void UpdateSummary(List<AttendanceRecord> data)
        {
            int total = data.Count;
            int hadir = data.Count(d => d.Status == "Hadir");
            int izin = data.Count(d => d.Status == "Izin");
            int sakit = data.Count(d => d.Status == "Sakit");
            int alpha = data.Count(d => d.Status == "Alpha");
            double persen = total == 0 ? 0 : hadir * 100.0 / total;

            txtSummary.Text =
                $"Total: {total} data   |   Hadir: {hadir}   |   Izin: {izin}   |   " +
                $"Sakit: {sakit}   |   Alpha: {alpha}   |   Kehadiran: {persen:0.#}%";
        }

        /// <summary>Kosongkan form dan kembalikan ke nilai awal.</summary>
        private void ClearForm()
        {
            dgAttendance.UnselectAll();
            _selectedId = null;

            txtNim.Clear();
            txtNama.Clear();
            cmbMatkul.SelectedIndex = -1;
            dpTanggal.SelectedDate = DateTime.Today;
            rbHadir.IsChecked = true;
            txtKeterangan.Clear();

            btnUpdate.IsEnabled = false;
            btnHapus.IsEnabled = false;
            txtNim.Focus();
        }

        private string GetSelectedStatus()
        {
            var checkedButton = StatusButtons.FirstOrDefault(rb => rb.IsChecked == true);
            return checkedButton == null ? null : checkedButton.Tag.ToString();
        }

        private void SetSelectedStatus(string status)
        {
            foreach (var rb in StatusButtons)
            {
                rb.IsChecked = rb.Tag.ToString() == status;
            }
        }

        // ============================================================
        // VALIDASI INPUT
        // ============================================================

        /// <summary>
        /// Validasi semua input form. Jika valid, kembalikan AttendanceRecord;
        /// jika tidak valid, tampilkan pesan lalu kembalikan null.
        /// </summary>
        private AttendanceRecord BuildRecordFromForm(int? excludeId)
        {
            string nim = txtNim.Text.Trim();
            string nama = txtNama.Text.Trim();
            string keterangan = txtKeterangan.Text.Trim();
            string status = GetSelectedStatus();

            if (string.IsNullOrWhiteSpace(nim))
            {
                Warn("NIM harus diisi!");
                txtNim.Focus();
                return null;
            }

            if (!Regex.IsMatch(nim, @"^\d{8,12}$"))
            {
                Warn("NIM harus berupa angka 8-12 digit!");
                txtNim.Focus();
                return null;
            }

            if (string.IsNullOrWhiteSpace(nama))
            {
                Warn("Nama mahasiswa harus diisi!");
                txtNama.Focus();
                return null;
            }

            if (nama.Length < 3)
            {
                Warn("Nama minimal 3 karakter!");
                txtNama.Focus();
                return null;
            }

            if (cmbMatkul.SelectedItem == null)
            {
                Warn("Pilih mata kuliah!");
                cmbMatkul.Focus();
                return null;
            }

            if (dpTanggal.SelectedDate == null)
            {
                Warn("Pilih tanggal absensi!");
                dpTanggal.Focus();
                return null;
            }

            if (dpTanggal.SelectedDate.Value.Date > DateTime.Today)
            {
                Warn("Tanggal absensi tidak boleh melebihi hari ini!");
                dpTanggal.Focus();
                return null;
            }

            if (status == null)
            {
                Warn("Pilih status kehadiran!");
                return null;
            }

            if ((status == "Izin" || status == "Sakit") && string.IsNullOrWhiteSpace(keterangan))
            {
                Warn("Keterangan wajib diisi untuk status Izin atau Sakit!");
                txtKeterangan.Focus();
                return null;
            }

            string matkul = cmbMatkul.SelectedItem.ToString();
            DateTime tanggal = dpTanggal.SelectedDate.Value.Date;

            try
            {
                if (DatabaseHelper.IsDuplicate(nim, matkul, tanggal, excludeId))
                {
                    Warn("Absensi untuk NIM, mata kuliah, dan tanggal tersebut sudah ada!");
                    return null;
                }
            }
            catch (Exception ex)
            {
                ShowError("Gagal memeriksa data duplikat.", ex);
                return null;
            }

            return new AttendanceRecord
            {
                Nim = nim,
                Nama = nama,
                MataKuliah = matkul,
                Tanggal = tanggal,
                Status = status,
                Keterangan = keterangan
            };
        }

        private void Warn(string message)
        {
            MessageBox.Show(message, "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ShowError(string message, Exception ex)
        {
            MessageBox.Show(message + "\n\nDetail: " + ex.Message, "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
