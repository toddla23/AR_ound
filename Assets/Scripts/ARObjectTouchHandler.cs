using UnityEngine;

public class ARObjectTouchHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    string url = PlayerPrefs.GetString("url", "");
                    if (!string.IsNullOrEmpty(url))
                    {
                        Application.OpenURL(url);
                    }
                    else
                    {
                        Debug.LogWarning("⚠️ No URL found in PlayerPrefs.");
                    }
                }
            }
        }
    }
}
