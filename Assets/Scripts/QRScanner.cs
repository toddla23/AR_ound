using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using ZXing;
using Unity.Collections;
using TMPro;
using UnityEngine.Networking;

// Prefab=ObjectA;Text=http://34.134.87.58:8080/discriptions/1;URL=http://qweasd/option/1
public class QRScanner : MonoBehaviour
{
    [SerializeField] private ARCameraManager arCameraManager;
    [SerializeField] private TextMeshProUGUI toastText;    // 기존
    [SerializeField] private TextMeshProUGUI toastText2;   // ✅ 새로 추가

    private Texture2D cameraTexture;
    private bool isProcessing = false;
    private bool isScanned = false;

    private void OnEnable()
    {
        if (arCameraManager != null)
            arCameraManager.frameReceived += OnCameraFrameReceived;

        HideToast();
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
                Debug.Log($"QR Detected: {result.Text}");
                ParseQRData(result.Text);
                isScanned = true;

                if (PlayerPrefs.HasKey("Text"))
                {
                    StartCoroutine(RequestDescription(PlayerPrefs.GetString("Text")));
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"QR Decode Error: {e.Message}");
        }

        yield return new WaitForSeconds(0.2f);
        isProcessing = false;
    }

    private void ParseQRData(string data)
    {
        var pairs = data.Split(';');
        foreach (var p in pairs)
        {
            var kv = p.Split(new char[] { '=' }, 2);
            if (kv.Length == 2)
            {
                string key = kv[0].Trim();
                string value = kv[1].Trim();

                PlayerPrefs.SetString(key, value);
                Debug.Log($"Saved {key} = {value}");
            }
        }
        PlayerPrefs.Save();
    }

    private IEnumerator RequestDescription(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            ShowToast($"<mark=#ff4444AA>Error:</mark>\n{request.error}");
        }
        else
        {
            string desc = request.downloadHandler.text;


            ShowToast(desc);
        }
    }

    // ✅ toastText, toastText2 둘 다 표시
    private void ShowToast(string message)
    {
        if (toastText != null)
        {
            toastText.text = $"{message}";
            toastText.gameObject.SetActive(true);
        }

        if (toastText2 != null)
        {
            toastText2.text = $"<mark=#d0ee17AA>{message}</mark>";
            toastText2.gameObject.SetActive(true);
        }

        StartCoroutine(HideToastAfterDelay(20f));
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

        if (toastText2 != null)
            toastText2.gameObject.SetActive(false);
    }
}
