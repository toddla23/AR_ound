using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StoreDetailPage : MonoBehaviour
{
    [SerializeField] private TMP_Text storeNameText;
    [SerializeField] private TMP_Text descriptionText;

    void Start()
    {
        string name = PlayerPrefs.GetString("SelectedStoreName", "알 수 없는 가게");
        storeNameText.text = name;

        // 더미 설명 (실제 데이터가 있다면 불러오세요)
        descriptionText.text = "이곳은 " + name + "의 상세 설명입니다.";
    }

    public void OnBack()
    {
        SceneManager.LoadScene("StoreListScene");
    }
}
