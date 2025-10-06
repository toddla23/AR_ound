using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class StoreDetailPage : MonoBehaviour
{
    [SerializeField] private TMP_Text storeNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text urlText;

    private const string BASE_URL = "http://localhost:8080/stores";

    void Start()
    {
        // StoreListManager에서 저장한 정보 불러오기
        string storeId = PlayerPrefs.GetString("SelectedStoreID", "-1");
        string storeName = PlayerPrefs.GetString("SelectedStoreName", "알 수 없는 가게");

        storeNameText.text = storeName;

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

            // UI 표시
            descriptionText.text = data.description;
            urlText.text = data.url;

            // URL 클릭 이벤트 등록
            AddUrlClickListener(data.url);
        }
    }

    void AddUrlClickListener(string url)
    {
        // TextMeshPro는 Button이 아니라 직접 클릭 감지해야 함
        var urlButton = urlText.GetComponent<UnityEngine.UI.Button>();
        if (urlButton == null)
        {
            // 버튼 컴포넌트가 없으면 자동 추가
            urlButton = urlText.gameObject.AddComponent<UnityEngine.UI.Button>();
        }

        urlButton.onClick.RemoveAllListeners();
        urlButton.onClick.AddListener(() =>
        {
            Application.OpenURL(url);
        });
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
