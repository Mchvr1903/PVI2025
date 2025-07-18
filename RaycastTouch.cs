using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class RaycastTouch : MonoBehaviour
{
    public Camera arCamera;
    public GameObject infoPanel;
    public TMP_Text judulTMP;
    public TMP_Text deskripsiTMP;
    public Vector2 panelOffset = new Vector2(120, 50);
    public bool debugMode = true;

    private GameObject lastHitObject;
    private AudioSource audioSource;
    public bool isTouchActive = false; // State untuk mendeteksi sentuhan aktif

    void Start()
    {
        Debug.Log("RaycastTouch script initialized.");

        if (arCamera == null)
        {
            arCamera = Camera.main;
            if (debugMode) Debug.LogWarning("AR Camera tidak diatur di inspector. AR Camera diatur ke Camera.main.");
        }

        if (infoPanel == null)
        {
            infoPanel = GameObject.Find("PANEL INFORMASI");
            if (infoPanel == null)
                Debug.LogError("Panel Info tidak ditemukan!");
        }

        if (judulTMP == null && infoPanel != null)
            judulTMP = infoPanel.transform.Find("JUDUL KOMPONEN")?.GetComponent<TMP_Text>();
        if (deskripsiTMP == null && infoPanel != null)
            deskripsiTMP = infoPanel.transform.Find("DESKRIPSI KOMPONEN")?.GetComponent<TMP_Text>();
        if (infoPanel != null)
            infoPanel.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // Periksa apakah pointer berada di atas UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            isTouchActive = false; // Tidak ada sentuhan karena UI aktif
            return; // Abaikan raycasting jika menyentuh elemen UI
        }

        // Tangani input layar sentuh
        if (Touchscreen.current?.primaryTouch.press.isPressed == true)
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            if (debugMode) Debug.Log($"Raycast Fisika (Sentuh) di posisi: {touchPosition}");
            HandleTouch(touchPosition);
        }

        // Tangani input mouse hanya pada Unity Editor
#if UNITY_EDITOR
        if (Mouse.current?.leftButton.isPressed == true)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            if (debugMode) Debug.Log($"Raycast Fisika (Mouse) di posisi: {mousePosition}");
            HandleTouch(mousePosition);
        }
#endif
    }

    private void HandleTouch(Vector2 screenPosition)
    {
        if (arCamera == null)
        {
            Debug.LogError("Kamera AR tidak diatur!");
            return;
        }

        Ray ray = arCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            isTouchActive = true; // Sentuhan terdeteksi
            if (debugMode) Debug.Log($"Raycast mengenai objek: {hit.collider.name}");

            // Dapatkan informasi dari komponen terkait
            KomponenInfoMobile info = hit.collider.GetComponent<KomponenInfoMobile>();
            if (info != null)
            {
                ShowInfo(info, hit.point);
                PlayAudio(info);
                lastHitObject = hit.collider.gameObject;
            }
        }
        else
        {
            isTouchActive = false; // Tidak ada sentuhan saat ini
            if (infoPanel?.activeSelf == true)
            {
                HideInfo();
            }
        }
    }

    public void ShowInfo(KomponenInfoMobile info, Vector3 hitPoint)
    {
        if (infoPanel == null || judulTMP == null || deskripsiTMP == null)
        {
            Debug.LogError("Referensi ke UI (InfoPanel atau TMP) tidak diatur!");
            return;
        }

        judulTMP.text = string.IsNullOrEmpty(info.judulKomponen) ? "Tanpa Judul" : info.judulKomponen;
        deskripsiTMP.text = string.IsNullOrEmpty(info.deskripsiKomponen) ? "Tidak ada deskripsi." : info.deskripsiKomponen;
        if (!infoPanel.activeSelf)
            infoPanel.SetActive(true);
        PositionPanelNearPoint(hitPoint);
    }

    private void PositionPanelNearPoint(Vector3 worldPoint)
    {
        Vector2 screenPos = arCamera.WorldToScreenPoint(worldPoint);
        screenPos += panelOffset;

        RectTransform panelRect = infoPanel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            screenPos.x = Mathf.Clamp(screenPos.x, panelRect.rect.width / 2, Screen.width - panelRect.rect.width / 2);
            screenPos.y = Mathf.Clamp(screenPos.y, panelRect.rect.height / 2, Screen.height - panelRect.rect.height / 2);
            panelRect.position = new Vector3(screenPos.x, screenPos.y, panelRect.position.z);
        }
    }

    private void PlayAudio(KomponenInfoMobile info)
    {
        if (audioSource != null && info.AudioDeskripsi != null)
        {
            audioSource.clip = info.AudioDeskripsi;
            audioSource.Play();
            if (debugMode) Debug.Log($"Memainkan audio: {info.AudioDeskripsi.name}");
        }
    }

    public void HideInfo()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);

        if (audioSource != null && audioSource.isPlaying)
            audioSource.Stop();

        lastHitObject = null;
    }

    public void OnNextButtonPressed()
    {
        Debug.Log("FUNGSI TOMBOL TERPANGGIL! Memuat scene POSTEST...");
        SceneManager.LoadScene("POSTEST");
    }
}