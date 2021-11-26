using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newMov : MonoBehaviour
{
    public float speed, jumpForce, dashForce;

    private bool inD, inE;

    private bool InGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z), Vector3.down, 0.2f, 1 << 8);
        if (hit.collider != null)
            return true;

        return false;
    }

    private void Update()
    {
        this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(speed * Input.GetAxisRaw("Horizontal"), this.gameObject.GetComponent<Rigidbody2D>().velocity.y);
        
        if(Input.GetAxisRaw("Horizontal") != 0)
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
                inD = false;
            else if (Input.GetAxisRaw("Horizontal") < 0)
                inD = true;
        }
    
    }

    private void Flip()
    {
        if ((inD && !inE) || (!inD && inE))
        {
            inE = !inE;
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }
    }
}