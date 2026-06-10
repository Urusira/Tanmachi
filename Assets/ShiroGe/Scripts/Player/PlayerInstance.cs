using System;
using ShiroGe.Scripts.Inventory;
using UnityEngine;

namespace ShiroGe.CharacterController
{
    [DefaultExecutionOrder(-2)]
    public class PlayerInstance : MonoBehaviour
    {
        public static PlayerInstance Instance { get; private set; }
        private PlayerController _pc;
        private CashManager _cashManager;

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

        public void PlayerRegister(PlayerController pc, CashManager cashManager)
        {
            _pc = pc;
            _cashManager = cashManager;
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

        public void GiveCash(int amount)
        {
            _cashManager.AddCash(amount);
        }
    }
}