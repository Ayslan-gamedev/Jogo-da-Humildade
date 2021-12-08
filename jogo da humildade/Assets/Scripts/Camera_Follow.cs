using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Follow : MonoBehaviour
{
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.Find("player").GetComponent<Transform>().position.x - this.gameObject.transform.position.x - 4 < 4)
        {
            this.gameObject.transform.position = new Vector2(+1 * Time.deltaTime, this.transform.position.x);
        }
    }
}
