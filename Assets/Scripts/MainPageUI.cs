using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainPageUI : MonoBehaviour
{
    [SerializeField] private Button qrScanButton;
    [SerializeField] private Button storeListButton;

    private void Start()
    {
        qrScanButton.onClick.AddListener(OnQRScanClicked);
        storeListButton.onClick.AddListener(OnStoreListClicked);
    }

    private void OnQRScanClicked()
    {
        SceneManager.LoadScene("QRScanScene");
    }

    private void OnStoreListClicked()
    {
        SceneManager.LoadScene("StoreListScene");
    }

    private void Update()
    {
        // 안드로이드 뒤로가기 버튼 처리
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 현재 씬이 메인페이지인지 확인
            if (SceneManager.GetActiveScene().name == "MainPage")
            {
                // 메인페이지에서는 앱 종료
                Application.Quit();
            }
            else
            {
                // 다른 씬에서는 이전 씬(메인페이지)으로 돌아가기
                SceneManager.LoadScene("MainPage");
            }
        }
    }
}
