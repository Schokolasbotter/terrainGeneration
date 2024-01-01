using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float sensitivity = 2.0f; // Mouse sensitivity.
    public float maxYRotation = 90.0f; // Maximum vertical rotation angle.
    public float moveSpeed = 5.0f; // Camera movement speed.

    private float rotationX = 0;
    private float rotationY = 0;

    void Start()
    {
        // Lock the cursor and hide it.
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    void LateUpdate()
    {
        // Get the mouse input.
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Rotate the camera horizontally.
        Camera.main.transform.Rotate(0, mouseX, 0);

        // Calculate the vertical rotation angle.
        rotationX -= mouseY;
        rotationY += mouseX;
        rotationX = Mathf.Clamp(rotationX, -maxYRotation, maxYRotation);

        // Rotate the camera vertically.
        Camera.main.transform.rotation = Quaternion.Euler(rotationX,rotationY, 0);

        // Get the scroll wheel input.
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

        // Move the camera forward/backward.
        Vector3 moveDirection = transform.forward * scrollWheel * moveSpeed;
        transform.Translate(moveDirection, Space.World);

        //Get Arrow Key Input
        Vector2 horizontal;
        horizontal.x = Input.GetAxis("Horizontal");
        horizontal.y = Input.GetAxis("Vertical");
        //Move Camera in Space
        Vector3 direction = transform.forward * horizontal.y * moveSpeed + transform.right * horizontal.x * moveSpeed;
        transform.Translate(moveDirection, Space.World);
    }
}
