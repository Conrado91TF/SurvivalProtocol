using UnityEngine;
// El nuevo namespace obligatorio
using Unity.Cinemachine;

public class AimStateManager : MonoBehaviour
{
    [SerializeField] float mouseSense = 1;
    float xAxis, yAxis;
    [SerializeField] Transform camFollowPos;

    void Update()
    {
        // Actualizamos los valores de los ejes con la entrada del ratón
        xAxis += Input.GetAxis("Mouse X") * mouseSense;
        yAxis -= Input.GetAxis("Mouse Y") * mouseSense;
        // Limitamos la rotación vertical para que no dé la vuelta completa
        yAxis = Mathf.Clamp(yAxis, -80, 80);

    }

    private void LateUpdate()
    {
        // Aplicamos la rotación a la posición que sigue la cámara
        camFollowPos.localEulerAngles = new Vector3(yAxis, camFollowPos.localEulerAngles.y, camFollowPos.localEulerAngles.z);

        // Aplicamos la rotación horizontal al jugador (para que gire con la cámara)
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis, transform.eulerAngles.z);
    }
}