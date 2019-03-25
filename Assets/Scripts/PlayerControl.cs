using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerControl : MonoBehaviour {

    private Rigidbody2D rb;
    private BoxCollider2D box;
    private Vector3 velocity;
    private Animator animator;
    private SpriteRenderer sr;
    public ParticleSystem dash;
    public GameObject phantom;

    public bool canJump;              //can the player jump now?
    private bool onWall;
    private bool backWall;
    private bool canControlJump;
    private float fallGravity = 4f;  //control gravity when jumping
    private float lowGravity = 3.25f;    //control gravity when jumping

    public bool canDash;              //can the player dash now?
    public bool dead;
    public bool canControlMove;
    private Vector2 dashDirection;
    public float dashTime = 0;
    public float dashCooldown = 0;
    private float spawntime = 0.05f;

    public int currentLevel = 0;

    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        box = this.GetComponent<BoxCollider2D>();
        animator = this.GetComponent<Animator>();
        sr = this.GetComponent<SpriteRenderer>();
        dashDirection = new Vector2(0, 0);
        canJump = false;
        canDash = true;
        canControlJump = true;
        canControlMove = true;
        onWall = false;
        backWall = false;
        rb.gravityScale = 3f;
    }

    // Update is called once per frame
    private void Update()
    {

        /* Check whether the player is dead or not*/
        if (dead)
        {
            animator.SetBool("Dead", dead);
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            dashTime = 0;
            return;
        }
        else
        {
            animator.SetBool("Dead", dead);
            rb.gravityScale = 3f;
        }  

        /* Check collision */
        Vector2 leftfoot, rightfoot, wallVector;  
        leftfoot = new Vector2(rb.transform.position.x - 0.3f, rb.transform.position.y - 1.01f);
        rightfoot = new Vector2(rb.transform.position.x + 0.3f, rb.transform.position.y - 1.01f);
        RaycastHit2D lefthit = Physics2D.Raycast(leftfoot, Vector2.down,0.04f), righthit = Physics2D.Raycast(rightfoot, Vector2.down, 0.04f);
        if ((lefthit.collider != null && lefthit.collider.isTrigger == false) || (righthit.collider != null && righthit.collider.isTrigger == false))
        {
            canJump = true;
            canControlMove = true;
            if (dashTime == 0)
                canDash = true;
        }
        else
            canJump = false;
        wallVector = (sr.flipX == false) ? Vector2.right : Vector2.left;
        RaycastHit2D wallhit = Physics2D.Raycast(this.transform.position, wallVector, 0.6f), backwall = Physics2D.Raycast(this.transform.position, -wallVector, 0.6f);
        onWall = (wallhit.collider != null && wallhit.collider.isTrigger == false) ? true : false;
        backWall = (backwall.collider != null && backwall.collider.isTrigger == false) ? true : false;

        /* Control moving and jumping */
        velocity = rb.velocity;

        if(dashTime == 0)
        {
            if (Input.GetKey(KeyCode.L) && onWall == true)
            {
                velocity.x = 0;
                rb.gravityScale = 0;
                if (Input.GetKey(KeyCode.W))
                    velocity.y = 3f;
                else if (Input.GetKey(KeyCode.S))
                    velocity.y = -5f;
                else
                    velocity.y = 0;
                canControlMove = false;
            }
            else if(Input.GetKeyUp(KeyCode.L) && onWall == true)
            {
                rb.gravityScale = 3;
                canControlMove = true;
            }
            else if (canControlMove == true)
            {
                if (Input.GetKey(KeyCode.D))
                    velocity.x = 7.5f;
                else if (Input.GetKey(KeyCode.A))
                    velocity.x = -7.5f;
                else
                    velocity.x = rb.velocity.x * 0.8f;
            }
            else
            {
                if (Input.GetKey(KeyCode.D) && velocity.x <= 7.5f)
                    velocity.x += 0.75f;
                else if (Input.GetKey(KeyCode.A) && velocity.x >= -7.5f)
                    velocity.x -= 0.75f;
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                if(canJump == true)
                {
                    velocity.y = 13f;
                    canJump = false;
                    canControlJump = true;
                }
                else if(onWall == true || backWall == true)
                {
                    canControlMove = false;
                    canControlJump = true;
                    velocity.y = 13f;
                    if(!Input.GetKey(KeyCode.L))
                    {
                        if ((sr.flipX == false && onWall == true) || (sr.flipX == true && backWall == true))
                            velocity.x = -7.5f;
                        else
                            velocity.x = 7.5f;
                    }
                }
            }
            rb.velocity = velocity;
        }
        
        /* Contorl dashing*/ 
        if (Input.GetKeyDown(KeyCode.J) && dashTime == 0 && canDash == true && dashCooldown >= 0)
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
            dashTime += 0.01f;
            canControlMove = true;
            canControlJump = false;
        }

        if (canDash == true && dashTime >= 0.01f && dashCooldown >= 0) //check if player press the dash button and can dash or not
        {
            if(spawntime >= 0.05f)
            {
                GameObject temp = Instantiate(phantom,this.transform.position,Quaternion.identity);
                temp.GetComponent<SpriteRenderer>().sprite = sr.sprite;
                temp.GetComponent<SpriteRenderer>().flipX = sr.flipX;
                temp.transform.rotation = this.transform.rotation;
                spawntime = 0;
            }
            rb.velocity = dashDirection.normalized * 35;
            dashTime += Time.deltaTime;
            spawntime += Time.deltaTime;
            if (dashTime >= 0.15f)
            {
                canDash = false;
                rb.velocity = dashDirection.normalized * 10;
                dashTime = 0;
                dashCooldown = -0.2f;
                spawntime = 0.05f;
            }
        }

        if(dashCooldown <= 0)
            dashCooldown += Time.deltaTime;

        /* Control animation and sprite flip */

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

        /* This part change the gravity of the player to make the jump seems less "floaty" */
        if (rb.velocity.y > 13)
            canControlJump = false;
        if(canJump == true)
            canControlJump = true;
        if (rb.velocity.y < -35)
            rb.velocity = new Vector2(rb.velocity.x, -35);
        if (canControlJump == true)
        {
            if (rb.velocity.y < 0)
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallGravity) * Time.deltaTime;
            else if (rb.velocity.y > 0 && (!Input.GetKey(KeyCode.K)))
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowGravity) * Time.deltaTime;
        }
        else
        {
            if(!(onWall == true && Input.GetKey(KeyCode.L)))
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowGravity) * Time.deltaTime;
        }
    }
}