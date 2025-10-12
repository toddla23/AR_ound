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
        // QRScanner에서 PlayerPrefs로 저장한 데이터 읽기
        prefabName = PlayerPrefs.GetString("Prefab");
        url = PlayerPrefs.GetString("URL");
        Debug.LogError($"[ARPlacementController] '{prefabName}' catched");
        Debug.LogError($"[ARPlacementController] '{url}' catched");


        // 아직 스폰되지 않았고, 프리팹 이름이 있으면 스폰 시도
        if (!string.IsNullOrEmpty(prefabName) && spawnedObject == null)
        {
            TrySpawnPrefab(prefabName);
        }

        // 터치 감지
        if (spawnedObject != null && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                CheckTouch(touch.position);
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

        // 화면 중앙에서 평면 감지
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes))
        {
            Pose hitPose = hits[0].pose;
            spawnedObject = Instantiate(prefab, hitPose.position, hitPose.rotation);
            Debug.Log($"[ARPlacementController] Spawned {prefabName} at {hitPose.position}");
        }
    }

    private void CheckTouch(Vector2 touchPosition)
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        if (raycastManager.Raycast(touchPosition, hits, TrackableType.Planes))
        {
            // 터치가 스폰된 오브젝트 근처면 URL 열기
            Ray ray = Camera.main.ScreenPointToRay(touchPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.gameObject == spawnedObject)
                {
                    Debug.Log($"[ARPlacementController] Opening URL: {url}");
                    if (!string.IsNullOrEmpty(url))
                        Application.OpenURL(url);
                }
            }
        }
    }
    
}
