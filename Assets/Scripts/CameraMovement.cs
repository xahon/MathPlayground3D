using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private new Camera camera;

    [SerializeField]
    private MainUi mainUi;

    [SerializeField]
    private float movementSpeed = 1.0f;

    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    private void Awake()
    {
        defaultPosition = transform.position;
        defaultRotation = transform.rotation;
    }

    void Update()
    {
        if (!Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            mainUi.EnableCodeInput();
            return;
        }

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        mainUi.DisableCodeInput();

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("UpDown");
        float moveZ = Input.GetAxis("Vertical");

        Quaternion rotationYaw = Quaternion.AngleAxis(mouseX, Vector3.up);
        Quaternion rotationPitch = Quaternion.AngleAxis(-mouseY, Vector3.right);

        transform.rotation *= rotationYaw;
        camera.transform.rotation *= rotationPitch;

        transform.Translate(new Vector3(moveX, moveY, moveZ) * movementSpeed * Time.deltaTime, Space.Self);
    }

    public void ResetView()
    {
        transform.position = defaultPosition;
        transform.rotation = defaultRotation;
        camera.transform.rotation = Quaternion.identity;
    }
}
