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
    }
}