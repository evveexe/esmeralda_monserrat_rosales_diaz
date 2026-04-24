using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleCamera : MonoBehaviour
{
    [Header("Sensibilidad")]
    public float sensX = 100f;
    public float sensY = 100f;

    [Header("Joystick Móvil")]
    public Joystick cameraJoystick; 
    public bool useMobileJoystick = false;

    [Header("Referencias")]
    public Transform playerBody;

    private float xRotation = 0f;
    private float yRotation = 0f;
    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        xRotation = transform.localRotation.eulerAngles.x;
        yRotation = playerBody.eulerAngles.y;
    }

    private void Update()
    {
        float mouseX = 0f;
        float mouseY = 0f;
                
        if (useMobileJoystick && cameraJoystick != null)
        {
            mouseX = cameraJoystick.axis.x * sensX * Time.deltaTime;
            mouseY = cameraJoystick.axis.y * sensY * Time.deltaTime;
          
        }
        else
        {            
            Vector2 lookInput = controls.Player.Look.ReadValue<Vector2>();
            mouseX = lookInput.x * sensX * Time.deltaTime;
            mouseY = lookInput.y * sensY * Time.deltaTime;
        }
                
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
                
        yRotation += mouseX;
                
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}

//Este es el controlador de camara que esta conectado al joystick, usa sensibilidad para el movimiento y el player para la orientacion