using ShiroGe.Scripts.Items;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Item", menuName = "NewItem")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public ItemTypeEnum itemType;
    public string itemDescription;
    public Sprite icon;
    public int maxStackSize;
    [FormerlySerializedAs("itemPrefab")] public GameObject itemWorldPrefab;
    public GameObject itemPreviewPrefab;
    [FormerlySerializedAs("handItemPrefab")] public GameObject itemHandPrefab;
    public RecipeSO repice;
}
