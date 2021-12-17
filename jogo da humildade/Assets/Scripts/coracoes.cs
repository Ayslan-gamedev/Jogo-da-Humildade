using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))] // o script só ira fucionar com esse componente
public class coracoes : MonoBehaviour
{
    public string typeOfHeart;
    public float lifeAdd, maxLifeAdd;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            switch (typeOfHeart) 
            {
                case "changeLife":
                    GameObject.Find("GameControll").GetComponent<Player_script>().ChangeLife(lifeAdd, maxLifeAdd);
                    break;

                case "none":
                    break;
            }
            Destroy(this.gameObject);
        }
    }
}