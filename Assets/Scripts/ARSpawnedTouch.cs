using UnityEngine;

public class ARSpawnedTouch : MonoBehaviour
{
    public string targetUrl;

    void Update()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == this.transform)
                {
                    Application.OpenURL(targetUrl);
                }
            }
        }
    }
}
