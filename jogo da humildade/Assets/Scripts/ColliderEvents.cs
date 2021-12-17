using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))] // o script só ira fucionar com esse componente
public class ColliderEvents : MonoBehaviour
{
    public string typeOfEvent;

    public float lifeAdd, maxLifeAdd;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            switch (typeOfEvent) 
            {
                case "changeLife":
                    GameObject.Find("GameControll").GetComponent<Player_script>().ChangeLife(lifeAdd, maxLifeAdd);
                    Destroy(this.gameObject);
                    break;

                case "damage":
                    GameObject.Find("GameControll").GetComponent<Player_script>().ChangeLife(lifeAdd, maxLifeAdd);
                    break;

                case "wentIntoLimbo":
                    GameObject.Find("GameControll").GetComponent<Player_script>().ChangeLife(lifeAdd, maxLifeAdd);
                    GameObject.Find("GameControll").GetComponent<Player_script>().returnToGround();
                    break;

                case "saveGame":
                    GameObject.Find("AutoSave").GetComponent<Animator>().Play("autoSave");
                    GameObject.Find("GameControll").GetComponent<Player_script>().Save(GameObject.Find("GameControll").GetComponent<Player_script>().theSave);
                    GameObject.Find("GameControll").GetComponent<Player_script>().Save(GameObject.Find("GameControll").GetComponent<Player_script>().theSave);
                    GameObject.Find("GameControll").GetComponent<Player_script>().Save(GameObject.Find("GameControll").GetComponent<Player_script>().theSave);
                    GameObject.Find("GameControll").GetComponent<Player_script>().Save(GameObject.Find("GameControll").GetComponent<Player_script>().theSave);
                    GameObject.Find("GameControll").GetComponent<Player_script>().Save(GameObject.Find("GameControll").GetComponent<Player_script>().theSave);
                    GameObject.Find("GameControll").GetComponent<Player_script>().Save(GameObject.Find("GameControll").GetComponent<Player_script>().theSave);
                    GameObject.Find("GameControll").GetComponent<Player_script>().Save(GameObject.Find("GameControll").GetComponent<Player_script>().theSave);
                    break;
            }
        }
    }
}