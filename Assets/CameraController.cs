using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public float scroll = 8;
    public Camera m_OrthographicCamera;

    public float minFOV;
    public float maxFOV;
    public float sensitivity;
    public float FOV;

    public int scrollSpeed = 1;

    private Rect m_worldRect = new Rect(0, 0, 20, 10);

    public void SetCameraPosition(Vector2 position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }

    public void SetWorldRect(Rect rect)
    {
        m_worldRect = rect;
        m_worldRect.x -= panBorderThickness;
        m_worldRect.y -= panBorderThickness;
        m_worldRect.width += panBorderThickness * 2;
        m_worldRect.height += panBorderThickness * 2;
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.y += panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            pos.y -= panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }


        FOV = Camera.main.fieldOfView;
        FOV += Input.GetAxis("Mouse ScrollWheel") * sensitivity * -1;
        FOV = Mathf.Clamp(FOV, minFOV, maxFOV);
        Camera.main.fieldOfView = FOV;

        pos.x = Mathf.Clamp(pos.x, m_worldRect.min.x, m_worldRect.max.x);
        pos.y = Mathf.Clamp(pos.y, m_worldRect.min.y, m_worldRect.max.y);

        transform.position = pos;
    }
}