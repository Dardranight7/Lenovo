using UnityEngine;

public class MultiDisplaySetup : MonoBehaviour
{
    void Start()
    {
        // Activar el display 2 si existe
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
        }

        // --- Display principal (0) ---
        // Lo ponemos en vertical (1080 x 1920) en modo ventana
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.SetResolution(1920, 1080, false);

        // --- Display secundario (1) ---
        if (Display.displays.Length > 1)
        {
            // SetParams(width, height, x, y)
            // Renderiza a 1920x1080 en modo ventana
            Display.displays[1].SetParams(1920, 1080, 0, 0);
        }

        Debug.Log("✅ Displays configurados en modo ventana (puedes moverlos manualmente).");
    }
}