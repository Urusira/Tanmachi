using System.ComponentModel;
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

        public PickupbleItem PlayerDrop(GameObject item, float throwForce = 0f, bool needInterpolate = false, int amount = 1)
        {
            Vector3 spawnPosition;
            Quaternion spawnRotation = Quaternion.LookRotation(PlayerInstance.Instance.GetPlayerForward());
    
            Vector3 groundPoint = PlayerInstance.Instance.GetPlayerGroundedPointView();
            if (groundPoint != Vector3.zero)
            {
                spawnPosition = groundPoint;
            }
            else
            {
                spawnPosition = PlayerInstance.Instance.GetPlayerRawStartPoint();
            }
    
            GameObject droppedObject = Instantiate(item, spawnPosition, spawnRotation);
            PickupbleItem droppedItem = droppedObject.GetComponent<PickupbleItem>();
            
            if (droppedItem == null)
            {
                LODGroup lod = droppedObject.GetComponent<LODGroup>();
                if (lod != null)
                {
                    droppedItem = droppedObject.GetComponentInChildren<PickupbleItem>();
                }
                
                if(droppedItem == null) throw new WarningException("WorldSpawner dropped item and LOD's haven't pickupable script");
            }
            
            droppedItem.amount = amount;
    
            if (throwForce > 0f)
            {
                Rigidbody rb = droppedObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    if(needInterpolate) rb.interpolation = RigidbodyInterpolation.Interpolate;
                    Vector3 throwDirection = PlayerInstance.Instance.GetPlayerForward();
                    rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
                }
            
            }
    
            return droppedItem;
        }
    }
}