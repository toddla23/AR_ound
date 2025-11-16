using UnityEngine;
using UnityEngine.UI;   // ← Toast UI용

public class ARObjectTouchHandler : MonoBehaviour
{
    [SerializeField] private Text toastText;  // ← Toast UI 연결 필요!

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    ShowToastFromQR();
                }
            }
        }
    }

    private void ShowToastFromQR()
    {
        // QRScanner에서 저장한 텍스트 가져오기
        string qrMessage = PlayerPrefs.GetString("Text", "");

        if (string.IsNullOrEmpty(qrMessage))
        {
            qrMessage = "⚠️ QR 텍스트가 없습니다.";
        }

        Debug.Log("[TouchHandler] Toast Text = " + qrMessage);

        if (toastText != null)
        {
            toastText.text = qrMessage;
            toastText.gameObject.SetActive(true);
            StartCoroutine(HideToast());
        }
        else
        {
            Debug.LogWarning("⚠ toastText UI가 연결되지 않았습니다!");
        }
    }

    private System.Collections.IEnumerator HideToast()
    {
        yield return new WaitForSeconds(2f);
        toastText.gameObject.SetActive(false);
    }
}
