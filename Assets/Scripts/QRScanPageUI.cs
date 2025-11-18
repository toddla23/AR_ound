using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QRScanPageUI : MonoBehaviour
{
    [SerializeField] private Button qrScanButton1;



    private void Start()
    {
        qrScanButton1.onClick.AddListener(OnQRScanClicked);
    }

    private void OnQRScanClicked()
    {
        SceneManager.LoadScene("StoreListScene");
    }


    private void Update()
    {
        // 안드로이드 뒤로가기 버튼 처리
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            SceneManager.LoadScene("StoreListScene");

        }
    }
}
