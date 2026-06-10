using ShiroGe.CharacterController;
using ShiroGe.Scripts;
using ShiroGe.Scripts.NPC;
using ShiroGe.Scripts.Tavern;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TavernOpeningSign : Interactable
{
    [SerializeField] public GameObject signFrontTextObj;
    [SerializeField] public GameObject signBackTextObj;
    
    private TextMeshProUGUI signFrontText;
    private TextMeshProUGUI signBackText;
    
    protected override PlayerActionsState PlayerOverridableInteract(GameObject player)
    {
        if (TavernController.Instance.TavernOpen)
        {
            TavernController.Instance.CloseTavern();
            signFrontText.text = "Закрыто";
            signBackText.text = "Закрыто";
        }
        else
        {
            TavernController.Instance.OpenTavern();
            signFrontText.text = "Открыто";
            signBackText.text = "Открыто";
        }

        return PlayerActionsState.Default;
    }

    protected override NPCActionsState NpcOverridableInteract(GameObject npc)
    {
        throw new System.NotImplementedException();
    }

    protected override void Initiate()
    {
        signFrontText = signFrontTextObj.GetComponent<TextMeshProUGUI>();
        signBackText = signBackTextObj.GetComponent<TextMeshProUGUI>();
        
        return;
    }

    public override string ShowHint()
    {
        base.ShowHint();
        return "Нажмите F, чтобы " + (TavernController.Instance.TavernOpen == true ? "закрыть" : "открыть") +
               "заведение";
    }
}
