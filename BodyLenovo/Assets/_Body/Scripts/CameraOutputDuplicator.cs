using UnityEngine;

public class CameraOutputDuplicator : MonoBehaviour
{
    public RenderTexture copyTexture;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        // Copiar a la RenderTexture
        if (copyTexture != null)
            Graphics.Blit(src, copyTexture);

        // Continuar renderizando a la pantalla
        Graphics.Blit(src, dest);
    }
}