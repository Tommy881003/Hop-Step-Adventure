using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerControl : MonoBehaviour {

    private Rigidbody2D rb;
    private Vector2 velocity;
    private Animator animator;
    private SpriteRenderer sr;
    public ParticleSystem dash;
    public GameObject phantom;

    public bool canJump;              //can the player jump now?
    private bool onWall;
    private bool backWall;
    private bool upWall;
    public bool onGround;
    public bool canControlJump;
    private float fallGravity = 4f;  //control gravity when jumping
    private float lowGravity = 3f;    //control gravity when jumping
    private float jumpVelocity = 13.5f;
    private float walkVelocity = 7.5f;

    public bool canDash;              //can the player dash now?
    public bool isDashing;
    public bool isTransitioning;
    public bool dead;
    public bool canControlMove;
    private Vector2 dashDirection;
    public float dashTime = 0;
    public float dashCooldown = 0;
    private float spawntime = 0.05f;
    public Vector2 spawnPos;
    public PlayerPosition pos;
    public int currentLevel = 0;

    void Start()
    {
        if (pos != null && pos.testMode == true)
        {
            this.transform.position = pos.spawnPos;
            spawnPos = pos.spawnPos;
            currentLevel = pos.currentLevel;
        }
        else
            spawnPos = this.transform.position;
        rb = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();
        sr = this.GetComponent<SpriteRenderer>();
        dashDirection = new Vector2(0, 0);
        canJump = false;
        canDash = true;
        isDashing = false;
        canControlJump = true;
        canControlMove = true;
        onWall = false;
        backWall = false;
        isTransitioning = false;
        rb.gravityScale = 2.75f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsDead())
            return;
        CheckCollision();

        /* Control moving */

        velocity = rb.velocity;

        if ((Input.GetKeyDown(KeyCode.J) && canDash == true) || isDashing == true)
            velocity = Dash(velocity);
        if (isDashing == false)
            velocity = HorizontalMoving(velocity);
        if (Input.GetKeyDown(KeyCode.K) || (Input.GetKey(KeyCode.L) && isTransitioning == false))
            velocity = JumpAndClimb(velocity);

        velocity = JumpGravity(velocity);

        rb.velocity = velocity;

        SetAnimAndFlip();
    }

    Vector2 HorizontalMoving(Vector2 velocity)
    {
        int dir = 0; // 1 for right, -1 for left, 0 for none
        if (Input.GetAxisRaw("Horizontal") > 0) dir = 1;
        else if (Input.GetAxisRaw("Horizontal") < 0) dir = -1;
        else dir = 0;
        if (canControlMove == true)
            velocity.x = walkVelocity * dir;
        else
        {
            if (dir == 1)
            {
                if (velocity.x <= walkVelocity)
                    velocity.x += 0.75f;
                else if (velocity.x >= 12.5f)
                    velocity.x = 17.5f;
            }
            else if (dir == -1)
            {
                if (velocity.x >= -walkVelocity)
                    velocity.x -= 0.75f;
                else if (velocity.x <= -12.5f)
                    velocity.x = -17.5f;
            }
            else
            {
                if(Mathf.Abs(velocity.x) >= 5)
                    velocity.x = velocity.x * 0.95f;
            }
        }
        return velocity;
    }

    Vector2 JumpAndClimb(Vector2 velocity)
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            if (canJump == true)
            {
                canJump = false;
                canControlJump = true;
                if (isDashing == true)
                {
                    isDashing = false;
                    canControlMove = false;
                    velocity.y = 10;
                }
                else
                    velocity.y = jumpVelocity;
            }
            else if(onWall == true || backWall == true)
            {
                isDashing = false;
                canControlMove = false;
                canControlJump = true;
                velocity.y = jumpVelocity;
                if ((sr.flipX == false && onWall == true) || (sr.flipX == true && backWall == true))
                    velocity.x = -walkVelocity;
                else
                    velocity.x = walkVelocity;
            }
        }
        else if(Input.GetKey(KeyCode.L) && onWall == true)
        {
            velocity.x = 0;
            rb.gravityScale = 0;
            if (Input.GetKey(KeyCode.W) && upWall == true)
                velocity.y = 3f;
            else if (Input.GetKey(KeyCode.S))
                velocity.y = -5f;
            else
                velocity.y = 0;
            canControlMove = false;
        }
        return velocity;
    }

    Vector2 JumpGravity(Vector2 velocity)
    {
        if (velocity.y > jumpVelocity)
            canControlJump = false;
        if (canJump == true)
            canControlJump = true;
        if (velocity.y < -35)
            velocity = new Vector2(velocity.x, -35);
        if (canControlJump == true)
        {
            if (velocity.y < 0)
                velocity += Vector2.up * Physics2D.gravity.y * (fallGravity) * Time.deltaTime;
            else if (velocity.y > 0 && (!Input.GetKey(KeyCode.K)))
                velocity += Vector2.up * Physics2D.gravity.y * (lowGravity) * Time.deltaTime;
        }
        else
        {
            if (!(onWall == true && Input.GetKey(KeyCode.L)))
                velocity += Vector2.up * Physics2D.gravity.y * (lowGravity) * Time.deltaTime;
        }
        return velocity;
    }

    Vector2 Dash(Vector2 velocity)
    {
        if (Input.GetKeyDown(KeyCode.J) && canDash == true)
        {
            Instantiate(dash, this.transform.position, Quaternion.identity);
            dashDirection = new Vector2((sr.flipX == false ? 1 : -1), 0);
            if (Input.GetKey(KeyCode.W))
                dashDirection = new Vector2(0, 1);
            if (Input.GetKey(KeyCode.S))
                dashDirection = new Vector2(0, -1);
            if (Input.GetKey(KeyCode.D))
                dashDirection.x = 1;
            if (Input.GetKey(KeyCode.A))
                dashDirection.x = -1;
            if (Physics2D.gravity.y > 0)
                dashDirection.y = -dashDirection.y;
            canControlMove = true;
            canControlJump = false;
            canDash = false;
            isDashing = true;
            StartCoroutine(Dashing());
        }

        if (isDashing)
        {
            canDash = false;
            if (spawntime >= 0.05f)
            {
                GameObject temp = Instantiate(phantom, this.transform.position, Quaternion.identity);
                temp.GetComponent<SpriteRenderer>().sprite = sr.sprite;
                temp.GetComponent<SpriteRenderer>().flipX = sr.flipX;
                temp.transform.rotation = this.transform.rotation;
                spawntime = 0;
            }
            velocity = dashDirection.normalized * 30;
            spawntime += Time.deltaTime;
        }
        return velocity;
    }

    IEnumerator Dashing()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        if(canControlMove == true && isDashing == true)
            rb.velocity = dashDirection.normalized * 10;
        isDashing = false;
    }

    public IEnumerator DashReset()
    {
        yield return new WaitForSecondsRealtime(0.05f);
        canDash = true;
    }

    bool IsDead()
    {
        if (dead)
        {
            animator.SetBool("Dead", dead);
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            dashTime = 0;
            return true;
        }
        else
        {
            animator.SetBool("Dead", dead);
            rb.gravityScale = 2.75f;
            return false;
        }
    }

    void CheckCollision()
    {
        Vector2 leftfoot, rightfoot, wallVector;
        leftfoot = new Vector2(rb.transform.position.x - 0.3f, rb.transform.position.y - rb.transform.localScale.y -0.01f);
        rightfoot = new Vector2(rb.transform.position.x + 0.3f, rb.transform.position.y - rb.transform.localScale.y - 0.01f);
        RaycastHit2D lefthit = Physics2D.Raycast(leftfoot, Vector2.down, 0.04f), righthit = Physics2D.Raycast(rightfoot, Vector2.down, 0.04f);
        if ((lefthit.collider != null && lefthit.collider.isTrigger == false) || (righthit.collider != null && righthit.collider.isTrigger == false))
        {
            onGround = true;
            canJump = true;
            canControlMove = true;
            if (isDashing == false)
                StartCoroutine(DashReset());
        }
        else
        {
            canJump = false;
            onGround = false;
        }
        wallVector = (sr.flipX == false) ? Vector2.right : Vector2.left;
        RaycastHit2D wallhit = Physics2D.Raycast(this.transform.position, wallVector, 0.6f, ~((1 << 9) | (1 << 8))), 
                     backwall = Physics2D.Raycast(this.transform.position, -wallVector, 0.6f, ~((1 << 9) | (1 << 8))),
                     upwall = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y + 0.1f), wallVector, 0.6f, ~((1 << 9) | (1 << 2)));
        onWall = (wallhit.collider != null && wallhit.collider.isTrigger == false) ? true : false;
        backWall = (backwall.collider != null && backwall.collider.isTrigger == false) ? true : false;
        upWall = (upwall.collider != null && upwall.collider.isTrigger == false) ? true : false;
    }

    void SetAnimAndFlip()
    {
        if (canControlMove && (onWall == false || !Input.GetKey(KeyCode.L)))
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
                sr.flipX = false;
            else if (Input.GetAxisRaw("Horizontal") < 0)
                sr.flipX = true;
        }
        else
        {
            if (rb.velocity.x > 0.1f)
                sr.flipX = false;
            else if (rb.velocity.x < -0.1f)
                sr.flipX = true;
        }

        animator.SetFloat("SpeedX", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("SpeedY", rb.velocity.y);
        animator.SetBool("IsJumping", !canJump);
    }

    public IEnumerator EndTransition()
    {
        yield return new WaitUntil(() => onGround == true);
        isTransitioning = false;
    }
}