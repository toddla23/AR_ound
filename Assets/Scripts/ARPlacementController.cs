using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Networking;

public class ARPlacementController : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private TMP_Text toastText;

    private GameObject spawnedObject;

    private string prefabName;
    private string qrTextUrl;  // ← GET 요청용 URL
    private string qrOptionUrl; // ← QR의 URL= 항목

    void Start()
    {
        if (toastText != null)
            toastText.gameObject.SetActive(false);
    }

    void Update()
    {
        prefabName = PlayerPrefs.GetString("Prefab", "");
        qrTextUrl = PlayerPrefs.GetString("Text", "");  // 설명 URL
        qrOptionUrl = PlayerPrefs.GetString("URL", ""); // 옵션 URL

        if (spawnedObject == null && !string.IsNullOrEmpty(prefabName))
            TrySpawnPrefab(prefabName);

        if (spawnedObject != null && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.transform.gameObject == spawnedObject)
                    {
                        // 🔥 터치하면 QR의 URL로 GET 요청!
                        if (!string.IsNullOrEmpty(qrOptionUrl))
                            StartCoroutine(RequestOption(qrOptionUrl));
                        else
                            ShowToast("<mark=#ff4444AA>URL 값이 없습니다.</mark>");
                    }
                }
            }
        }
    }

    private void TrySpawnPrefab(string prefabName)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");
        if (prefab == null)
        {
            Debug.LogError($"Prefab '{prefabName}' not found!");
            return;
        }

        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(
            new Vector2(Screen.width / 2, Screen.height / 2),
            hits,
            TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            spawnedObject = Instantiate(prefab, hitPose.position, hitPose.rotation);

            // ⭐ 오브젝트 크기 줄이기 (예: 30% 크기)
            spawnedObject.transform.localScale = prefab.transform.localScale * 0.3f;

            if (!spawnedObject.TryGetComponent(out Collider _))
                spawnedObject.AddComponent<BoxCollider>();
        }
    }

    // -----------------------------
    // 🔥 URL GET 요청 (옵션 데이터 요청)
    // -----------------------------
    private System.Collections.IEnumerator RequestOption(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            ShowToast($"<mark=#ff4444AA>Error:\n{request.error}</mark>");
        }
        else
        {

            string highlighted = $"수집 완료!";

            ShowToast(highlighted);
        }
    }

    // -----------------------------
    // Toast 메시지 기능
    // -----------------------------
    private void ShowToast(string message)
    {
        if (toastText == null) return;

        toastText.text = message;
        toastText.gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(HideToast());
    }

    private System.Collections.IEnumerator HideToast()
    {
        yield return new WaitForSeconds(3f);
        toastText.gameObject.SetActive(false);
    }
}
