using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_mov : MonoBehaviour
{
    public float jumpForce, speed;
    public bool canMov;
    public int quantJumps;
    public Color[] playerColors;

    public float contador;

    bool inD, inE;
    public int jumps;
    bool inDash = false;

    private bool inGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z), Vector3.down, 0.2f, 1 << 8);
        if (hit.collider != null)
            return true;

        return false;
    }

    void Update()
    {
        if (inGround() == true && inDash == false)
        {
            contador = 0;
            jumps = 0;
            Jump();
        }
        else
        {
            contador += Time.deltaTime;
            if (contador < 0.15F && jumps == 0)
            {
                jumps = 0;
                Jump();
            }

            if (jumps <= quantJumps)
                Jump();
        }

        this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Input.GetAxisRaw("Horizontal"), this.gameObject.GetComponent<Rigidbody2D>().velocity.y);
        
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            Flip();
            this.gameObject.GetComponent<Animator>().Play("ANDANDO");

            if (Input.GetAxisRaw("Horizontal") > 0)
                inD = false;
            else if (Input.GetAxisRaw("Horizontal") < 0)
                inD = true;
        }
        else
            this.gameObject.GetComponent<Animator>().Play("PARADO");

        if (Input.GetKey(KeyCode.F))
        {
            inDash = true;
        }

        if (inDash == true)
        {
            contador += Time.deltaTime;
            if (contador >= 0.2f)
            {
                inDash = false;
            }
            this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
            this.gameObject.GetComponent<SpriteRenderer>().color = playerColors[0];
            GameObject.Find("dash").GetComponent<SpriteRenderer>().color = playerColors[1];
            this.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(400, 0));

        }
        else
        {
            this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 10;
            this.gameObject.GetComponent<SpriteRenderer>().color = playerColors[1];
            GameObject.Find("dash").GetComponent<SpriteRenderer>().color = playerColors[0];
        }
    }
    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(this.gameObject.GetComponent<Rigidbody2D>().velocity.x, +jumpForce);
            jumps++;
        }
    }
    void Flip()
    {
        if ((inD && !inE) || (!inD && inE))
        {
            inE = !inE;
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }
    }
}