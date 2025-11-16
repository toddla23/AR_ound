using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using TMPro;  // ← TextMeshPro 추가

public class ARPlacementController : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private TMP_Text toastText; // ← TMP_Text로 변경

    private GameObject spawnedObject;

    private string prefabName;
    private string qrText;

    void Start()
    {
        if (toastText != null)
            toastText.gameObject.SetActive(false);
    }

    void Update()
    {
        prefabName = PlayerPrefs.GetString("Prefab", "");
        qrText = PlayerPrefs.GetString("Text", "");

        if (spawnedObject == null && !string.IsNullOrEmpty(prefabName))
            TrySpawnPrefab(prefabName);

        if (spawnedObject != null && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0))
                {
                    if (hit.transform.gameObject == spawnedObject)
                    {
                        ShowToast(qrText);
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

            if (!spawnedObject.TryGetComponent(out Collider _))
                spawnedObject.AddComponent<BoxCollider>();
        }
    }

    // -----------------------------
    // Toast 메시지 기능 (하이라이트 적용)
    // -----------------------------
    private void ShowToast(string message)
    {
        if (toastText == null) return;

        if (string.IsNullOrEmpty(message))
            message = "⚠ QR에 텍스트가 없습니다.";

        toastText.text = $"<mark=#d0ee17AA>{message}</mark>";

        toastText.gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(HideToast());
    }

    private System.Collections.IEnumerator HideToast()
    {
        yield return new WaitForSeconds(2f);
        toastText.gameObject.SetActive(false);
    }
}
