using UnityEngine;

public class Target : MonoBehaviour
{
    [HideInInspector] public TargetSpawner.SpawnPoint spawnPoint;
    [HideInInspector] public bool moveHorizontal = false;
    [HideInInspector] public bool moveVertical = false;
    [HideInInspector] public float moveSpeed = 1f;
    [HideInInspector] public float moveRange = 5f;
    [HideInInspector] public int health = 1;
    [HideInInspector] public int pointsValue = 10;

    private Vector3 startPosition;
    private float directionX = 1f;
    private float directionY = 1f;
    private FPSAimController playerShooter;

    void Start()
    {
        startPosition = transform.position;
        playerShooter = FindAnyObjectByType<FPSAimController>();
    }

    void Update()
    {
        Vector3 newPos = transform.position;

        // Movimento Horizontal (Eixo X)
        if (moveHorizontal)
        {
            newPos.x += directionX * moveSpeed * Time.deltaTime;
            
            float offsetX = newPos.x - startPosition.x;

            // Se chegou no limite, inverte para voltar
            if (offsetX >= moveRange)
            {
                newPos.x = startPosition.x + moveRange; // Trava no limite exato
                directionX = -1f;
            }
            // Se voltou para o ponto de criação original, inverte para ir para frente
            else if (offsetX <= 0f)
            {
                newPos.x = startPosition.x; // Trava no zero exato
                directionX = 1f;
            }
        }

        // Movimento Vertical (Eixo Y) - Agora implementado!
        if (moveVertical)
        {
            newPos.y += directionY * moveSpeed * Time.deltaTime;
            
            float offsetY = newPos.y - startPosition.y;

            if (offsetY >= moveRange)
            {
                newPos.y = startPosition.y + moveRange;
                directionY = -1f;
            }
            else if (offsetY <= 0f)
            {
                newPos.y = startPosition.y;
                directionY = 1f;
            }
        }

        transform.position = newPos;
        
        // Continua girando o alvo para ficar dinâmico
        transform.Rotate(Vector3.up, 180 * Time.deltaTime);
    }

    // Usando a Colisão Sólida definitiva
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            health--;

            if (health <= 0)
            {
                if (playerShooter != null) playerShooter.AddScore(pointsValue);

                Destroy(collision.gameObject); // Destrói a bala
                Destroy(gameObject); // Destrói o alvo
            }
            else
            {
                Destroy(collision.gameObject);
            }
        }
    }
}