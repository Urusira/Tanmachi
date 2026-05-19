using UnityEngine;

namespace ShiroGe.CharacterController
{
    public class PlayerEquipController : MonoBehaviour
    {
        public static PlayerEquipController Instance  { get; private set; }
        
        [SerializeField] private GameObject rightHandJoint;
        
        private bool hasRHEqupped = false;
        private GameObject RHEqupped;
        
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
        
        public void EquipRightHand(GameObject item)
        {
            hasRHEqupped = true;
            RHEqupped = Instantiate(item, rightHandJoint.transform);
        }

        public void UnequipRightHand()
        {
            if(hasRHEqupped)
            {
                hasRHEqupped = false;
                Destroy(RHEqupped);
            }
        }
    }
}