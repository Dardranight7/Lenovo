using UnityEngine;

public class TestingCamera : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float verticalSpeed = 5f;

    public string PlayerPrefX = "CameraPosX";
    public string PlayerPrefY = "CameraPosY";
    public string PlayerPrefZ = "CameraPosZ";

    public AvatarController avatarController; // Referencia al AvatarController

    void OnEnable()
    {
        // Cargar posición guardada si existe
        if (PlayerPrefs.HasKey(PlayerPrefX))
        {
            float x = PlayerPrefs.GetFloat(PlayerPrefX);
            float y = transform.position.y; // Mantener la altura actual
            float z = PlayerPrefs.GetFloat(PlayerPrefZ);

            transform.position = new Vector3(x, y, z);
            avatarController.verticalOffset = PlayerPrefs.GetFloat(PlayerPrefY, avatarController.verticalOffset); // Cargar offset vertical
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
            avatarController.verticalOffset += 1f * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            avatarController.verticalOffset -= 1f * Time.deltaTime;

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
            PlayerPrefs.SetFloat(PlayerPrefY, avatarController.verticalOffset);
            PlayerPrefs.Save();
        }
    }
}
