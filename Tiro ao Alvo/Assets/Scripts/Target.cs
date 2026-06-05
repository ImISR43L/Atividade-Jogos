using UnityEngine;

public class Target : MonoBehaviour
{
    [HideInInspector]
    public TargetSpawner.SpawnPoint spawnPoint;
    [HideInInspector]
    public bool moveHorizontal = false;
    [HideInInspector]
    public bool moveVertical = false;
    [HideInInspector]
    public float moveSpeed = 1f;
    [HideInInspector]
    public float moveRange = 5f;
    [HideInInspector]
    public int health = 1;
    [HideInInspector]
    public int pointsValue = 10;

    private Vector3 startPosition;
    private float directionX = 1f;
    private float directionY = 1f;
    private FPSAimController playerShooter;
    void Start()
    {
        startPosition = transform.position;
        playerShooter = FindAnyObjectByType<FPSAimController>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = transform.position;

        if (moveHorizontal)
        {
            newPos.x += directionX * moveSpeed * Time.deltaTime;
            if(Mathf.Abs(newPos.y - startPosition.y) >= moveRange)
            {
                directionY *= .1f;
            }
        }

        transform.position = newPos;

        transform.Rotate(Vector3.up, 180 * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            health--;

            if (health <= 0)
            {
                if (playerShooter != null) playerShooter.AddScore(pointsValue);

                Destroy(other.gameObject);
                Destroy(gameObject);
            }
            else
            {
                Destroy(other.gameObject);
            }
        }
    }
}
