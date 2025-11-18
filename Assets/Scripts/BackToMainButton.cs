using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackToMainButton : MonoBehaviour
{
    private void Start()
    {
        // UI 버튼 클릭 이벤트 연결
        // GetComponent<Button>().onClick.AddListener(OnBackClicked);
    }

    private void Update()
    {
        // 안드로이드 하드웨어 뒤로가기 버튼 처리
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoBack();
        }
    }

    // private void OnBackClicked()
    // {
    //     GoBack();
    // }

    private void GoBack()
    {
        // 👉 메인페이지로 이동
        SceneManager.LoadScene("MainPage");
    }
}
