using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoreListManager : MonoBehaviour
{
    [SerializeField] private RectTransform contentParent; // ScrollView Content
    [SerializeField] private StoreItem itemPrefab;        // StoreItem 타입으로 드래그

    // 더미 데이터 (원하면 Inspector에서 넣어도 됨)
    private List<StoreData> storeList = new List<StoreData>
    {
        new StoreData("1", "스타벅스"),
        new StoreData("2", "이디야"),
        new StoreData("3", "메가커피"),
        new StoreData("4", "투썸플레이스"),
        new StoreData("5", "빽다방")
    };

    void Start()
    {
        PopulateStoreList(storeList);
    }

    void PopulateStoreList(List<StoreData> stores)
    {
        // 기존 항목 삭제 (테스트용)
        foreach (Transform child in contentParent) Destroy(child.gameObject);

        foreach (var s in stores)
        {
            // Instantiate (프리팹은 StoreItem 타입)
            StoreItem item = Instantiate(itemPrefab, contentParent);
            item.transform.localScale = Vector3.one; // Scale 문제 예방

            // 초기화: 클릭 시 OnStoreSelected 호출
            item.Init(s, OnStoreSelected);
        }
    }

    void OnStoreSelected(StoreData selected)
    {
        // 선택 정보 저장 (간단한 방법)
        PlayerPrefs.SetString("SelectedStoreID", selected.id);
        PlayerPrefs.SetString("SelectedStoreName", selected.name);

        // 상세 씬으로 이동
        SceneManager.LoadScene("StoreDetailScene");
    }

    // Back 버튼에서 호출
    public void OnBackButton()
    {
        SceneManager.LoadScene("MainPage");
    }
}
