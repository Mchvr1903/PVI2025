using UnityEngine;
using TMPro;

public class RaycastTouch : MonoBehaviour
{
    public Camera arCamera;
    public GameObject infoPanel;
    public TMP_Text judulTMP;
    public TMP_Text deskripsiTMP;
    public Vector2 panelOffset = new Vector2(120, 50);
    public bool debugMode = true;

    private GameObject lastHitObject;

    void Start()
    {
        if (arCamera == null)
        {
            arCamera = Camera.main;
        }

        if (infoPanel == null)
        {
            infoPanel = GameObject.Find("PANEL INFORMASI");
        }

        if (judulTMP == null && infoPanel != null)
        {
            judulTMP = infoPanel.transform.Find("JUDUL KOMPONEN")?.GetComponent<TMP_Text>();
        }

        if (deskripsiTMP == null && infoPanel != null)
        {
            deskripsiTMP = infoPanel.transform.Find("DESKRIPSI KOMPONEN")?.GetComponent<TMP_Text>();
        }

        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Touch touch = Input.touches[0];
            HandleTouch(touch.position);
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch(Input.mousePosition);
        }
#endif
    }

    private void HandleTouch(Vector2 screenPosition)
    {
        if (arCamera == null)
        {
            Debug.LogError("No AR Camera assigned!");
            return;
        }

        Ray ray = arCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            KomponenInfoMobile info = hit.collider.GetComponent<KomponenInfoMobile>();
            if (info != null)
            {
                ShowInfo(info, hit.point);
                lastHitObject = hit.collider.gameObject;
            }
            else
            {
                if (hit.collider.gameObject != lastHitObject && infoPanel != null && infoPanel.activeSelf)
                {
                    HideInfo();
                }
            }
        }
        else
        {
            if (infoPanel != null && infoPanel.activeSelf)
            {
                HideInfo();
            }
        }
    }

    public void ShowInfo(KomponenInfoMobile info, Vector3 hitPoint)
    {
        if (infoPanel == null || judulTMP == null || deskripsiTMP == null)
        {
            Debug.LogError("Missing UI references! Panel or Text components not assigned.");
            return;
        }

        judulTMP.text = string.IsNullOrEmpty(info.judulKomponen) ? "Komponen Tanpa Judul" : info.judulKomponen;
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
            screenPos.x = Mathf.Clamp(screenPos.x, 0 + (panelRect.rect.width / 2), Screen.width - (panelRect.rect.width / 2));
            screenPos.y = Mathf.Clamp(screenPos.y, 0 + (panelRect.rect.height / 2), Screen.height - (panelRect.rect.height / 2));

            panelRect.position = screenPos;
        }
    }

    public void HideInfo()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);

        lastHitObject = null;
    }
}
