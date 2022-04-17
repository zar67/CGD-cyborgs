using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera m_targetCamera = default;
    [SerializeField] private Transform m_targetCameraTransform = default;

    [Header("Camera Movement Values")]
    [SerializeField] private float m_defaultCameraSpeed = 5.0f;
    [SerializeField] private float m_fasterCameraSpeed = 10.0f;

    [Header("Camer Zoom Values")]
    [SerializeField] private float m_defaultZoomSpeed = 1.0f;
    [SerializeField] private float m_fasterZoomSpeed = 2.0f;
    [SerializeField] private float m_minSizeValue = 5.0f;
    [SerializeField] private float m_maxSizeValue = 15.0f;

    private Vector2 m_movementInputValue = Vector2.zero;
    private bool m_fastInputValue = false;
    private float m_zoomInputValue = 0;

    private Rect m_worldRect = new Rect(0, 0, 20, 10);

    public void SetCameraPosition(Vector2 position)
    {
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }

    public void SetWorldRect(Rect rect)
    {
        m_worldRect = rect;
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        m_movementInputValue = context.ReadValue<Vector2>();
    }

    public void OnFasterInput(InputAction.CallbackContext context)
    {
        m_fastInputValue = context.performed;
    }

    public void OnZoomInput(InputAction.CallbackContext context)
    {
        m_zoomInputValue = context.ReadValue<float>();
    }

    private void Update()
    {
        Move();
        Zoom();
    }

    private void Move()
    {
        float movementSpeed = m_fastInputValue ? m_fasterCameraSpeed : m_defaultCameraSpeed;

        Vector3 cameraPosition = m_targetCameraTransform.position;
        cameraPosition.x += m_movementInputValue.x * movementSpeed * Time.deltaTime;
        cameraPosition.y += m_movementInputValue.y * movementSpeed * Time.deltaTime;

        cameraPosition.x = Mathf.Clamp(cameraPosition.x, m_worldRect.min.x, m_worldRect.max.x);
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, m_worldRect.min.y, m_worldRect.max.y);

        m_targetCameraTransform.position = cameraPosition;
    }

    private void Zoom()
    {
        float zoomSpeed = m_fastInputValue ? m_fasterZoomSpeed : m_defaultZoomSpeed;

        float cameraSize = m_targetCamera.orthographicSize;
        cameraSize += m_zoomInputValue * zoomSpeed * Time.deltaTime;

        cameraSize = Mathf.Clamp(cameraSize, m_minSizeValue, m_maxSizeValue);
        m_targetCamera.orthographicSize = cameraSize;
    }
}