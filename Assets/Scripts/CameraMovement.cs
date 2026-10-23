using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    Camera cam;
    public float moveSpeed;
    private Vector2 axis;
    public float scrollSpeed;
    private float zoom;
    public Vector2 scrollLimit;
    public bool mouseMovement;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    public void Zoom(InputAction.CallbackContext ctx)
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        if (ctx.performed)
        {
            zoom = ctx.ReadValue<Vector2>().y;
            cam.orthographicSize += zoom * scrollSpeed * Time.deltaTime;
            if (cam.orthographicSize < scrollLimit.x) cam.orthographicSize = scrollLimit.x;
            if (cam.orthographicSize > scrollLimit.y) cam.orthographicSize = scrollLimit.y;
        }
    }

    public void Movement(InputAction.CallbackContext ctx)
    {
        axis = ctx.ReadValue<Vector2>();
    }

    public void ToogleMovement(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) mouseMovement = true;
        if (ctx.canceled) mouseMovement = false;
        axis = Vector2.zero;
    }

    public void MouseMovement(InputAction.CallbackContext ctx)
    {
        if (mouseMovement == true) axis = ctx.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 dir = axis * (moveSpeed * Time.fixedDeltaTime);
        if (axis.sqrMagnitude != 0)
            transform.position = Vector3.MoveTowards(transform.position, transform.position + dir,
                moveSpeed * Time.fixedDeltaTime);
    }
}