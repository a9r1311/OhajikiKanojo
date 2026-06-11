using UnityEngine;

namespace Ohajiki.Core
{
    public class CharacterMove : MonoBehaviour
    {
        [SerializeField] FollowTargetUI target;
        public GameObject prince;
        float moveSpd = 12f;
        private CharacterController controller;

        void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        void Update()
        {
            if (target.explaind)
            {
                transform.position += Vector3.forward * moveSpd * Time.deltaTime;
                //Vector3 moveDirection = Vector3.forward;
                //moveDirection.y = -2f;
                //controller.Move(moveDirection * moveSpd * Time.deltaTime);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                EnemyBase enemyBase = other.transform.parent.gameObject.GetComponent<EnemyBase>();

                //敵が死んでいるかどうかを確認
                if (!enemyBase.isDead)
                {
                    //敵の状態をリセット
                    enemyBase.ResetState();

                    //敵をオブジェクトプールに返す
                    enemyBase.spawnDirector.ReturnEnemyToPool(other.transform.parent.gameObject, enemyBase.id);
                    enemyBase.spawnDirector.RemoveActiveEnemy(other.transform.parent.gameObject);
                }
            }
        }
    }
}