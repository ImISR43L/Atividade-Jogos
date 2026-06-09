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

        if (moveHorizontal)
        {
            newPos.x += directionX * moveSpeed * Time.deltaTime;
            // Verifica o eixo X no movimento horizontal
            if(Mathf.Abs(newPos.x - startPosition.x) >= moveRange)
            {
                directionX *= -1f; // Inverte a direção
            }
        }

        transform.position = newPos;
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