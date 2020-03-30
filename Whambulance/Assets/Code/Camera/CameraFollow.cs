using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 velocity = Vector3.zero;

    [Range(0.01f, 0.99f)]
    public float smoothing;

    private void LateUpdate()
    {
        if (Player.Instance)
        {
            Vector3 newCamPos = Player.Instance.transform.position;
            newCamPos.z = transform.position.z;

            //too far away so just teleport the cam
            if (Vector2.SqrMagnitude(newCamPos - transform.position) > 3f * 3f)
            {
                transform.position = newCamPos;
            }

            //go towards new cam pos
            transform.position = Vector3.SmoothDamp(transform.position, newCamPos, ref velocity, smoothing);
        }
    }
}
