using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    // Velocidades y estado (mantenidas para la máquina de estados existente)
    public float currentMoveSpeed;
    public float walkSpeed = 3, walkBackSpeed = 2;
    public float runSpeed = 7, runBackSpeed = 5;
    public float crouchSpeed = 2, crouchBackSpeed = 5;

    [HideInInspector] public Vector3 dir;
    [HideInInspector] public float hzInput, vInput;
    private CharacterController controller;

    // Ajustes serializables (adaptados del nuevo código)
    [SerializeField] private float groundYOffset = 0.1f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity = -9.81f;

    // Variables internas de movimiento y gravedad
    private Vector3 velocity;
    private Vector3 movementDirection;
    private Vector3 spherePos;

    [HideInInspector] public Animator anim;

    MovementBaseState currentState;

    public IdleState Idle = new IdleState();
    public WalkState Walk = new WalkState();
    public CrouchState Crouch = new CrouchState();
    public RunState Run = new RunState();

    private void Awake()
    {
        // Cachear CharacterController y validar
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("CharacterController component is missing!");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        SwitchState(Idle);
    }

    void Update()
    {
        // Obtener dirección de movimiento (rellena hzInput, vInput y dir)
        GetDirection();

        // Aplicar gravedad
        ApplyGravity();

        // Combinar movimiento y gravedad y mover al personaje
        // Se utiliza currentMoveSpeed (debería ser ajustado por los estados)
        Vector3 finalMove = movementDirection.normalized * currentMoveSpeed + velocity;
        controller.Move(finalMove * Time.deltaTime);

        // Actualizar estado y animaciones
        currentState?.UpdateState(this);
        anim.SetFloat("hzInput", hzInput);
        anim.SetFloat("vInput", vInput);
    }

    private void GetDirection()
    {
       
        hzInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");

        // Dirección relativa al transform
        dir = transform.forward * vInput + transform.right * hzInput;
        movementDirection = dir;
    }

    private bool IsGrounded()
    {
        spherePos = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        return Physics.CheckSphere(spherePos, controller.radius - 0.05f, groundMask);
    }

    private void ApplyGravity()
    {
        if (IsGrounded())
        {
            // Mantener un pequeño valor negativo para asegurar contacto con el suelo
            if (velocity.y < 0f)
                velocity.y = -2f;
        }
        else
        {
            // Aplicar gravedad cuando no está en el suelo
            velocity.y += gravity * Time.deltaTime;
        }
    }

    public void SwitchState(MovementBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    private void OnDrawGizmos()
    {
        if (controller == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(spherePos, controller.radius - 0.05f);
    }
}
