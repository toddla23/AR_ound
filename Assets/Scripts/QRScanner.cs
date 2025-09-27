using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using TMPro;

public class QRScanner : MonoBehaviour
{
    [SerializeField] private RawImage cameraPreview;
    [SerializeField] private AspectRatioFitter aspectRatioFitter;
    [SerializeField] private TMP_Text resultText;

    private WebCamTexture webcamTexture;
    private bool isScanning = false;

    void Start()
    {
        // 기기 카메라 가져오기
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            webcamTexture = new WebCamTexture(devices[0].name);
            cameraPreview.texture = webcamTexture;
            webcamTexture.Play();
            isScanning = true;
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

        // 화면 회전 보정
        if (aspectRatioFitter != null && cameraPreview != null)
        {
            // 회전 적용
            cameraPreview.rectTransform.localEulerAngles = new Vector3(0, 0, -webcamTexture.videoRotationAngle);

            // 세로 반전 보정
            cameraPreview.uvRect = new Rect(
                webcamTexture.videoVerticallyMirrored ? 1 : 0,
                0,
                webcamTexture.videoVerticallyMirrored ? -1 : 1,
                1
            );
        }
    }


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

                    // 👉 여기서 AR 씬으로 이동
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
        {
            webcamTexture.Stop();
        }
    }
}
