using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float sensitivity = 2.0f; // Mouse sensitivity.
    public float maxYRotation = 90.0f; // Maximum vertical rotation angle.
    public float moveSpeed = 5.0f; // Camera movement speed.

    public GameObject UIElement;
    private bool isObjectEnabled = true;

    private bool rotationLocked = true;
    private float rotationX = 0;
    private float rotationY = 0;

    void Start()
    {
        // Lock the cursor and hide it.
        //Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        
    }

    void LateUpdate()
    {
        if (!rotationLocked)
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
            Camera.main.transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        }
        

        // Get the scroll wheel input.
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");

        // Move the camera forward/backward.
        Vector3 moveDirection = transform.forward * scrollWheel * moveSpeed;
        transform.Translate(moveDirection, Space.World);

        //Get the Horizontal and Vertical Input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        //Pan the camera
        moveDirection = transform.up*vertical*moveSpeed + transform.right*horizontal*moveSpeed;
        transform.Translate(moveDirection*Time.deltaTime, Space.World);

        //Close and Open Menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isObjectEnabled = !isObjectEnabled;
            UIElement.SetActive(isObjectEnabled);

            if(isObjectEnabled)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                rotationLocked = true;
            } else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                rotationLocked = false;
            }
        }
    }
}
