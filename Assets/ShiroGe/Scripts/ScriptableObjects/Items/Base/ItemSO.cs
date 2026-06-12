using ShiroGe.Scripts.Enums;
using ShiroGe.Scripts.Items;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Item", menuName = "NewItem")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public ItemTypeEnum itemType;
    public WearableType itemWearType;
    public string itemDescription;
    public Sprite icon;
    public int maxStackSize;
    public GameObject itemWorldPrefab;
    public GameObject itemPreviewPrefab;
    public GameObject itemHandPrefab;
    public GameObject placeableBuildPrefab;
    public RecipeSO repice;
}
