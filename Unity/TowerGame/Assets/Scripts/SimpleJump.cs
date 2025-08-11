using UnityEngine;
using System.Collections;

public class SimpleJump : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject gapDetector;
    public AudioClip deathSfx;
    public GameObject tower;
    public GameObject cam;
    public UnityReact react;
    
    [Header("Jump Heights")]
    public float smallJumpHeight = 1.5f;
    public float boostJumpHeight = 2.5f;

    [Header("Gravity")]
    public float fallSpeed = 10f;

    [Header("Animation")]
    public Animator animator; // Assign in Inspector if not on same GameObject
    
    [Header("SFX")]
    public AudioClip jumpSfx;

    public AudioSource audioSource_Running;

    private float verticalVelocity = 0f;
    private bool isJumping = false;
    private bool hasDoubleJumped = false;
    private float jumpStartY;
    private bool wasJumpingLastFrame = false;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        HandleInput();
        HandleJump();
        HandleDeath();
        
        if (isDead)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.down * 4f, Time.deltaTime * 0.5f);
        }

    }

    void HandleInput()
    {
        
        
        if (Input.GetMouseButtonDown(0) && gameManager.gameActive == true)
        {
            if (!isJumping)
            {
                // First jump from ground
                StartJump(smallJumpHeight);
                AudioManager.Instance.PauseSfx(audioSource_Running);
                hasDoubleJumped = false;
            }
            else if (!hasDoubleJumped)
            {
                // Mid-air boost jump
                BoostJump(boostJumpHeight);
            }
        }
        
        if (Input.GetMouseButtonDown(0) && !gameManager.gameActive)
        {
            gameManager.StartGame();
            AudioManager.Instance.ActivateSoundSource(audioSource_Running);
        }
    }

    bool isDead = false;

    void HandleDeath()
    {
        if (isDead) return; // Don't trigger twice

        bool overGap = !gapDetector.GetComponent<BoxCollider>().enabled;
        bool grounded = !isJumping;

        if (overGap && grounded)
        {
            isDead = true;
            Debug.Log("DEADDDDDD");
            AudioManager.Instance.PlaySfxWithVolume(deathSfx, 0.3f);
            AudioManager.Instance.PauseSfx(audioSource_Running);

            // Play death SFX
            // AudioManager.Instance.PlaySfx(deathClip);

            // Trigger death animation
            animator.SetTrigger("death");

            // Disable movement script (optional)
            // GetComponent<SimpleJump>().enabled = false;
            
            // turn off look at with camera
            cam.GetComponent<CameraFollow>().enabled = false;
            
            // Pause scene after 2 seconds
            StartCoroutine(PauseAfterDelay(0.5f));
            
            
            // fire react game over UI
            react.React_GameOverUI(true, gameManager.coins);
            
            // set tower rotation to 0
            tower.GetComponent<TowerController>().rotationSpeed = 0;

            // TODO: Respawn or game over logic here
        }
    }


    void StartJump(float height)
    {
        isJumping = true;
        animator.SetTrigger("IsJumping");
        jumpStartY = transform.position.y;
        verticalVelocity = Mathf.Sqrt(2 * fallSpeed * height);
        AudioManager.Instance.PlaySfxWithVolume(jumpSfx, 0.3f);

        // Trigger jump animation
        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }
    }

    void BoostJump(float height)
    {
        verticalVelocity = Mathf.Sqrt(2 * fallSpeed * height);
        hasDoubleJumped = true;

        // Trigger double jump animation (optional)
        if (animator != null)
        {
            animator.SetTrigger("DoubleJump");
        }
    }

    void HandleJump()
    {
        if (!isJumping)
        {
            // Detect landing for optional landing animation
            if (wasJumpingLastFrame && animator != null)
            {
                animator.SetTrigger("Land");
                
            }
            wasJumpingLastFrame = false;
            return;
        }

        // Apply vertical movement
        Vector3 pos = transform.position;
        pos.y += verticalVelocity * Time.deltaTime;
        transform.position = pos;

        // Gravity
        verticalVelocity -= fallSpeed * Time.deltaTime;

        // Land
        if (transform.position.y <= jumpStartY)
        {
            Vector3 reset = transform.position;
            reset.y = jumpStartY;
            transform.position = reset;
            animator.SetBool("isJumping", false);
            AudioManager.Instance.UnPauseSfx(audioSource_Running);

            isJumping = false;
            verticalVelocity = 0f;
        }

        wasJumpingLastFrame = isJumping;
    }
    
    IEnumerator PauseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        print("PAUSED");
        Time.timeScale = 0f; // Pause the game
        react.React_GameOverUI(true, gameManager.coins);
    }
}

