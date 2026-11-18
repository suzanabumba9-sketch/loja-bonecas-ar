using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Coloque este script em UM objeto vazio da cena (ex.: "GerenteDeCliques").
// Arraste o painel e os 4 textos nos campos do Inspector.
public class ProductClickManager : MonoBehaviour
{
    [Header("Interface")]
    public GameObject painel;
    public TMP_Text textoNome;
    public TMP_Text textoPreco;
    public TMP_Text textoEstoque;
    public TMP_Text textoDescricao;

    [Header("Camera (deixe vazio para usar a AR Camera)")]
    public Camera cameraAR;

    void Start()
    {
        if (cameraAR == null) cameraAR = Camera.main;
        if (cameraAR == null) cameraAR = FindFirstObjectByType<Camera>();

        // Garante que cada boneca tem collider para receber o clique.
        foreach (var info in FindObjectsByType<ProductInfo>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            foreach (var mf in info.GetComponentsInChildren<MeshFilter>(true))
            {
                if (mf.GetComponent<Collider>() == null)
                    mf.gameObject.AddComponent<MeshCollider>();
            }
        }

        if (painel != null) painel.SetActive(false);
    }

    void Update()
    {
        if (!TryGetTap(out Vector2 pos)) return;

        // Se o toque foi sobre a interface (botao Fechar etc.), ignora o 3D.
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = cameraAR.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            ProductInfo info = hit.collider.GetComponentInParent<ProductInfo>();
            if (info != null) { Mostrar(info); return; }
        }
        Fechar();
    }

    bool TryGetTap(out Vector2 pos)
    {
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            pos = Touchscreen.current.primaryTouch.position.ReadValue();
            return true;
        }
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            pos = Mouse.current.position.ReadValue();
            return true;
        }
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            pos = Input.GetTouch(0).position;
            return true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            pos = Input.mousePosition;
            return true;
        }
#endif
        pos = Vector2.zero;
        return false;
    }

    void Mostrar(ProductInfo info)
    {
        if (painel == null) return;
        painel.SetActive(true);

        textoNome.text = info.nome;
        textoDescricao.text = info.descricao;

        if (info.isFuncionario)
        {
            textoPreco.text = "Funcionario da loja";
            textoEstoque.text = "";
        }
        else
        {
            textoPreco.text = "Preco: " + info.preco;
            textoEstoque.text = "Em estoque: " + info.estoque + (info.estoque == 1 ? " unidade" : " unidades");
        }
    }

    // Ligue este metodo ao botao "Fechar" (evento OnClick).
    public void Fechar()
    {
        if (painel != null) painel.SetActive(false);
    }
}
