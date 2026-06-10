using System.Collections.Generic;
using ShiroGe.Scripts.Items;
using UnityEngine;

namespace ShiroGe.CharacterController
{
    [DefaultExecutionOrder(1)]
    public class PlayerEquipController : MonoBehaviour
    {
        public static PlayerEquipController Instance  { get; private set; }
        
        [SerializeField] private GameObject rightHandJoint;
        
        [SerializeField] private GameObject playerAvatar;

        [SerializeField] private GameObject standartPlayerUnderwear;
        [SerializeField] private GameObject playerHair;
        
        [SerializeField] private GameObject standartPlayerAvatarUnderwear;
        [SerializeField] private GameObject playerAvatarHair;
        
        [SerializeField] private ObjectPlacer objectPlacer;
        
        //private readonly Dictionary<ItemTypeEnum, GameObject> _equppedEquipment = new Dictionary<ItemTypeEnum, GameObject>();
        
        private bool hasRHEqupped = false;
        private GameObject RHEquppedObj;
        private ItemSO RHEqupped;
        
        private bool hasHeadEqupped = false;
        private GameObject HeadEquppedObj;
        private ItemSO HeadEqupped;
        
        private bool hasBodyEqupped = false;
        private GameObject BodyEquppedObj;
        private ItemSO BodyEqupped;
        
        private bool hasLegsEqupped = false;
        private GameObject LegsEquppedObj;
        private ItemSO LegsEqupped;
        
        private ItemTypeEnum handlingItemType;
        
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

        public void EquipItem(ItemSO item)
        {
            ItemTypeEnum type = ItemTypeEnum.DEFAULT;
            GameObject itemEquppedPrefab;
            
            if(item != null)
            {
                type = item.itemType;
                itemEquppedPrefab = item.itemHandPrefab;
            }
            
            //TODO: Перейти на кешированный массив. Доработать логику так, чтобы источником истины был массив. Пока что заглушка.
            //_equppedEquipment[type] = itemEquppedPrefab;
            
            switch (type)
            {
                case ItemTypeEnum.DEFAULT:
                {
                    UnequipRightHand();
                    handlingItemType = item.itemType;
                    if(item != null) EquipRightHand(item);
                    break;
                }
                case ItemTypeEnum.HEADWEAR:
                {
                    UnequipHead(item);
                    if(item != null) EquipHead(item);
                    break;
                }
                case ItemTypeEnum.BODYWEAR:
                {
                    UnequipBody(item);
                    if(item != null) EquipBody(item);
                    break;
                }
                case ItemTypeEnum.LEGSWEAR:
                {
                    UnequipLegs(item);
                    if(item != null) EquipLegs(item);
                    break;
                }
                case ItemTypeEnum.WEAPON:
                {
                    UnequipRightHand();
                    handlingItemType = item.itemType;
                    if(item != null) EquipRightHand(item);
                    break;
                }
                case ItemTypeEnum.TOOL:
                {
                    UnequipRightHand();
                    handlingItemType = item.itemType;
                    if(item != null) EquipRightHand(item);
                    break;
                }
                case ItemTypeEnum.CONSUMABLE:
                {
                    UnequipRightHand();
                    handlingItemType = item.itemType;
                    if(item != null) EquipRightHand(item);
                    break;
                }
                case ItemTypeEnum.PLACEABLE:
                {
                    UnequipRightHand();
                    handlingItemType = item.itemType;
                    if(item != null)
                    {
                        EquipRightHand(item);
                        objectPlacer.ObjectSet(item.itemWorldPrefab, item.itemPreviewPrefab, item);
                        objectPlacer.EnterPlacementMode();
                    }
                    break;
                }
            }
        }
        
        public void UnequipItem(ItemSO item)
        {
            ItemTypeEnum type = ItemTypeEnum.DEFAULT;
            
            if(item != null)
            {
                type = item.itemType;
            }
            
            switch (type)
            {
                case ItemTypeEnum.DEFAULT:
                {
                    UnequipRightHand();
                    break;
                }
                case ItemTypeEnum.HEADWEAR:
                {
                    UnequipHead(item);
                    break;
                }
                case ItemTypeEnum.BODYWEAR:
                {
                    UnequipBody(item);
                    break;
                }
                case ItemTypeEnum.LEGSWEAR:
                {
                    UnequipLegs(item);
                    break;
                }
                case ItemTypeEnum.WEAPON:
                {
                    UnequipRightHand();
                    break;
                }
                case ItemTypeEnum.TOOL:
                {
                    UnequipRightHand();
                    break;
                }
                case ItemTypeEnum.CONSUMABLE:
                {
                    UnequipRightHand();
                    break;
                }
                case ItemTypeEnum.PLACEABLE:
                {
                    UnequipRightHand();
                    if(item != null)
                    {
                        objectPlacer.ExitPlacementMode();
                    }
                    break;
                }
            }
        }

        public void TakeInHand(ItemSO item)
        {
            UnequipRightHand();
            
            if(item != null)
            {
                handlingItemType = item.itemType;
            
                EquipRightHand(item);
                
                if (item.itemType == ItemTypeEnum.PLACEABLE)
                {
                    objectPlacer.ObjectSet(item.itemWorldPrefab, item.itemPreviewPrefab, item);
                    objectPlacer.EnterPlacementMode();
                }
            }
        }
        
        private void EquipRightHand(ItemSO item)
        {
            hasRHEqupped = true;
            RHEquppedObj = Instantiate(item.itemHandPrefab, rightHandJoint.transform);
            RHEqupped = item;
        }

        private void UnequipRightHand()
        {
            if (handlingItemType == ItemTypeEnum.PLACEABLE)
            {
                objectPlacer.ExitPlacementMode();
            }
            if(hasRHEqupped)
            {
                hasRHEqupped = false;
                Destroy(RHEquppedObj);
                RHEqupped = null;
            }
        }

        
        private void EquipHead(ItemSO item)
        {
            playerHair.SetActive(false);
            playerAvatarHair.SetActive(false);
            playerAvatarHair.SetActive(false);
            HeadEquppedObj = item.itemHandPrefab;
            HeadEquppedObj.GetComponent<HeadWear>().Equip(gameObject);
            HeadEquppedObj.GetComponent<HeadWear>().Equip(playerAvatar);
            HeadEqupped = item;
            hasHeadEqupped = true;
        }
        
        private void UnequipHead(ItemSO item)
        {
            playerHair.SetActive(true);
            playerAvatarHair.SetActive(true);
            item.itemHandPrefab.GetComponent<HeadWear>().Unequip(gameObject);
            item.itemHandPrefab.GetComponent<HeadWear>().Unequip(playerAvatar);
            HeadEquppedObj = null;
            HeadEqupped = null;
            hasHeadEqupped = false;
        }

        private void EquipBody(ItemSO item)
        {
            BodyEquppedObj = item.itemHandPrefab;
            BodyEquppedObj.GetComponent<BodyWear>().Equip(gameObject);
            BodyEquppedObj.GetComponent<BodyWear>().Equip(playerAvatar);
            BodyEqupped = item;
            hasBodyEqupped = true;
        }
        
        private void UnequipBody(ItemSO item)
        {
            item.itemHandPrefab.GetComponent<BodyWear>().Unequip(gameObject);
            item.itemHandPrefab.GetComponent<BodyWear>().Unequip(playerAvatar);
            BodyEquppedObj = null;
            BodyEqupped = null;
            hasBodyEqupped = false;
        }

        private void EquipLegs(ItemSO item)
        {
            standartPlayerUnderwear.SetActive(false);
            standartPlayerAvatarUnderwear.SetActive(false);
            LegsEquppedObj = item.itemHandPrefab;
            LegsEquppedObj.GetComponent<LegsWear>().Equip(gameObject);
            LegsEquppedObj.GetComponent<LegsWear>().Equip(playerAvatar);
            LegsEqupped = item;
            hasLegsEqupped = true;
        }
        
        private void UnequipLegs(ItemSO item)
        {
            standartPlayerUnderwear.SetActive(true);
            standartPlayerAvatarUnderwear.SetActive(true);
            item.itemHandPrefab.GetComponent<LegsWear>().Unequip(gameObject);
            item.itemHandPrefab.GetComponent<LegsWear>().Unequip(playerAvatar);
            LegsEquppedObj = null;
            LegsEqupped = null;
            hasLegsEqupped = false;
        }
    }
}