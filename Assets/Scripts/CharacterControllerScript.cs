using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CharacterControllerScript : MonoBehaviour
{

    public Rigidbody2D rb;
    public Animator animator;
    bool isFacingRight = true;
    public ParticleSystem smokeFX;
    AudioManager audioManager;
    public Canvas victoryCanvas;
    public Canvas gameOverCanvas;
    public Canvas pauseMenu;
    public bool isPaused = false;
    public int collects = 4;

    [Header("Collider")]
    public BoxCollider2D myBodyCollider;
    public Vector2 overlapBoxOffset = Vector2.zero;
    public Vector2 overlapBoxSize = new(0.5f, 0.5f);


    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;

    [Header("Dashing")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 0.1f;
    bool isDashing;
    bool canDash = true;
    TrailRenderer trailRenderer;

    [Header("Jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 1;
    int jumpsRemaining;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new(0.49f, 0.03f);
    public LayerMask blockLayer;
    bool isGrounded;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new(0.49f, 0.03f);

    [Header("WallMovement")]
    public float wallSlideSpeed = 2;
    bool isWallSliding;

    //Wall Jumping
    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new(5f, 10f);

    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        Time.timeScale = 1;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                UnpauseGame();
            }
            else
            {
                PauseGame();
            }
        }

        if (FindObjectOfType<Tetromino>().isLocked == false)
        {
            return;
        }

        if (FindObjectOfType<Tetromino>().isLocked == true)
        {

            animator.SetFloat("magnitude", rb.velocity.magnitude);
            animator.SetBool("isWallSliding", isWallSliding);

            if (isDashing)
            {
                return;
            }
            GroundCheck();
            ProcessGravity();
            ProcessWallSlide();
            ProcessWallJump();

            if (!isWallJumping)
            {
                rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);
                Flip();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseMenu.gameObject.SetActive(true); // Show the pause menu
        isPaused = true;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        pauseMenu.gameObject.SetActive(false); // Hide the pause menu
        isPaused = false;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (FindObjectOfType<Tetromino>().isLocked == true)
        {
            if (context.performed && canDash)
            {
                StartCoroutine(DashCoroutine());
            }
        }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;

        trailRenderer.emitting = true;
        float dashDirection = isFacingRight ? 1f : -1f;

        rb.velocity = new Vector2(dashDirection * dashSpeed, rb.velocity.y);

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = new Vector2(0f, rb.velocity.y);

        isDashing = false;
        trailRenderer.emitting = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }
    public void Jump(InputAction.CallbackContext context)
    {
        if (FindObjectOfType<Tetromino>().isLocked == true)
        {
            if (jumpsRemaining > 0)
            {
                if (context.performed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                    jumpsRemaining--;
                    JumpFX();
                    audioManager.PlaySFX(audioManager.jump);
                }
                else if (context.canceled)
                {

                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.3f);
                    jumpsRemaining--;
                    JumpFX();
                    audioManager.PlaySFX(audioManager.jump);
                }
            }

            //Wall Jump
            if (context.performed && wallJumpTimer > 0f)
            {
                isWallJumping = true;
                rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
                wallJumpTimer = 0;
                JumpFX();
                audioManager.PlaySFX(audioManager.jump);

                if (transform.localScale.x != wallJumpDirection)
                {
                    isFacingRight = !isFacingRight;
                    Vector3 ls = transform.localScale;
                    ls.x *= -1f;
                    transform.localScale = ls;
                }

                Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
            }
        }

        else
        {
            return;
        }
    }

    private void JumpFX()
    {
        smokeFX.Play();
    }
    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, blockLayer))
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    private bool WallCheck()
    {
        return (Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, blockLayer));
    }
    private void ProcessGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }
    private void ProcessWallSlide()
    {
        if (!isGrounded & WallCheck() & horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }
    private void ProcessWallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;

            if (rb.velocity.y == 0)
            {
                smokeFX.Play();
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Goal"))
        {
            if (isGrounded)
            {
                audioManager.PlaySFX(audioManager.victory);
                Victory();
            }
        }
    }

    public void Victory()
    {
        victoryCanvas.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(wallCheckPos.position, wallCheckSize);

        Gizmos.color = Color.red;
        Vector2 boxCenter = (Vector2)transform.position + overlapBoxOffset;
        Gizmos.DrawWireCube(boxCenter, overlapBoxSize);
    }
}

