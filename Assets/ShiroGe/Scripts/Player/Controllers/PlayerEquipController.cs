using System.Collections.Generic;
using ShiroGe.Scripts.Items;
using UnityEngine;

namespace ShiroGe.CharacterController
{
    public class PlayerEquipController : MonoBehaviour
    {
        public static PlayerEquipController Instance  { get; private set; }
        
        [SerializeField] private GameObject rightHandJoint;
        
        [SerializeField] private GameObject playerAvatar;

        [SerializeField] private GameObject standartPlayerUnderwear;
        [SerializeField] private GameObject playerHair;
        
        [SerializeField] private GameObject standartPlayerAvatarUnderwear;
        [SerializeField] private GameObject playerAvatarHair;
        
        //private readonly Dictionary<ItemTypeEnum, GameObject> _equppedEquipment = new Dictionary<ItemTypeEnum, GameObject>();
        
        private bool hasRHEqupped = false;
        private GameObject RHEqupped;
        
        private bool hasHeadEqupped = false;
        private GameObject HeadEqupped;
        
        private bool hasBodyEqupped = false;
        private GameObject BodyEqupped;
        
        private bool hasLegsEqupped = false;
        private GameObject LegsEqupped;
        
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
                itemEquppedPrefab = item.handItemPrefab;
            }
            
            //TODO: Перейти на кешированный массив. Доработать логику так, чтобы источником истины был массив. Пока что заглушка.
            //_equppedEquipment[type] = itemEquppedPrefab;
            
            switch (type)
            {
                case ItemTypeEnum.DEFAULT:
                {
                    UnequipRightHand();
                    if(item != null) EquipRightHand(item.handItemPrefab);
                    break;
                }
                case ItemTypeEnum.HEADWEAR:
                {
                    UnequipHead(item.handItemPrefab);
                    if(item != null) EquipHead(item.handItemPrefab);
                    break;
                }
                case ItemTypeEnum.BODYWEAR:
                {
                    UnequipBody(item.handItemPrefab);
                    if(item != null) EquipBody(item.handItemPrefab);
                    break;
                }
                case ItemTypeEnum.LEGSWEAR:
                {
                    UnequipLegs(item.handItemPrefab);
                    if(item != null) EquipLegs(item.handItemPrefab);
                    break;
                }
                case ItemTypeEnum.WEAPON:
                {
                    UnequipRightHand();
                    if(item != null) EquipRightHand(item.handItemPrefab);
                    break;
                }
                case ItemTypeEnum.TOOL:
                {
                    UnequipRightHand();
                    if(item != null) EquipRightHand(item.handItemPrefab);
                    break;
                }
                case ItemTypeEnum.CONSUMABLE:
                {
                    UnequipRightHand();
                    if(item != null) EquipRightHand(item.handItemPrefab);
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
                    UnequipHead(item.handItemPrefab);
                    break;
                }
                case ItemTypeEnum.BODYWEAR:
                {
                    UnequipBody(item.handItemPrefab);
                    break;
                }
                case ItemTypeEnum.LEGSWEAR:
                {
                    UnequipLegs(item.handItemPrefab);
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
            }
        }

        public void TakeInHand(ItemSO item)
        {
            UnequipRightHand();
            if(item != null) EquipRightHand(item.handItemPrefab);
        }
        
        
        private void EquipRightHand(GameObject item)
        {
            hasRHEqupped = true;
            RHEqupped = Instantiate(item, rightHandJoint.transform);
        }

        private void UnequipRightHand()
        {
            if(hasRHEqupped)
            {
                hasRHEqupped = false;
                Destroy(RHEqupped);
            }
        }

        
        private void EquipHead(GameObject item)
        {
            playerHair.SetActive(false);
            playerAvatarHair.SetActive(false);
            playerAvatarHair.SetActive(false);
            item.GetComponent<HeadWear>().Equip(gameObject);
            item.GetComponent<HeadWear>().Equip(playerAvatar);
            HeadEqupped = item;
            hasHeadEqupped = true;
        }
        
        private void UnequipHead(GameObject item)
        {
            playerHair.SetActive(true);
            playerAvatarHair.SetActive(true);
            item.GetComponent<HeadWear>().Unequip(gameObject);
            item.GetComponent<HeadWear>().Unequip(playerAvatar);
            HeadEqupped = null;
            hasHeadEqupped = false;
        }

        private void EquipBody(GameObject item)
        {
            item.GetComponent<BodyWear>().Equip(gameObject);
            item.GetComponent<BodyWear>().Equip(playerAvatar);
            BodyEqupped = item;
            hasBodyEqupped = true;
        }
        
        private void UnequipBody(GameObject item)
        {
            item.GetComponent<BodyWear>().Unequip(gameObject);
            item.GetComponent<BodyWear>().Unequip(playerAvatar);
            BodyEqupped = null;
            hasBodyEqupped = false;
        }

        private void EquipLegs(GameObject item)
        {
            standartPlayerUnderwear.SetActive(false);
            standartPlayerAvatarUnderwear.SetActive(false);
            item.GetComponent<LegsWear>().Equip(gameObject);
            item.GetComponent<LegsWear>().Equip(playerAvatar);
            LegsEqupped = item;
            hasLegsEqupped = true;
        }
        
        private void UnequipLegs(GameObject item)
        {
            standartPlayerUnderwear.SetActive(true);
            standartPlayerAvatarUnderwear.SetActive(true);
            item.GetComponent<LegsWear>().Unequip(gameObject);
            item.GetComponent<LegsWear>().Unequip(playerAvatar);
            LegsEqupped = null;
            hasLegsEqupped = false;
        }
    }
}