using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

public class FPSAimController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f; 

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public bool lockCursor = true;
    
    [Header("Shooting - Armas Duplas")]
    public Transform mustardFirePoint;
    public GameObject mustardBulletPrefab;
    public Transform ketchupFirePoint;
    public GameObject ketchupBulletPrefab;
    
    [Header("Shooting - Status")]
    public float bulletSpeed = 50f;
    public float fireRate = 0.2f;
    public int maxAmmo = 30;
    public float reloadTime = 1.5f;
    public float maxShootDistance = 100f;
    
    [Header("UI")]
    public Image crosshair;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;
    public UnityEngine.UI.Button restartButton;
    
    [Header("Game Settings")]
    public int scorePerHit = 10;
    public float gameDuration = 60f;
    public LayerMask targetLayer;
    
    private float xRotation = 0f;
    private int currentAmmo;
    private int currentScore;
    private float nextFireTime;
    private bool isReloading = false;
    private float gameTimer;
    private bool isGameActive = true;
    private Camera playerCamera;
    
    // Variável para acessar a física do Player
    private Rigidbody rb;
    
    void Start()
    {
        // Pega automaticamente o Rigidbody do Player
        rb = GetComponent<Rigidbody>();
        
        currentAmmo = maxAmmo;
        currentScore = 0;
        gameTimer = gameDuration;
        playerCamera = Camera.main;
        
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        UpdateUI();
        
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
    }
    
    // FixedUpdate roda em sincronia com a Física da Unity (ideal para movimento)
    void FixedUpdate()
    {
        if (!isGameActive || Keyboard.current == null) return;
        HandleMovement();
    }
    
    // Update roda a cada frame de vídeo (ideal para cliques, mira e UI)
    void Update()
    {
        if (!isGameActive || Mouse.current == null || Keyboard.current == null) return;
        
        HandleMouseLook();
        UpdateCrosshairFeedback();
        
        gameTimer -= Time.deltaTime;
        if (gameTimer <= 0)
        {
            EndGame();
            return;
        }
        
        // Lógica de Disparo
        if (!isReloading && currentAmmo > 0 && Time.time >= nextFireTime)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame) 
            {
                Shoot(mustardFirePoint, mustardBulletPrefab);
            }
            else if (Mouse.current.rightButton.wasPressedThisFrame) 
            {
                Shoot(ketchupFirePoint, ketchupBulletPrefab);
            }
        }
        
        if (Keyboard.current.rKey.wasPressedThisFrame && !isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }
        
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            lockCursor = false;
        }
        
        if (Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            lockCursor = true;
        }
        
        if (ammoText != null)
        {
            if (isReloading)
                ammoText.text = "RELOADING...";
            else
                ammoText.text = $"Ammo: {currentAmmo}/{maxAmmo}\nTime: {Mathf.CeilToInt(gameTimer)}s";
        }
    }

    void HandleMovement()
    {
        float x = 0f;
        float z = 0f;

        if (Keyboard.current.wKey.isPressed) z += 1f;
        if (Keyboard.current.sKey.isPressed) z -= 1f;
        if (Keyboard.current.dKey.isPressed) x += 1f;
        if (Keyboard.current.aKey.isPressed) x -= 1f;

        Vector3 inputDir = new Vector3(x, 0f, z).normalized;
        Vector3 move = transform.right * inputDir.x + transform.forward * inputDir.z;

        // Movimentação fluida e sem tremedeira pela física!
        if (rb != null)
        {
            rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
        }
    }
    
    void HandleMouseLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * mouseSensitivity * 0.1f * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * 0.1f * Time.deltaTime;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);
        else
            transform.Rotate(Vector3.up * mouseX);
    }
    
    void UpdateCrosshairFeedback()
    {
        if (crosshair != null)
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit, maxShootDistance, targetLayer))
                crosshair.color = Color.red; 
            else
                crosshair.color = Color.white; 
        }
    }
    
    void Shoot(Transform currentFirePoint, GameObject currentBulletPrefab)
    {
        nextFireTime = Time.time + fireRate;
        currentAmmo--;
        
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        Vector3 targetPoint;
        
        if (Physics.Raycast(ray, out RaycastHit hit, maxShootDistance, targetLayer))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(maxShootDistance);
        
        Vector3 shootDirection = (targetPoint - currentFirePoint.position).normalized;
        
        GameObject bullet = Instantiate(currentBulletPrefab, currentFirePoint.position, Quaternion.LookRotation(shootDirection));
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        
        if (bulletRb != null)
            bulletRb.linearVelocity = shootDirection * bulletSpeed;
        
        Destroy(bullet, 5f);
        UpdateUI();
        
        if (crosshair != null)
            StartCoroutine(FlashCrosshair());
    }
    
    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateUI();
    }
    
    IEnumerator FlashCrosshair()
    {
        Color originalColor = crosshair.color;
        crosshair.color = Color.yellow;
        yield return new WaitForSeconds(0.05f);
        crosshair.color = originalColor;
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateUI();
    }
    
    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {currentScore}";
    }
    
    void EndGame()
    {
        isGameActive = false;
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = $"Game Over!\nFinal Score: {currentScore}\nPress Restart";
        }
        if (restartButton != null)
            restartButton.gameObject.SetActive(true);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    void RestartGame()
    {
        currentScore = 0;
        currentAmmo = maxAmmo;
        gameTimer = gameDuration;
        isGameActive = true;
        isReloading = false;
        
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
        if (restartButton != null)
            restartButton.gameObject.SetActive(false);
        
        UpdateUI();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        lockCursor = true;
        xRotation = 0f;
    }
}