using UnityEngine;

public class CameraDepth : MonoBehaviour
{
    private void Update()
    {
        // Aspect ratio of the screen
        float ratio = (float)Screen.width / Screen.height;

        // Depth of the camera based on an online quadratic regression with some sample points
        float depth = -79.38451f + 50.33977f * ratio - 10.59345f * ratio * ratio;

        // Update the depth of the camera
        transform.position = new Vector3(transform.position.x, transform.position.y, depth);
    }
}
