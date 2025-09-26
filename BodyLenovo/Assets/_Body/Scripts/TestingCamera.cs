using UnityEngine;

public class TestingCamera : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float verticalSpeed = 5f;

    public string PlayerPrefX = "CameraPosX";
    public string PlayerPrefY = "CameraPosY";
    public string PlayerPrefZ = "CameraPosZ";


    void OnEnable()
    {
        // Cargar posición guardada si existe
        if (PlayerPrefs.HasKey(PlayerPrefX))
        {
            float x = PlayerPrefs.GetFloat(PlayerPrefX);
            float y = PlayerPrefs.GetFloat(PlayerPrefY); // Mantener la altura actual
            float z = PlayerPrefs.GetFloat(PlayerPrefZ);

            transform.position = new Vector3(x, y, z);
        }
    }

    void Update()
    {
        // Movimiento en plano XZ con Input Axis
        float moveX = Input.GetAxis("Horizontal"); // A-D o flechas
        float moveZ = Input.GetAxis("Vertical");   // W-S o flechas

        Vector3 move = new Vector3(moveX, 0f, moveZ);

        // Movimiento vertical con teclas
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            move.y += 1f;

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            move.y -= 1f;

        // Aplicar movimiento en espacio mundial (sin rotación)
        transform.position += new Vector3(
            move.x * moveSpeed * Time.deltaTime,
            move.y * verticalSpeed * Time.deltaTime,
            move.z * moveSpeed * Time.deltaTime
        );

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Guardar posición actual en PlayerPrefs
            PlayerPrefs.SetFloat(PlayerPrefX, transform.position.x);
            PlayerPrefs.SetFloat(PlayerPrefZ, transform.position.z);
            PlayerPrefs.SetFloat(PlayerPrefY, transform.position.y);
            PlayerPrefs.Save();
        }
    }
}
