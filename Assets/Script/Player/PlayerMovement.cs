using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private static PlayerMovement instance;
    public static PlayerMovement Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerMovement>();
            }
            return instance;
        }
    }

    protected Rigidbody2D rb;
    protected float speed;
    [SerializeField] protected float pressX = 0f;
    [SerializeField] protected float pressY = 0f;
    [SerializeField] protected Vector2 direction = Vector2.down;
    [SerializeField] protected bool isMoving = false;
    public Vector2 Direction => direction;

    [Header("Input")] 
    public KeyCode inputUp = KeyCode.W;
    public KeyCode inputDown = KeyCode.S;
    public KeyCode inputLeft = KeyCode.A;
    public KeyCode inputRight = KeyCode.D;
    
    [Header("Sprites")]
    [SerializeField] protected AnimatedSpriteRenderer spriteRendererUp;
    [SerializeField] protected AnimatedSpriteRenderer spriteRendererDown;
    [SerializeField] protected AnimatedSpriteRenderer spriteRendererLeft;
    [SerializeField] protected AnimatedSpriteRenderer spriteRendererRight;
    // [SerializeField] protected AnimatedSpriteRenderer spriteRendererDeath;
    protected AnimatedSpriteRenderer activeSpriteRenderer;

    [Header("Input")]
    [SerializeField] protected float flickerSpeed = 10f;
    [SerializeField] protected Color damageColor = Color.red;
    protected Color originalColor;
    protected bool isFlickering = false;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        // DontDestroyOnLoad(this.gameObject);

        speed = PlayerStatus.Instance.SpeedDefault;
        rb = GetComponent<Rigidbody2D>();

        activeSpriteRenderer = spriteRendererDown;

        originalColor = Color.white;
    }

    void Update()
    {
        GetInput();
        UpdatePosition();
    }

    void FixedUpdate()
    {
        UpdatePosition();
    }

    void GetInput()
    {
        if (Input.GetKey(inputUp))
        {
            SetDirection(Vector2.up, spriteRendererUp);
        }
        else if (Input.GetKey(inputDown))
        {
            SetDirection(Vector2.down, spriteRendererDown);
        }
        else if (Input.GetKey(inputLeft))
        {
            SetDirection(Vector2.left, spriteRendererLeft);
        }
        else if (Input.GetKey(inputRight))
        {
            SetDirection(Vector2.right, spriteRendererRight);
        }
        else
        {
            activeSpriteRenderer.idle = true;
            isMoving = false;
        }
    }
    
    protected void SetDirection(Vector2 newDirection, AnimatedSpriteRenderer spriteRenderer)
    {
        isMoving = true;
        activeSpriteRenderer.idle = false;

        direction = newDirection;
    
        spriteRendererUp.enabled = spriteRenderer == spriteRendererUp;
        spriteRendererDown.enabled = spriteRenderer == spriteRendererDown;
        spriteRendererLeft.enabled = spriteRenderer == spriteRendererLeft;
        spriteRendererRight.enabled = spriteRenderer == spriteRendererRight;
    
        activeSpriteRenderer = spriteRenderer;
    }

    void UpdatePosition()
    {
        if (!isMoving) return;

        Vector2 position = rb.position;

        rb.MovePosition(position + direction * speed * Time.fixedDeltaTime);
    }

    public void ChangeSpeed(float newSpeed)
    {
        this.speed = newSpeed;
    }

    public void Flickering()
    {
        if (!isFlickering)
        {
            // Start the flickering coroutine
            StartCoroutine(FlickerCoroutine());
        }
    }

    private IEnumerator FlickerCoroutine()
    {
        isFlickering = true;

        SpriteRenderer objectRenderer1 = spriteRendererUp.GetComponent<SpriteRenderer>();
        SpriteRenderer objectRenderer2 = spriteRendererDown.GetComponent<SpriteRenderer>();
        SpriteRenderer objectRenderer3 = spriteRendererLeft.GetComponent<SpriteRenderer>();
        SpriteRenderer objectRenderer4 = spriteRendererRight.GetComponent<SpriteRenderer>();

        for(int i = 0; i < 5; i++)
        {
            objectRenderer1.color = (objectRenderer1.color == originalColor) ? damageColor : originalColor;
            objectRenderer2.color = (objectRenderer2.color == originalColor) ? damageColor : originalColor;
            objectRenderer3.color = (objectRenderer3.color == originalColor) ? damageColor : originalColor;
            objectRenderer4.color = (objectRenderer4.color == originalColor) ? damageColor : originalColor;

            yield return new WaitForSeconds(1f / flickerSpeed);

            objectRenderer1.color = (objectRenderer1.color == originalColor) ? damageColor : originalColor;
            objectRenderer2.color = (objectRenderer2.color == originalColor) ? damageColor : originalColor;
            objectRenderer3.color = (objectRenderer3.color == originalColor) ? damageColor : originalColor;
            objectRenderer4.color = (objectRenderer4.color == originalColor) ? damageColor : originalColor;

            yield return new WaitForSeconds(1f / flickerSpeed);

        }

        objectRenderer1.color = originalColor;
        objectRenderer2.color = originalColor;
        objectRenderer3.color = originalColor;
        objectRenderer4.color = originalColor;

        isFlickering = false;
    }
}