using UnityEngine;

public class KomponenInfoMobile : MonoBehaviour
{
    // Properti untuk judul
    public string judulKomponen;

    // Properti untuk deskripsi
    public string deskripsiKomponen;

    // Properti untuk audio deskripsi
    public AudioClip AudioDeskripsi;

    // Method untuk memutar audio ketika collider di-trigger
    public void PlayAudio()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null && AudioDeskripsi != null)
        {
            audioSource.clip = AudioDeskripsi;
            audioSource.Play();
        }
    }

    // Trigger ketika objek dengan collider ini bertabrakan dengan objek lain (misalnya player)
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Pastikan objek yang bertabrakan adalah player
        {
            PlayAudio();  // Panggil fungsi untuk memutar audio
        }
    }
}
