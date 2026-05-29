using UnityEngine;
using UnityEngine.InputSystem;

public class DualShooter : MonoBehaviour
{
    [Header("Configurações Gerais")]
    public float forcaDoTiro = 20f;

    [Header("Mostarda (Botão Esquerdo)")]
    public Transform mustardFirePoint;
    public GameObject balaMostardaPrefab;

    [Header("Ketchup (Botão Direito)")]
    public Transform ketchupFirePoint;
    public GameObject balaKetchupPrefab;

    void Update()
    {
        // Prevenção de erro caso o mouse não seja detectado
        if (Mouse.current == null) return;

        // Atira com a Mostarda (Esquerdo) usando o prefab amarelo
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Atirar(mustardFirePoint, balaMostardaPrefab);
        }

        // Atira com o Ketchup (Direito) usando o prefab vermelho
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Atirar(ketchupFirePoint, balaKetchupPrefab);
        }
    }

    void Atirar(Transform pontoDeDisparo, GameObject prefabDaBala)
    {
        // Cria o prefab específico que foi passado para a função
        GameObject bala = Instantiate(prefabDaBala, pontoDeDisparo.position, pontoDeDisparo.rotation);

        Rigidbody rb = bala.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(pontoDeDisparo.forward * forcaDoTiro, ForceMode.Impulse);
        }
    }
}