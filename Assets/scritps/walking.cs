using UnityEngine;
using UnityEngine.AI;

public class walking : MonoBehaviour
{
    [SerializeField]
    NavMeshAgent agent;
    [SerializeField]
    Transform targetPosition;
    [SerializeField]
    Animation animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animation>();
        if (animator == null)
        {
            Debug.LogError("Animation component not found on " + gameObject.name, gameObject);
        }
        agent.SetDestination(targetPosition.position);
    }

    // Update is called once per frame
    void Update()
    {
        //Distacia a destino
        //agent.remainingDistance
        //Velocidad actual
        //agent.velocity
        // animator.setfloat(walking, agent.velocity.magnitude);
        // aqui tiene que poner el nombre que este puesto en el animator con este codigo de arriba
    }
}
