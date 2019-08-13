using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

public class PlayerControl : MonoBehaviour {

    [SerializeField]
    private ButtonHandler handler;
    private Rigidbody2D rb;
    private Vector2 velocity;
    private Animator animator;
    private SpriteRenderer sr;
    public ParticleSystem dash;
    public ParticleSystem jump;
    public GameObject phantom;
    public Material NoDash;
    private Material Normal;
    public PlayerPosition pos;
    [SerializeField]
    private InputManager input;
    private ObjAudioManager audioManager;

    [HideInInspector]
    public Vector2 dashDirection;
    private float spawntime = 0.05f;
    [HideInInspector]
    public Vector2 spawnPos;
    [HideInInspector]
    public int currentLevel = 0;

    [HideInInspector]
    public bool canJump, onWall, backWall, upWall, canCheckWall , dashJump;
    public bool canControlJump, onGround;
    [HideInInspector]
    public bool springJump, canControlSpring, canControlMove;

    private float fallGravity = 4.5f, lowGravity = 3f, jumpVelocity = 13.5f, walkVelocity = 7.5f;  

    [HideInInspector]
    public bool canReset = true, canDash, isDashing, isTransitioning, dead, canCollect, complete = false, isCutScene;

    [HideInInspector]
    public float dashTime = 0, dashCooldown = 0;

    [HideInInspector]
    public EventHandler OnDash;

    void Start()
    {
        audioManager = this.GetComponent<ObjAudioManager>();
        if (handler != null)
            handler.OnRetry += kill;
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
        Normal = sr.material;
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
        canCollect = true;
        dashJump = false;
        springJump = false;
        canCheckWall = true;
    }

    private void Update()
    {
        if (IsDead() || isCutScene)
            return;
        velocity = rb.velocity;
        CheckCollision();
        if (onGround == false)
            velocity = JumpGravity(velocity);
        rb.velocity = velocity;
        if (complete)
        {
            isDashing = false;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX;
            SetAnimAndFlip();
            return;
        }
            

        /* Control moving */
        if (((Input.GetKeyDown(input.dash) && canDash == true) || isDashing == true) && isTransitioning == false)
            velocity = Dash(velocity);
        if (isDashing == false)
            velocity = HorizontalMoving(velocity);
        if (Input.GetKeyDown(input.jump) || (Input.GetKey(input.climb) && isTransitioning == false))
            velocity = JumpAndClimb(velocity);
        rb.velocity = velocity;

        SetAnimAndFlip();
    }

    Vector2 HorizontalMoving(Vector2 velocity)
    {
        int dir = 0; // 1 for right, -1 for left, 0 for none
        if (Input.GetKey(input.right)) dir = 1;
        else if (Input.GetKey(input.left)) dir = -1;
        else dir = 0;
        if (canControlMove == true)
            velocity.x = walkVelocity * dir;
        else
        {
            if(springJump && canControlSpring)
            {
                if (dir == 1)
                {
                    if (velocity.x < 0)
                        velocity.x *= 0.92f;
                }
                else if (dir == -1)
                {
                    if (velocity.x > 0)
                        velocity.x *= 0.92f;
                }
                else
                {
                    if (Mathf.Abs(velocity.x) >= 5)
                        velocity.x = velocity.x * 0.95f;
                }
            }
            else if(springJump == false)
            {
                if (dir == 1)
                {
                    if (velocity.x <= walkVelocity)
                        velocity.x += 1.5f;
                    else if (velocity.x >= 12.5f)
                        velocity.x = 17.5f;
                }
                else if (dir == -1)
                {
                    if (velocity.x >= -walkVelocity)
                        velocity.x -= 1.5f;
                    else if (velocity.x <= -12.5f)
                        velocity.x = -17.5f;
                }
                else
                {
                    if (Mathf.Abs(velocity.x) >= 5)
                        velocity.x = velocity.x * 0.95f;
                }
            }
        }
        return velocity;
    }

    Vector2 JumpAndClimb(Vector2 velocity)
    {
        if(Input.GetKeyDown(input.jump))
        {
            if (canJump == true)
            {
                if(jump != null)
                    Instantiate(jump, new Vector3(this.transform.position.x, this.transform.position.y - 1.2f, this.transform.position.z) , Quaternion.identity, this.transform);
                canJump = false;
                canControlJump = true;
                if (dashJump == true)
                {
                    isDashing = false;
                    canControlMove = false;
                    dashJump = false;
                    velocity.y = 12f;
                    velocity.x = 17.5f * Mathf.Sign(dashDirection.x);
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
        else if(Input.GetKey(input.climb) && onWall == true)
        {
            velocity.x = 0;
            rb.gravityScale = 0;
            if (Input.GetKey(input.up) && upWall == true)
                velocity.y = 3f;
            else if (Input.GetKey(input.down))
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
        if (velocity.y < -20 && isDashing == false)
            velocity = new Vector2(velocity.x, -20);
        if (canControlJump == true || isTransitioning || complete)
        {
            if (velocity.y < 0)
                velocity += Vector2.up * Physics2D.gravity.y * (fallGravity) * Time.deltaTime;
            else if ((velocity.y > 0 && (!Input.GetKey(input.jump))) || springJump == true || complete)
                velocity += Vector2.up * Physics2D.gravity.y * (lowGravity) * Time.deltaTime;
        }
        else
        {
            if (!(onWall == true && Input.GetKey(input.climb)))
                rb.gravityScale = 3;
            if (!(onWall == true && Input.GetKey(input.climb)))
                velocity += Vector2.up * Physics2D.gravity.y * (lowGravity) * Time.deltaTime;
        }
        return velocity;
    }

    Vector2 Dash(Vector2 velocity)
    {
        if (Input.GetKeyDown(input.dash) && canDash == true && isDashing == false)
        {
            audioManager.PlayByName("Dash");
            if (OnDash != null)
                OnDash(this, EventArgs.Empty);
            Camera.main.GetComponent<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
            dashDirection = new Vector2((sr.flipX == false ? 1 : -1), 0);
            if (Input.GetKey(input.up))
                dashDirection = new Vector2(0, 1);
            if (Input.GetKey(input.down))
                dashDirection = new Vector2(0, -1);
            if (Input.GetKey(input.right))
                dashDirection.x = 1;
            if (Input.GetKey(input.left))
                dashDirection.x = -1;
            ParticleSystem dashParticle = Instantiate(dash, this.transform.position, Quaternion.Euler(0,0,Vector2.SignedAngle(Vector2.up,dashDirection)),this.transform);
            canControlMove = true;
            canControlJump = false;
            canDash = false;
            isDashing = true;
            if(dashDirection.x != 0)
                dashJump = true;
            if(dashDirection.y > 0 && onGround && !Input.GetKeyDown(input.up) && jump != null)
                Instantiate(jump, new Vector3(this.transform.position.x, this.transform.position.y - 1.2f, this.transform.position.z), Quaternion.identity, this.transform);
            StartCoroutine(Dashing(dashParticle));
            StartCoroutine((Phantom(spawntime)));
        }
        if (isDashing)
            velocity = dashDirection.normalized * 30;
        return velocity;
    }

    IEnumerator Phantom(float spawn)
    {
        GameObject temp = Instantiate(phantom, this.transform.position, Quaternion.identity);
        temp.GetComponent<SpriteRenderer>().sprite = sr.sprite;
        temp.GetComponent<SpriteRenderer>().flipX = sr.flipX;
        temp.transform.rotation = this.transform.rotation;
        yield return new WaitForSeconds(spawntime);
        if (isDashing)
            StartCoroutine(Phantom(spawn));
    }

    IEnumerator Dashing(ParticleSystem toKill)
    {
        yield return new WaitForSeconds(0.15f);
        if (canControlMove == true && isDashing == true && dashJump == false)
            rb.velocity = dashDirection.normalized * 10;
        isDashing = false;
        yield return new WaitForSeconds(0.05f);
        yield return new WaitWhile(() => Mathf.Abs(rb.velocity.x) > 17);
        var em = toKill.emission;
        em.enabled = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(toKill.gameObject);
    }

    bool IsDead()
    {
        if (dead)
        {
            sr.material = Normal;
            animator.SetBool("Dead", dead);
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            dashTime = 0;
            canCheckWall = true;
            return true;
        }
        else
        {
            animator.SetBool("Dead", dead);
            rb.gravityScale = 2.75f;
            return false;
        }
    }

    [HideInInspector]
    public RaycastHit2D lefthit, righthit, wallhit;

    void CheckCollision()
    {
        Vector2 leftfoot, rightfoot, wallVector;
        leftfoot = new Vector2(rb.transform.position.x - 0.3f, rb.transform.position.y - rb.transform.localScale.y -0.01f);
        rightfoot = new Vector2(rb.transform.position.x + 0.3f, rb.transform.position.y - rb.transform.localScale.y - 0.01f);
        lefthit = Physics2D.Raycast(leftfoot, Vector2.down, 0.04f, ~((1 << 10) | (1 << 2) | (1 << 12))); righthit = Physics2D.Raycast(rightfoot, Vector2.down, 0.04f, ~((1 << 10) | (1 << 2) | (1 << 12)));
        if ((lefthit.collider != null && lefthit.collider.isTrigger == false) || (righthit.collider != null && righthit.collider.isTrigger == false))
        {
            onGround = true;
            canJump = true;
            canControlMove = true;
            if (canReset == true && isDashing == false && canDash == false)
            {
                onGround = false;
                StartCoroutine(DashReset());
            }
        }
        else if(isDashing && Mathf.Sign(dashDirection.y) > 0)
            StartCoroutine(ApplyCollisionChange(0));
        else
            StartCoroutine(ApplyCollisionChange(0.1f));
        if(canCheckWall)
        {
            wallVector = (sr.flipX == false) ? Vector2.right : Vector2.left;
            wallhit = Physics2D.Raycast(this.transform.position, wallVector, 0.6f, ~((1 << 9) | (1 << 8) | (1 << 2) | (1 << 10) | (1 << 12)));
            RaycastHit2D backwall = Physics2D.Raycast(this.transform.position, -wallVector, 0.6f, ~((1 << 9) | (1 << 8) | (1 << 2) | (1 << 10) | (1 << 12))),
                         upwall = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y + 0.1f), wallVector, 0.6f, ~((1 << 9) | (1 << 8) | (1 << 2) | (1 << 10) | (1 << 12)));
            onWall = (wallhit.collider != null && wallhit.collider.isTrigger == false) ? true : false;
            backWall = (backwall.collider != null && backwall.collider.isTrigger == false) ? true : false;
            upWall = (upwall.collider != null && upwall.collider.isTrigger == false) ? true : false;
        }
    }

    public void reportCollision(string suffix)
    {
        if ((lefthit.collider == null && sr.flipX == true) || (righthit.collider == null && sr.flipX == false))
            return;
        string name = ((sr.flipX == true) ? lefthit.collider.name : righthit.collider.name);
        string toSend;
        switch (name)
        {
            case "Scaffolding":
                toSend = "Metal" + suffix;
                break;
            case "Bridge":
                toSend = "Wood" + suffix;
                break;
            default:
                toSend = "Concrete" + suffix;
                break;
        }
        audioManager.PlayByName(toSend);
    }

    public void SetAnimAndFlip()
    {
        if (canControlMove && (onWall == false || !Input.GetKey(input.climb)) && complete == false)
        {
            if (Input.GetKey(input.right))
                sr.flipX = false;
            else if (Input.GetKey(input.left))
                sr.flipX = true;
        }
        else
        {
            if (rb.velocity.x > 0.1f)
                sr.flipX = false;
            else if (rb.velocity.x < -0.1f)
                sr.flipX = true;
        }

        if (canDash)
            sr.material = Normal;
        else
            sr.material = NoDash;

        animator.SetFloat("SpeedX", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("SpeedY", rb.velocity.y);
        animator.SetBool("IsJumping", !canJump);
        animator.SetBool("OnWall", onWall && Input.GetKey(input.climb));
    }

    public IEnumerator EndTransition()
    {
        yield return new WaitUntil(() => velocity.y <= 0);
        isTransitioning = false;
    }

    IEnumerator ApplyCollisionChange(float time)
    {
        yield return new WaitForSeconds(time);
        canJump = false;
        onGround = false;
        dashJump = false;
    }

    IEnumerator DashReset()
    {
        canReset = false;
        yield return new WaitForSeconds(0.1f);
        canDash = true;
        canReset = true;
        dashJump = false;
    }

    public IEnumerator Spring()
    {
        canControlSpring = false;
        StartCoroutine(SpringControl());
        yield return new WaitUntil(() => onGround == true || isDashing == true || (Mathf.Abs(rb.velocity.x) < 5 && canControlSpring == true));
        springJump = false;
        canControlSpring = false;
    }

    IEnumerator SpringControl()
    {
        yield return new WaitForSeconds(0.5f);
        if(!(onGround == true || isDashing == true || Mathf.Abs(rb.velocity.x) <= 1))
            canControlSpring = true;
    }

    void kill(object sender, EventArgs e)
    {
        this.dead = true;
    }
}