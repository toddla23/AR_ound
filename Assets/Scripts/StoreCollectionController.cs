using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class StoreCollectionController : MonoBehaviour
{
    [SerializeField] private Transform content;          // Grid Layout Content
    [SerializeField] private GameObject imageItemPrefab; // Image UI 프리팹

    private string requestUrl = "http://34.134.87.58:8080/collections"; // 서버 URL

    [System.Serializable]
    public class StoreItem
    {
        public int store_id;
    }

    [System.Serializable]
    public class StoreListWrapper
    {
        public StoreItem[] items;
    }

    void Start()
    {
        StartCoroutine(LoadStoreList());
    }

    private IEnumerator LoadStoreList()
    {
        UnityWebRequest request = UnityWebRequest.Get(requestUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("GET Error : " + request.error);
            yield break;
        }

        string rawJson = request.downloadHandler.text;

        // JSON 배열을 강제로 래핑
        string wrappedJson = "{\"items\":" + rawJson + "}";

        StoreListWrapper data = JsonUtility.FromJson<StoreListWrapper>(wrappedJson);

        foreach (var item in data.items)
        {
            SpawnStoreImage(item.store_id);
        }
    }

    private void SpawnStoreImage(int storeId)
    {
        Sprite sprite = Resources.Load<Sprite>($"StoreImages/{storeId}");

        if (sprite == null)
        {
            Debug.LogWarning($"이미지 {storeId}.png 를 찾지 못했습니다.");
            return;
        }

        GameObject item = Instantiate(imageItemPrefab, content);
        Image img = item.GetComponent<Image>();
        img.sprite = sprite;
    }
}
