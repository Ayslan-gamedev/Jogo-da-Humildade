using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player_script : MonoBehaviour
{
    public float startspeed, speedMax, jumpForce, dashForce;
    private float speed, timerDash,coyoteTimer, directX, directY;
    private bool inL, inR, inDash;
    private int jumps;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        this.transform.position = new Vector3(0, 0, 0);
    }

    private void FixedUpdate()
    {
        this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Input.GetAxisRaw("Horizontal"), this.gameObject.GetComponent<Rigidbody2D>().velocity.y);

        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            Flip();
            if (Input.GetAxisRaw("Horizontal") > 0) inL = false;
            else inL = true;

            if (speed < 0.50f) speed = startspeed;
            if (speed <= speedMax) speed += 0.25F;
        }
        else
        {
            if(speed > 0) speed -= 2;
            if (speed < 0) speed = 0;
        }

        // DASH

        if (Input.GetButtonDown("Fire1") && inDash == false)
        {
            if(Input.GetAxisRaw("Horizontal") > 0)
                directX = 1;
            else if (Input.GetAxisRaw("Horizontal") < 0)
                directX = -1;
            else
                directX = 0;

            if (Input.GetAxisRaw("Vertical") > 0)
                directY = 1;
            else if (Input.GetAxisRaw("Vertical") < 0)
                directY = -1;
            else
                directY = 0;

            inDash = true;
        }

        if (inDash == true)
        {
            this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
            this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(dashForce * directX, dashForce * directY);
            timerDash += Time.deltaTime;
            speed = 0;

            if (timerDash >= 0.2f)
            {
                timerDash = 0;
                inDash = false;
            }
        }
        else
        {
            this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 10;

            directX = 0;
            directY = 0;
        }

        // jump
        if (InGround() != true)
            coyoteTimer += Time.deltaTime;
        else
            coyoteTimer = 0;
       
        if (Input.GetButton("Fire3") && (InGround() == true || coyoteTimer <= 0.2f && jumps == 0))
        {
            this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumps++;
        }
    }

    private bool InGround() // verifica se o player esta tocando no chão
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z), Vector3.down, 0.2f, 1 << 8);
        if (hit.collider != null)
        {
            jumps = 0;
            return true;
        }

        return false;
    }

    private void Flip()
    {
        if ((inL && !inR) || (!inL && inR))
        {
            inR = !inR;
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }
    }
}