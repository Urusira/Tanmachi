using System.Collections.Generic;
using ShiroGe.Scripts.Enums;
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

            Instance = this;
        }

        public void EquipItem(ItemSO item)
        {
            //ItemTypeEnum type = ItemTypeEnum.DEFAULT;
            WearableType wearingType = WearableType.NonWearable;
            
            if(item != null)
            {
                //type = item.itemType;
                wearingType = item.itemWearType;
            }
            
            //TODO: Перейти на кешированный массив. Доработать логику так, чтобы источником истины был массив. Пока что заглушка.

            if (wearingType != WearableType.NonWearable)
            {
                switch (wearingType)
                {
                    case WearableType.Headwear:
                    {
                        UnequipHead(item);
                        if(item != null) EquipHead(item);
                        break;
                    }
                    case WearableType.Bodywear:
                    {
                        UnequipBody(item);
                        if(item != null) EquipBody(item);
                        break;
                    }
                    case WearableType.Legswear:
                    {
                        UnequipLegs(item);
                        if(item != null) EquipLegs(item);
                        break;
                    }
                }
            }
            /*else
            {
                switch (type)
                {
                    case ItemTypeEnum.DEFAULT:
                    {
                        UnequipRightHand();
                        handlingItemType = item.itemType;
                        if (item != null) EquipRightHand(item);
                        break;
                    }
                    case ItemTypeEnum.WEAPON:
                    {
                        UnequipRightHand();
                        handlingItemType = item.itemType;
                        if (item != null) EquipRightHand(item);
                        break;
                    }
                    case ItemTypeEnum.TOOL:
                    {
                        UnequipRightHand();
                        handlingItemType = item.itemType;
                        if (item != null) EquipRightHand(item);
                        break;
                    }
                    case ItemTypeEnum.CONSUMABLE:
                    {
                        UnequipRightHand();
                        handlingItemType = item.itemType;
                        if (item != null) EquipRightHand(item);
                        break;
                    }
                    case ItemTypeEnum.PLACEABLE:
                    {
                        UnequipRightHand();
                        handlingItemType = item.itemType;
                        if (item != null)
                        {
                            EquipRightHand(item);
                            objectPlacer.ObjectSet(item.placeableBuildPrefab, item.itemPreviewPrefab, item);
                            objectPlacer.EnterPlacementMode();
                        }

                        break;
                    }
                }
            }*/
        }
        
        public void UnequipItem(ItemSO item)
        {
            //ItemTypeEnum type = ItemTypeEnum.DEFAULT;
            WearableType wearingType = WearableType.NonWearable;
            
            if(item != null)
            {
                //type = item.itemType;
                wearingType = item.itemWearType;
            }

            if(wearingType != WearableType.NonWearable)
            {
                switch (wearingType)
                {
                    case WearableType.Headwear:
                    {
                        UnequipHead(item);
                        break;
                    }
                    case WearableType.Bodywear:
                    {
                        UnequipBody(item);
                        break;
                    }
                    case WearableType.Legswear:
                    {
                        UnequipLegs(item);
                        break;
                    }
                }
            }
            /*else
            {
                switch (type)
                {
                    case ItemTypeEnum.DEFAULT:
                    {
                        UnequipRightHand();
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
                        if (item != null)
                        {
                            objectPlacer.ExitPlacementMode();
                        }

                        break;
                    }
                }
            }*/
        }

        public void TakeInHand(ItemSO item)
        {
            UnequipRightHand();
            
            if(item != null)
            {
                handlingItemType = item.itemType;
            
                EquipRightHand(item);
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