using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Fungsi untuk memindahkan scene berdasarkan nama
    public void PindahScene(string namaScene)
    {
        Debug.Log($"Fungsi dipanggil dengan parameter: {namaScene}");

        // Periksa nama scene kosong atau null
        if (string.IsNullOrEmpty(namaScene))
        {
            Debug.LogError("Nama scene kosong atau null!");
            return; // Berhenti jalankan fungsi
        }

        // Cleanup atau disable plugin sebelum memindahkan scene
        if (Vuforia.VuforiaBehaviour.Instance != null) // Vuforia hanya jika aktif
        {
            Vuforia.VuforiaBehaviour.Instance.enabled = false;
            Debug.Log("Vuforia AR dinonaktifkan sebelum scene dipindah.");
        }

        // Log untuk konfirmasi loading scene
        Debug.Log("Memuat scene: " + namaScene);

        // Pindahkan ke scene tujuan
        SceneManager.LoadScene(namaScene);
    }

    // Fungsi untuk keluar dari aplikasi
    public void KeluarAplikasi()
    {
        Debug.Log("Aplikasi keluar.");
        Application.Quit(); // Keluar aplikasi
    }
}