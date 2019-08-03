using UnityEngine;

public class CameraDepth : MonoBehaviour
{
    private const float ASPECT_RATIO = 16f / 9f;

    private void Update()
    {
        // Aspect ratio of the screen
        float ratio = Mathf.Min((float)Screen.width / Screen.height, ASPECT_RATIO);

        // Orthographic size of the camera based on an online quadratic regression with some sample points
        GetComponent<Camera>().orthographicSize = 49.92087f - 34.0246f * ratio + 7.616427f * ratio * ratio;
    }
}
