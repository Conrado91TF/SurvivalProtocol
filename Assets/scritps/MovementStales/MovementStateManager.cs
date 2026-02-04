using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    public float currentMoveSpeed;
    public float walkSpeed = 3, walkBackSpeed = 2;
    public float runSpeed = 7, runBackSpeed = 5;
    public float crouchSpeed = 2, crouchBackSpeed = 5;

    [HideInInspector] public Vector3 dir;
    [HideInInspector] public float hzInput, vInput;
    CharacterController controller;

    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    Vector3 spherePos;

    [SerializeField] float gravity = -9.81f;
    Vector3 velocity;

    [HideInInspector] public Animator anim;

    MovementBaseState currentState;

    public  IdleState Idle = new IdleState();
    public  WalkState Walk = new WalkState();
    public CrouchState Crouch = new CrouchState();
    public RunState Run = new RunState();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       controller = GetComponent<CharacterController>();
       anim = GetComponentInChildren<Animator>();
       SwitchState(Idle);
    }

    // Update is called once per frame
    void Update()
    {
        GetDirectioAndMove();
        Gravity();
        currentState.UpdateState(this);

        anim.SetFloat("hzInput", hzInput);
        anim.SetFloat("vInput", vInput);

    }
    public void SwitchState(MovementBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
        
    }
    void GetDirectioAndMove()
    { 
      hzInput = Input.GetAxis("Horizontal");
      vInput = Input.GetAxis("Vertical");

      dir = transform.forward * vInput + transform.right * hzInput;

      controller.Move(dir.normalized * currentMoveSpeed * Time.deltaTime);
    }
    
    bool IsGrounded()
    {
        spherePos = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        if (Physics.CheckSphere(spherePos, controller.radius -0.05f, groundMask)) return true;
        return false;
    }
    void Gravity()
    {
        if (IsGrounded()) velocity.y += gravity * Time.deltaTime;
        else if (velocity.y < 0) velocity.y = -2;

        controller.Move(velocity * Time.deltaTime);
    }
    private void OnDrawGizmos()
    {
        if (controller == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(spherePos, controller.radius - 0.05f);
    }
}
