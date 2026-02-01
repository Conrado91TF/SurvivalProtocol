using UnityEngine;
// El nuevo namespace obligatorio
using Unity.Cinemachine;

public class AimStateManager : MonoBehaviour
{
    // En la nueva versión, AxisState se encuentra en Unity.Cinemachine
    public InputAxis xAxis = new InputAxis { Range = new Vector2(-180, 180), Wrap = true };
    public InputAxis yAxis = new InputAxis { Range = new Vector2(-70, 70) };
    [SerializeField] Transform camFollowPos;

    void Update()
    {
        // Actualizamos los valores de los ejes con la entrada del ratón
        xAxis.Value += Input.GetAxis("Mouse X");
        yAxis.Value -= Input.GetAxis("Mouse Y");
        // Limitamos el valor del eje Y para evitar rotaciones excesivas
        yAxis.Value = Mathf.Clamp(yAxis.Value, yAxis.Range.x, yAxis.Range.y);
    }

    private void LateUpdate()
    {
        // Aplicamos la rotación a la posición que sigue la cámara
        camFollowPos.localEulerAngles = new Vector3(yAxis.Value, camFollowPos.localEulerAngles.y, camFollowPos.localEulerAngles.z);

        // Aplicamos la rotación horizontal al jugador (para que gire con la cámara)
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, xAxis.Value, transform.eulerAngles.z);
    }
}