using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    public float walkSpeed = 3f;
    [SerializeField] public Vector3 dir;
    float hzinpunt, vInpunt;
    CharacterController controller;

    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    Vector3 spherePos;

    [SerializeField] float gravity = -9.81f;
    Vector3 velocity;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        GetDirectioAndMove();
        Gravity();
    }
    void GetDirectioAndMove()
    { 
      hzinpunt = Input.GetAxis("Horizontal");
      vInpunt = Input.GetAxis("Vertical");

      dir = transform.forward * vInpunt + transform.right * hzinpunt;

      controller.Move(dir * walkSpeed * Time.deltaTime);
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
