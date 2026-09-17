using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CinemachineZoom : MonoBehaviour
{
    [Header("Настройки")]
    public InputActionAsset inputActions;
    public CinemachineOrbitalFollow orbitalFollow;
    public float zoomSensitivity = 2f;
    public float minRadius = 2f;
    public float maxRadius = 15f;

    private InputAction zoomAction;

    void Awake()
    {
        if (inputActions != null)
        {
            zoomAction = inputActions.FindAction("Zoom");
        }
    }

    void OnEnable()
    {
        if (zoomAction != null) zoomAction.Enable();
    }

    void OnDisable()
    {
        if (zoomAction != null) zoomAction.Disable();
    }

    void Update()
    {
        if (orbitalFollow == null) return;
        if (zoomAction == null) return;

        float scrollInput = zoomAction.ReadValue<float>();

        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            float newRadius = orbitalFollow.Radius - scrollInput * zoomSensitivity;
            orbitalFollow.Radius = Mathf.Clamp(newRadius, minRadius, maxRadius);
        }
    }
}