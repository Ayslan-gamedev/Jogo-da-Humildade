using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newMov : MonoBehaviour
{
    public float speed, jumpForce, dashForce;

    public int jumps,movs,dashJump;
    private float coyoteTimer, dashTimer,timetoDash, directionX, directionY;
    private bool inD, inE, canMov = true;

    private bool InGround() // verifica se o player esta tocando no chão
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z), Vector3.down, 0.2f, 1 << 8);
        if (hit.collider != null)
        {
            coyoteTimer = 0;
            jumps = 0;
            return true;
        }
        
        return false;
    }

    private void Update()
    {
        if(canMov == true)
        {
            this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Input.GetAxisRaw("Horizontal"), this.gameObject.GetComponent<Rigidbody2D>().velocity.y);
            if(Input.GetAxisRaw("Horizontal") == 0)
                directionX = 0;
        }
        else
        {
            dashTimer += Time.deltaTime;
            if (dashTimer > 0.7f)
            {
                this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 10;
                canMov = true;
                dashTimer = 0;
            }
            else
            {
                this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(dashForce * directionX, dashForce * directionY);
            }
        }

        if(Input.GetAxisRaw("Horizontal") != 0 && canMov == true)
        {
            Flip();
            directionX = Input.GetAxisRaw("Horizontal");
            if (Input.GetAxisRaw("Horizontal") > 0)
                inD = false;
            else if (Input.GetAxisRaw("Horizontal") < 0)
                inD = true;

            if(movs == 2 && timetoDash < 0.05f)
            {
                canMov = false;
                timetoDash = 0;
                movs = 0;
            }
        }
        else
            timetoDash += Time.deltaTime;

        if (timetoDash > 0.05f)
        {
            timetoDash = 0;
            movs = 0;
        }

        if (Input.GetAxisRaw("Vertical") != 0)
        {
            directionY = Input.GetAxisRaw("Vertical");
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                if (dashJump == 1)
                {
                    canMov = false;
                    dashJump = 0;
                }
            }
            else if (Input.GetAxisRaw("Vertical") < 0)
                dashJump = 1;
        }
        else if(canMov == true)
            directionY = 0;

        if (Input.GetKeyDown(KeyCode.R) && canMov == true)
            canMov = false;

        if (InGround() == false && jumps == 0)
            coyoteTimer += Time.deltaTime;

        if (Input.GetButtonDown("Jump") && (InGround() == true || coyoteTimer < 0.13f))
        {
            if (InGround() == false)
                this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce + 2), ForceMode2D.Impulse);
            else
                this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

            jumps++;
        }
    }

    private void Flip()
    {
        if ((inD && !inE) || (!inD && inE))
        {
            inE = !inE;
            movs += 1;
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }
    }
}