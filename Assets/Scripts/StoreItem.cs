using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class StoreItem : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Button button;

    private StoreData data;

    // 초기화: 매니저에서 Instantiate 후 이 함수 호출
    public void Init(StoreData storeData, UnityAction<StoreData> onClick)
    {
        data = storeData;
        if (nameText != null) nameText.text = storeData.name;

        // 안전하게 리스너 제거 후 추가
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke(data));
        }
    }
}
