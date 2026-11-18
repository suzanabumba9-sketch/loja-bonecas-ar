using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Ao clicar nesta boneca (ou num filho), ela gira sobre si mesma durante "duracao" segundos.
public class GirarAoClicar : MonoBehaviour
{
    public float velocidade = 180f; // graus por segundo
    public float duracao = 2f;      // segundos

    float restante;
    Camera cam;

    void Update()
    {
        if (restante > 0f)
        {
            float passo = Mathf.Min(Time.deltaTime, restante);
            transform.Rotate(0f, velocidade * passo, 0f, Space.Self);
            restante -= passo;
        }

        if (!CliqueNesteFrame(out Vector2 pos)) return;

        Camera c = ObterCamera();
        if (c == null) return;

        Ray ray = c.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
                restante = duracao;
        }
    }

    Camera ObterCamera()
    {
        if (Camera.main != null) return Camera.main;
        if (cam == null) cam = FindFirstObjectByType<Camera>(); // camara do ARCamera (Vuforia)
        return cam;
    }

    bool CliqueNesteFrame(out Vector2 pos)
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            pos = Mouse.current.position.ReadValue();
            return true;
        }
#else
        if (Input.GetMouseButtonDown(0))
        {
            pos = Input.mousePosition;
            return true;
        }
#endif
        pos = Vector2.zero;
        return false;
    }
}
