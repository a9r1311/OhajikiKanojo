using UnityEngine;

public class Princess_Move : MonoBehaviour
{
    [Header("–Ú“I’n‚ÌÀ•W")]
    [SerializeField] public float coordinate = 10f;
    public float speed = 5f;

    void Update()
    {
        Vector3 targetPosition = new Vector3(0f, 1f, coordinate);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }
}
