using UnityEngine;

public class camera_follow : MonoBehaviour
{
    public Transform[] limits;
    public Transform center;
    private Transform player;

    float camPos;

    void Start()
    {
        if (GameObject.Find("player") != null)
            player = GameObject.Find("player").GetComponent<Transform>();

    }

    void FixedUpdate()
    {
        camPos = center.transform.position.x;
        if (GameObject.Find("player") != null)
        {
            float playerDistanceX = player.position.x - camPos;

            if (playerDistanceX > 3.5f || playerDistanceX < -3.5f)
            {
                if(camPos > limits[0].transform.position.x && camPos < limits[1].transform.position.x)
                    center.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(playerDistanceX, center.gameObject.GetComponent<Rigidbody2D>().velocity.y);
                else
                {
                    if(playerDistanceX > 0 && camPos <= limits[0].transform.position.x || playerDistanceX < 0 && camPos >= limits[1].transform.position.x)
                        center.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(playerDistanceX, center.gameObject.GetComponent<Rigidbody2D>().velocity.y);
                    else
                        center.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, center.gameObject.GetComponent<Rigidbody2D>().velocity.y);
                }
            }
            else
                center.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, center.gameObject.GetComponent<Rigidbody2D>().velocity.y);

        }
    }
}