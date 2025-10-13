using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;
using Unity.Collections;

public class QRScanner : MonoBehaviour
{
    [SerializeField] private ARCameraManager arCameraManager;

    private Texture2D cameraTexture;
    private bool isProcessing = false;

    private void OnEnable()
    {
        if (arCameraManager != null)
            arCameraManager.frameReceived += OnCameraFrameReceived;
        else
            Debug.LogError("[QRScanner] ARCameraManager NOT assigned!");
    }

    private void OnDisable()
    {
        if (arCameraManager != null)
            arCameraManager.frameReceived -= OnCameraFrameReceived;
    }

    private void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        if (isProcessing) return;

        if (arCameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
        {
            StartCoroutine(ProcessImage(image));
        }
    }

    private IEnumerator ProcessImage(XRCpuImage image)
    {
        isProcessing = true;

        // 낮은 해상도로 변환해서 ZXing 처리 속도 최적화
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

        Debug.Log($"[QRScanner] Processing Texture {cameraTexture.width}x{cameraTexture.height}");

        // ZXing Decode
        try
        {
            var barcodeReader = new BarcodeReader
            {
                AutoRotate = true,
                TryInverted = true
            };

            var result = barcodeReader.Decode(cameraTexture.GetPixels32(), cameraTexture.width, cameraTexture.height);

            if (result != null)
            {
                Debug.Log($"✅ QR Detected: {result.Text}");
                ParseQRData(result.Text);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[QRScanner] QR Decode Error: {e.Message}");
        }

        // 다음 프레임까지 대기
        yield return new WaitForSeconds(0.2f);
        isProcessing = false;
    }

    private void ParseQRData(string data)
    {
        // 예: "Prefab=ObjectA;URL=https://www.youtube.com/watch?v=_J-FCm91_is"
        var pairs = data.Split(';');
        foreach (var p in pairs)
        {
            var kv = p.Split(new char[] { '=' }, 2); // 🔹 최대 2개까지만 split
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


}
