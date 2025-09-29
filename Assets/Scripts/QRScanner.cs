using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using TMPro;
using UnityEngine.SceneManagement;

public class QRScanner : MonoBehaviour
{
    [SerializeField] private RawImage cameraPreview;   // 카메라 출력
    [SerializeField] private TMP_Text resultText;      // QR 결과 표시

    private WebCamTexture webcamTexture;
    private bool isScanning = false;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            webcamTexture = new WebCamTexture(devices[0].name);
            cameraPreview.texture = webcamTexture;
            webcamTexture.Play();
            isScanning = true;

            // 카메라 크기 조정
            StartCoroutine(AdjustCameraWidth());
            StartCoroutine(ScanQRCode());
        }
        else
        {
            resultText.text = "카메라를 찾을 수 없습니다.";
        }
    }

    void Update()
    {
        if (webcamTexture == null || !webcamTexture.isPlaying) return;

        // 회전 및 세로 반전 보정
        cameraPreview.rectTransform.localEulerAngles = new Vector3(0, 0, -webcamTexture.videoRotationAngle);
        cameraPreview.uvRect = new Rect(
            webcamTexture.videoVerticallyMirrored ? 1 : 0,
            0,
            webcamTexture.videoVerticallyMirrored ? -1 : 1,
            1
        );

        // 안드로이드 뒤로가기
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainPage");
        }
    }

    // 화면 가로 기준으로 카메라 꽉 채우기
    IEnumerator AdjustCameraWidth()
    {
        yield return new WaitUntil(() => webcamTexture.width > 100);

        RectTransform parentRect = cameraPreview.transform.parent.GetComponent<RectTransform>();
        float screenWidth = parentRect.rect.width;

        float videoRatio = (float)webcamTexture.width / webcamTexture.height;
        float targetHeight = screenWidth / videoRatio;

        // RawImage 크기 적용
        cameraPreview.rectTransform.sizeDelta = new Vector2(screenWidth, targetHeight);

        // 중앙 정렬
        cameraPreview.rectTransform.anchoredPosition = Vector2.zero;
    }

    // QR 코드 스캔
    IEnumerator ScanQRCode()
    {
        IBarcodeReader barcodeReader = new BarcodeReader();

        while (isScanning)
        {
            try
            {
                var result = barcodeReader.Decode(webcamTexture.GetPixels32(),
                                                  webcamTexture.width,
                                                  webcamTexture.height);
                if (result != null)
                {
                    resultText.text = "QR 코드 인식됨: " + result.Text;
                    isScanning = false;

                    // ARScene 이동 예시
                    // SceneManager.LoadScene("ARScene");
                }
            }
            catch { }

            yield return null;
        }
    }

    private void OnDestroy()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
            webcamTexture.Stop();
    }
}
