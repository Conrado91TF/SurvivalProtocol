using UnityEngine;
// El nuevo namespace obligatorio
using Unity.Cinemachine;

public class AimStateManager : MonoBehaviour
{
    AimBaseState currentState;
    public HipFireState Hip = new HipFireState();
    public AimState Aim = new AimState();

    [SerializeField] float mouseSense = 1;
    [SerializeField] Transform camFollowPos;
    float xAxis, yAxis;

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


    void Start()
    {
        vCam = GetComponentInChildren<CinemachineCamera>();
            hipFov = vCam.Lens.FieldOfView;
            currentFov = hipFov;
        anim = GetComponent<Animator>();
        SwitchState(Hip);
    }
    void Update()
    {
        // Actualizamos los valores de los ejes con la entrada del ratón
        xAxis += Input.GetAxis("Mouse X") * mouseSense;
        yAxis -= Input.GetAxis("Mouse Y") * mouseSense;
        // Limitamos la rotación vertical para que no dé la vuelta completa
        yAxis = Mathf.Clamp(yAxis, -80, 80);

        vCam.Lens.FieldOfView = Mathf.Lerp(vCam.Lens.FieldOfView, currentFov, fovTransitionSpeed * Time.deltaTime);

        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask))
            aimPos.position = Vector3.Lerp(aimPos.position,hit.point, aimSoothSpeed * Time.deltaTime);
        
        currentState.UpdateState(this);
    }

    private void LateUpdate()
    {
        // Aplicamos la rotación a la posición que sigue la cámara
        camFollowPos.localEulerAngles = new Vector3(yAxis, camFollowPos.localEulerAngles.y, camFollowPos.localEulerAngles.z);

        // Aplicamos la rotación horizontal al jugador (para que gire con la cámara)
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis, transform.eulerAngles.z);
    }
    public void SwitchState(AimBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

}