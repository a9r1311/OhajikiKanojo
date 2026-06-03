using System.Collections;
using UnityEngine;

public class EnemyCycling : MonoBehaviour
{
    public void Rotation(Transform target)
    {
        StartCoroutine(Cycle(target));
    }

    IEnumerator Cycle(Transform target)
    {
        float timer = 0;

        float [] rotate = {180f, 180f, 180f};

        while (timer < 10.0f)
        {
            int rand = Random.Range(0, 3);
            rotate[rand] += Random.Range(-180f, 180f);
            rotate[rand] = Mathf.Clamp(rotate[rand], -180f, 540f);

            target.Rotate(rotate[0] * Time.deltaTime, rotate[1] * Time.deltaTime, rotate[2] * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        } 
    }
}
