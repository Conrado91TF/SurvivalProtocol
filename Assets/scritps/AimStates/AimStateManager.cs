using UnityEngine;
// El nuevo namespace obligatorio
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class AimStateManager : MonoBehaviour
{
    AimBaseState currentState;
    public HipFireState Hip = new HipFireState();
    public AimState Aim = new AimState();

    [SerializeField] private float mouseSensitivity = 1f; // antes: mouseSense
    [SerializeField] private Transform camFollowPos;
    private float xAxis, yAxis;

    // Input System
    private PlayerInput playerInput;
    private InputAction lookAction;

    [HideInInspector] public Animator anim;
    [HideInInspector] public CinemachineCamera vCam;
    public float adsFov = 40;
    [HideInInspector] public float hipFov;
    [HideInInspector] public float currentFov;
    public float fovTransitionSpeed = 10;

    public Transform aimPos;
    [HideInInspector] public Vector3 actulAimPos;
    [SerializeField] float aimSoothSpeed = 20;
    [SerializeField] LayerMask aimMask;

    private void Awake()
    {
        // Obtener PlayerInput si existe en este GameObject
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            // Se asume que el mapa de acciones contiene una acción llamada "Look"
            lookAction = playerInput.actions["Look"];
        }
    }

    void Start()
    {
        // Habilitar la acción de look si está presente
        lookAction?.Enable();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        vCam = GetComponentInChildren<CinemachineCamera>();
        if (vCam != null)
        {
            hipFov = vCam.Lens.FieldOfView;
            currentFov = hipFov;
        }

        anim = GetComponent<Animator>();
        SwitchState(Hip);
    }

    private void OnDisable()
    {
        lookAction?.Disable();

        // Restaurar cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // Obtener entrada con el New Input System si está disponible
        if (lookAction != null)
        {
            Vector2 lookDelta = lookAction.ReadValue<Vector2>();
            xAxis += lookDelta.x * mouseSensitivity;
            yAxis -= lookDelta.y * mouseSensitivity;
        }
        else
        {
            // Fallback al Input clásico (por compatibilidad)
            xAxis += Input.GetAxis("Mouse X") * mouseSensitivity;
            yAxis -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        }

        // Limitamos la rotación vertical para que no dé la vuelta completa
        yAxis = Mathf.Clamp(yAxis, -80f, 80f);

        if (vCam != null)
            vCam.Lens.FieldOfView = Mathf.Lerp(vCam.Lens.FieldOfView, currentFov, fovTransitionSpeed * Time.deltaTime);

        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Camera cam = Camera.main;
        if (cam != null)
        {
            Ray ray = cam.ScreenPointToRay(screenCenter);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask))
            {
                aimPos.position = Vector3.Lerp(aimPos.position, hit.point, aimSoothSpeed * Time.deltaTime);
                actulAimPos = aimPos.position;
            }
        }

        currentState?.UpdateState(this);
    }

    private void LateUpdate()
    {
        if (camFollowPos != null)
        {
            // Aplicamos la rotación a la posición que sigue la cámara
            camFollowPos.localEulerAngles = new Vector3(yAxis, camFollowPos.localEulerAngles.y, camFollowPos.localEulerAngles.z);
        }

        // Aplicamos la rotación horizontal al jugador (para que gire con la cámara)
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis, transform.eulerAngles.z);
    }

    public void SwitchState(AimBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
}