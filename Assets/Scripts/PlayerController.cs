using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform playerTransform;
    public Transform cameraTransform;
    public CharacterController character;

    public float moveSensitivity;
    public float lookSensitivity;
    public float maxLookY;
    public float minLookY;

    public float jumpPower;
    public float gravity;
    private float velocityY = 0;

    private float currentLookY;
    public bool controlEnabled;

    public void EnableControl(bool enable)
    {
        if (enable) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
        Cursor.visible = !enable;
        controlEnabled = enable;
    }
    void Update()
    {
        if (controlEnabled)
        {
            float lookX = Input.GetAxis("Mouse X") * Time.deltaTime * lookSensitivity;
            float lookY = Input.GetAxis("Mouse Y") * Time.deltaTime * lookSensitivity;
            currentLookY += lookY;
            cameraTransform.localRotation = Quaternion.Euler(
                Vector3.right * Mathf.Clamp(currentLookY, minLookY, maxLookY));
            playerTransform.Rotate(playerTransform.up * lookX);
        }

        float moveX = controlEnabled ?
            Input.GetAxis("Horizontal") * Time.deltaTime * moveSensitivity : 0;
        float moveZ = controlEnabled ?
            Input.GetAxis("Vertical") * Time.deltaTime * moveSensitivity : 0;
        if (character.isGrounded && controlEnabled)
        {
            velocityY = Input.GetKeyDown("space") ? jumpPower : 0;
        }
        velocityY += gravity * Time.deltaTime;
        character.Move(
            playerTransform.right * moveX
            + playerTransform.up * velocityY
            + playerTransform.forward * moveZ);
    }
}
