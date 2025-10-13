using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARPlacementController : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    private GameObject spawnedObject;

    private string prefabName;
    private string url;

    void Update()
    {
        // ✅ QRScanner에서 저장한 값 불러오기
        prefabName = PlayerPrefs.GetString("Prefab", "");
        url = PlayerPrefs.GetString("URL", "");

        // 디버그 로그
        Debug.Log($"[ARPlacementController] Prefab='{prefabName}', URL='{url}'");

        // ✅ 오브젝트 아직 없고, Prefab이 지정되어 있으면 배치 시도
        if (spawnedObject == null && !string.IsNullOrEmpty(prefabName))
        {
            TrySpawnPrefab(prefabName);
        }

        Debug.Log($"[ARPlacementController] touchCount= {Input.touchCount}");
        // ✅ 터치 감지 후 URL 열기
        if (spawnedObject != null && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
                Debug.Log($"[ARPlacementController] 123456");


            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log($"[ARPlacementController] touch Start");

                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                Debug.Log($"[ARPlacementController] ray Start");

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0))
                {
                    Debug.Log($"[ARPlacementController] Ray hit: {hit.transform.name}");

                    if (spawnedObject != null && hit.transform.gameObject == spawnedObject)
                    {
                        Debug.Log($"[ARPlacementController] Object touched, opening URL: {url}");
                        if (!string.IsNullOrEmpty(url))
                            Application.OpenURL(url);
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
            Debug.LogError($"[ARPlacementController] Prefab '{prefabName}' not found in Resources/Prefabs/");
            return;
        }

        // 🔹 화면 중앙에서 평면 감지
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            spawnedObject = Instantiate(prefab, hitPose.position, hitPose.rotation);
            Debug.Log($"[ARPlacementController] Spawned '{prefabName}' at {hitPose.position}");

            // ✅ Collider 자동 추가 (없을 경우)
            if (spawnedObject.GetComponent<Collider>() == null)
            {
                spawnedObject.AddComponent<BoxCollider>();
                Debug.Log($"[ARPlacementController] Collider added automatically to '{prefabName}'");
            }
        }
        else
        {
            Debug.LogWarning("[ARPlacementController] No plane detected yet, retrying...");
        }
    }
}
