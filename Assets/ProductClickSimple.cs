using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Versao SEM Canvas. Coloque em UM objeto vazio da cena (ex.: "GerenteDeCliques").
// Nao precisa arrastar nada no Inspector.
public class ProductClickSimple : MonoBehaviour
{
    public Camera cameraAR;

    [Header("Rotacao ao clicar na boneca")]
    public float velocidadeGiro = 60f;   // graus por segundo

    ProductInfo atual;
    Rect rectPainel;

    // Para girar e depois devolver a boneca ao lugar original
    Transform girando;
    Vector3 posOriginal;
    Quaternion rotOriginal;
    Vector3 centroLocal;   // centro da boneca no espaco da loja (acompanha o alvo AR)
    Transform raizLoja;

    void Start()
    {
        if (cameraAR == null) cameraAR = Camera.main;
        if (cameraAR == null) cameraAR = FindFirstObjectByType<Camera>();

        GameObject loja = GameObject.Find("loja_bonecas");
        if (loja != null) raizLoja = loja.transform;

        // Garante que cada boneca tem collider para receber o clique.
        foreach (var info in FindObjectsByType<ProductInfo>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            foreach (var mf in info.GetComponentsInChildren<MeshFilter>(true))
            {
                if (mf.GetComponent<Collider>() == null)
                    mf.gameObject.AddComponent<MeshCollider>();
            }
        }
    }

    void Update()
    {
        // Gira a boneca selecionada (nao gira o funcionario)
        if (girando != null)
        {
            Vector3 eixo = raizLoja != null ? raizLoja.up : Vector3.up;
            Vector3 centro = raizLoja != null ? raizLoja.TransformPoint(centroLocal) : centroLocal;
            girando.RotateAround(centro, eixo, velocidadeGiro * Time.deltaTime);
        }

        if (!TryGetTap(out Vector2 pos)) return;

        // Toque dentro do painel aberto: deixa o botao "Fechar" tratar.
        Vector2 guiPos = new Vector2(pos.x, Screen.height - pos.y);
        if (atual != null && rectPainel.Contains(guiPos)) return;

        Ray ray = cameraAR.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            ProductInfo info = hit.collider.GetComponentInParent<ProductInfo>();
            if (info != null) { Selecionar(info); return; }
        }
        Selecionar(null);
    }

    void Selecionar(ProductInfo info)
    {
        // Devolve a boneca anterior ao lugar original
        if (girando != null)
        {
            girando.localPosition = posOriginal;
            girando.localRotation = rotOriginal;
            girando = null;
        }

        atual = info;
        if (info == null || info.isFuncionario) return;

        // Centro visual da boneca (a origem do modelo pode estar longe dela)
        var rends = info.GetComponentsInChildren<Renderer>();
        if (rends.Length == 0) return;
        Bounds b = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);

        girando = info.transform;
        posOriginal = girando.localPosition;
        rotOriginal = girando.localRotation;
        centroLocal = raizLoja != null ? raizLoja.InverseTransformPoint(b.center) : b.center;
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

    void OnGUI()
    {
        if (atual == null) return;

        float w = Screen.width;
        float h = Screen.height;
        int fonte = Mathf.Max(16, (int)(h * 0.03f));

        rectPainel = new Rect(w * 0.04f, h * 0.60f, w * 0.92f, h * 0.36f);

        GUIStyle box = new GUIStyle(GUI.skin.box);
        box.fontSize = fonte;
        box.alignment = TextAnchor.UpperLeft;
        box.wordWrap = true;
        box.richText = true;
        box.normal.textColor = Color.white;
        box.padding = new RectOffset(24, 24, 20, (int)(h * 0.09f));

        string texto = "<b>" + atual.nome + "</b>\n";
        if (atual.isFuncionario)
        {
            texto += "Funcionario da loja\n\n" + atual.descricao;
        }
        else
        {
            texto += "Preco: " + atual.preco + "\n";
            texto += "Em estoque: " + atual.estoque + (atual.estoque == 1 ? " unidade" : " unidades") + "\n\n";
            texto += atual.descricao;
        }

        GUI.Box(rectPainel, texto, box);

        GUIStyle btn = new GUIStyle(GUI.skin.button);
        btn.fontSize = fonte;
        Rect rBtn = new Rect(rectPainel.xMax - w * 0.30f, rectPainel.yMax - h * 0.075f, w * 0.26f, h * 0.06f);
        if (GUI.Button(rBtn, "Fechar", btn)) Selecionar(null);
    }
}
