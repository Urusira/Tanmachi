using UnityEngine;
using UnityEngine.Serialization;

namespace ShiroGe.CharacterController
{
    public class PlayerStats : MonoBehaviour
    {
        public static PlayerStats Instance  { get; private set; }

        [field: SerializeField] public float ThrowForce { get; private set; } = 10f;
        [field: SerializeField] public float DropForce { get; private set; } = 5f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }
    }
}