using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class StoreListManager : MonoBehaviour
{
    [SerializeField] private RectTransform contentParent; // ScrollView Content
    [SerializeField] private StoreItem itemPrefab;        // StoreItem 프리팹 드래그

    private string storeListUrl = "http://localhost:8080/stores"; // 테스트용 URL

    void Start()
    {
        // JSON 다운로드 시작
        StartCoroutine(LoadStoreListFromServer());
    }

    IEnumerator LoadStoreListFromServer()
    {
        // 예시) 실제 요청
        UnityWebRequest request = UnityWebRequest.Get(storeListUrl);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"❌ 서버 요청 실패: {request.error}");
            // 실패 시 임시 더미데이터 사용
            List<StoreData> dummy = new List<StoreData>
            {
                new StoreData("1", "스타벅스"),
            };
            PopulateStoreList(dummy);
        }
        else
        {
            // ✅ JSON 파싱
            string json = request.downloadHandler.text;

            // JSON 배열을 감싸기 위해 Wrapper 사용
            json = "{\"items\":" + json + "}";

            StoreListWrapper wrapper = JsonUtility.FromJson<StoreListWrapper>(json);
            PopulateStoreList(wrapper.items);
        }
    }

    void PopulateStoreList(List<StoreData> stores)
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var s in stores)
        {
            StoreItem item = Instantiate(itemPrefab, contentParent);
            item.transform.localScale = Vector3.one;
            item.Init(s, OnStoreSelected);
        }
    }

    void OnStoreSelected(StoreData selected)
    {
        PlayerPrefs.SetString("SelectedStoreID", selected.id);
        PlayerPrefs.SetString("SelectedStoreName", selected.name);
        SceneManager.LoadScene("StoreDetailScene");
    }

    public void OnBackButton()
    {
        SceneManager.LoadScene("MainPage");
    }

    [System.Serializable]
    private class StoreListWrapper
    {
        public List<StoreData> items;
    }
}
