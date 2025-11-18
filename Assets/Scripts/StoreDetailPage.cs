using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class StoreDetailPage : MonoBehaviour
{
    [SerializeField] private TMP_Text storeNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private UnityEngine.UI.Button urlButton; // ← 버튼만 표시

    private const string BASE_URL = "http://34.134.87.58:8080/stores";

    private string storeUrl = null; // 서버에서 받아온 URL 저장용

    void Start()
    {
        string storeId = PlayerPrefs.GetString("SelectedStoreID", "-1");
        string storeName = PlayerPrefs.GetString("SelectedStoreName", "알 수 없는 가게");

        storeNameText.text = storeName;

        // URL 로딩 전까지 버튼 비활성화
        urlButton.interactable = false;

        if (storeId == "-1")
        {
            descriptionText.text = "가게 ID를 불러올 수 없습니다.";
            return;
        }

        StartCoroutine(LoadStoreDetail(storeId));
    }

    IEnumerator LoadStoreDetail(string storeId)
    {
        string url = $"{BASE_URL}/{storeId}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                descriptionText.text = "서버 요청 실패: " + request.error;
                yield break;
            }

            // JSON 파싱
            StoreDetailResponse data = JsonUtility.FromJson<StoreDetailResponse>(request.downloadHandler.text);

            // UI 적용
            descriptionText.text = data.description;
            storeUrl = data.url;  // URL 저장

            // URL 있으면 버튼 활성화
            if (!string.IsNullOrEmpty(storeUrl))
            {
                urlButton.interactable = true;
                urlButton.onClick.RemoveAllListeners();
                urlButton.onClick.AddListener(() =>
                {
                    Application.OpenURL(storeUrl);
                });
            }
        }
    }

    public void OnBack()
    {
        SceneManager.LoadScene("StoreListScene");
    }
}

[System.Serializable]
public class StoreDetailResponse
{
    public string description;
    public string url;
}
