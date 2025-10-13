using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;
using Unity.Collections;
using UnityEngine.UI; // ✅ UI 사용 추가

public class QRScanner : MonoBehaviour
{
    [SerializeField] private ARCameraManager arCameraManager;
    [SerializeField] private Text toastText; // ✅ Toast 텍스트 UI 연결
    private Texture2D cameraTexture;
    private bool isProcessing = false;
    private bool isScanned = false; // ✅ 중복 스캔 방지용

    private void OnEnable()
    {
        if (arCameraManager != null)
            arCameraManager.frameReceived += OnCameraFrameReceived;
        else
            Debug.LogError("[QRScanner] ARCameraManager NOT assigned!");

        HideToast(); // 시작 시 숨김
    }

    private void OnDisable()
    {
        if (arCameraManager != null)
            arCameraManager.frameReceived -= OnCameraFrameReceived;
    }

    private void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        if (isProcessing || isScanned) return;

        if (arCameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            StartCoroutine(ProcessImage(image));
        }
    }

    private IEnumerator ProcessImage(XRCpuImage image)
    {
        isProcessing = true;

        var conversionParams = new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(256, 256),
            outputFormat = TextureFormat.RGBA32,
            transformation = XRCpuImage.Transformation.MirrorY
        };

        var rawTextureData = new NativeArray<byte>(image.GetConvertedDataSize(conversionParams), Allocator.Temp);
        image.Convert(conversionParams, rawTextureData);
        image.Dispose();

        if (cameraTexture == null)
            cameraTexture = new Texture2D(conversionParams.outputDimensions.x,
                                          conversionParams.outputDimensions.y,
                                          TextureFormat.RGBA32, false);

        cameraTexture.LoadRawTextureData(rawTextureData);
        cameraTexture.Apply();
        rawTextureData.Dispose();

        try
        {
            var barcodeReader = new BarcodeReader { AutoRotate = true, TryInverted = true };
            var result = barcodeReader.Decode(cameraTexture.GetPixels32(), cameraTexture.width, cameraTexture.height);

            if (result != null)
            {
                Debug.Log($"✅ QR Detected: {result.Text}");
                ParseQRData(result.Text);
                isScanned = true;
                ShowToast("QR 인식 완료!"); // ✅ 화면에 표시
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[QRScanner] QR Decode Error: {e.Message}");
        }

        yield return new WaitForSeconds(0.2f);
        isProcessing = false;
    }

    private void ParseQRData(string data)
    {
        // 예: "Prefab=ObjectA;URL=https://www.youtube.com/watch?v=_J-FCm91_is"
        var pairs = data.Split(';');
        foreach (var p in pairs)
        {
            var kv = p.Split(new char[] { '=' }, 2); // 🔹 URL 안의 '=' 허용
            if (kv.Length == 2)
            {
                string key = kv[0].Trim();
                string value = kv[1].Trim();

                PlayerPrefs.SetString(key, value);
                Debug.Log($"[QRScanner] Saved {key} = {value}");
            }
            else
            {
                Debug.LogWarning($"[QRScanner] Invalid pair skipped: {p}");
            }
        }
        PlayerPrefs.Save();
    }

    // ✅ 간단한 Toast UI 표시
    private void ShowToast(string message)
    {
        if (toastText == null) return;
        toastText.text = message;
        toastText.gameObject.SetActive(true);
        StartCoroutine(HideToastAfterDelay(2f)); // 2초 후 숨기기
    }

    private IEnumerator HideToastAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideToast();
    }

    private void HideToast()
    {
        if (toastText != null)
            toastText.gameObject.SetActive(false);
    }
}
