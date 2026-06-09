using UnityEngine;

public class Camera_Self : MonoBehaviour
{
    public GameObject player;
    public float offset;
    void Update()
    {
        transform.position = new Vector2(player.transform.position.x, player.transform.position.y + offset);
    }
}
