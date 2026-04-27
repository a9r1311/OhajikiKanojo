using UnityEngine;

public class ZakoZakoEnemy : EnemyBase
{
    protected override void MovePatternWalk()
    {
        if (target == null) return;

        transform.LookAt(target.transform);
        transform.rotation =
            Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

        rb.linearVelocity = transform.forward * 3f; //速い雑魚とか
    }
}
