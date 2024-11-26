using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public Transform target;
    public float distance = 50f;
    public float height = 10f;
    public float rotationSpeed = 100f;
    public float minY = 5f;
    public float maxY = 80f;
    private float yaw;
    private float pitch;
    public float zoomSpeed = 2f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0.0f);
        Vector3 position = rotation * new Vector3(0f, 0f, -distance) + target.position + Vector3.up * height;

        transform.rotation = rotation;
        transform.position = position;

        if (Input.GetKey(KeyCode.Mouse1))
        {
            distance = Mathf.Clamp(distance-50f*Time.deltaTime*zoomSpeed,0f,50f);
            height = Mathf.Clamp(height+7f*Time.deltaTime*zoomSpeed,7f,14f);
        }
        else{
            distance = Mathf.Clamp(distance+50f*Time.deltaTime*zoomSpeed,0f,50f);
            height = Mathf.Clamp(height-7f*Time.deltaTime*zoomSpeed,7f,14f);
        }
    }
}