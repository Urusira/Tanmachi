using System;
using UnityEngine;

namespace ShiroGe.CharacterController
{
    [DefaultExecutionOrder(-2)]
    public class PlayerInstance : MonoBehaviour
    {
        public static PlayerInstance Instance { get; private set; }
        private PlayerController _pc;

        private void Start()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            //DontDestroyOnLoad(gameObject);

            Instance = this;
        }

        public void PlayerRegister(PlayerController pc)
        {
            _pc = pc;
        }

        public Vector3 PlayerWorldPosition()
        {
            return transform.position;
        }

        public Vector3 PlayerGroundedPointView()
        {
            return _pc.GlobalRaycastHit;
        }

        public Vector3 PlayerRawView()
        {
            return _pc.RawView;
        }
    }
}