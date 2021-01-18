using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveController : MonoBehaviour
{
    [Header("Movement")]
    public float moveAccel;
    public float maxSpeed;

    [Header("Jump")]
    public float jumpAccel;

    [Header("Ground Raycast")]
    public float groundRaycastDistance;
    public LayerMask groundLayerMask;

    [Header("Scoring")]
    public ScoreController score;
    public float scoringRatio;

    [Header("Game Over")]
    public float fallPositionY;
    public GameObject gameOverScreen;

    [Header("Camera")]
    public CameraMoveController cameraController;

    private Rigidbody2D rig;
    private Animator animator;
    private CharacterSoundController sound;

    private bool isOnGround;
    private bool isJumping;

    private float lastPositionX;

    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sound = GetComponent<CharacterSoundController>();

        lastPositionX = transform.position.x;
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastDistance, groundLayerMask);
        if (hit)
        {
            if (!isOnGround && rig.velocity.y <= 0)
            {
                isOnGround = true;
            }
        }
        else
        {
            isOnGround = false;
        }

        // change animation
        animator.SetBool("isOnGround", isOnGround);

        // read input
        if (Input.GetMouseButtonDown(0))
        {
            if (isOnGround)
            {
                isJumping = true;

                sound.PlayJump();
            }
        }

        // calculate score
        int distancePassed = Mathf.FloorToInt(transform.position.x - lastPositionX);
        int scoreIncrement = Mathf.FloorToInt(distancePassed / scoringRatio);

        if (scoreIncrement > 0)
        {
            score.IncreaseCurrentScore(scoreIncrement);
            lastPositionX += distancePassed;
        }

        // gameover
        if (transform.position.y < fallPositionY)
        {
            GameOver();
        }
    }

    private void FixedUpdate()
    {
        Vector2 velocityVector = rig.velocity;

        if (isJumping)
        {
            velocityVector.y += jumpAccel;
            isJumping = false;
        }

        velocityVector.x = Mathf.Clamp(velocityVector.x + moveAccel * Time.deltaTime, 0.0f, maxSpeed);

        rig.velocity = velocityVector;
    }

    private void GameOver()
    {
        score.FinishScoring();

        cameraController.enabled = false;

        gameOverScreen.SetActive(true);

        this.enabled = false;
    }

    private void OnDrawGizmos()
    {
        // raycast visualization
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * groundRaycastDistance), Color.red);
    }
}