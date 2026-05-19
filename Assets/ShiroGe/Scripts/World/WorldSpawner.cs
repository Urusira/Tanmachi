using ShiroGe.CharacterController;
using ShiroGe.Scripts.Items;
using UnityEngine;

namespace ShiroGe.Scripts.World
{
    public class WorldSpawner : MonoBehaviour
    {
        public static WorldSpawner Instance { get; private set; }

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

        public PickapbleItem PlayerDrop(GameObject item)
        {
            Vector3 viewPoint = PlayerInstance.Instance.PlayerGroundedPointView();
            if(viewPoint != Vector3.zero)
            {
                return Instantiate(item, viewPoint, new Quaternion()).GetComponent<PickapbleItem>();
            }
            return Instantiate(item, PlayerInstance.Instance.PlayerRawView(),
                new Quaternion()).GetComponent<PickapbleItem>();
        }
    }
}