using DG.Tweening;
using ShiroGe.CharacterController;
using ShiroGe.Scripts;
using ShiroGe.Scripts.NPC;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

[RequireComponent(typeof(NavMeshObstacle))]
public class TavernDoor : Interactable
{
    [field: SerializeField] public Vector3 OpenRotation { get; private set; }
    [field: SerializeField] public Vector3 ClosedRotation { get; private set; }
    [field: SerializeField] public float RotatingTime { get; private set; }
    [field: SerializeField] public TavernDoor[] CascadeDoors { get; private set; }
    
    private bool closed = true;
    
    protected override PlayerActionsState PlayerOverridableInteract(GameObject player)
    {
        if(closed)
        {
            transform.DOLocalRotate(OpenRotation, RotatingTime).SetEase(Ease.InOutBounce);
            if (CascadeDoors != null && CascadeDoors.Length > 0 )
            {
                foreach (TavernDoor door in CascadeDoors)
                {
                    door.transform.DOLocalRotate(door.OpenRotation, door.RotatingTime).SetEase(Ease.InOutBounce);
                    door.closed = false;
                }
            }
            closed = false;
        }
        else
        {
            transform.DOLocalRotate(ClosedRotation, RotatingTime).SetEase(Ease.InOutBounce);
            if (CascadeDoors != null && CascadeDoors.Length > 0)
            {
                foreach (TavernDoor door in CascadeDoors)
                {
                    door.transform.DOLocalRotate(door.ClosedRotation, door.RotatingTime).SetEase(Ease.InOutBounce);
                    door.closed = true;
                }
            }
            closed = true;
        }
        
        return PlayerActionsState.Default;
    }

    protected override NPCActionsState NpcOverridableInteract(GameObject npc)
    {
        throw new System.NotImplementedException();
    }

    protected override void Initiate()
    {
        NavMeshObstacle obstacle = gameObject.GetComponent<NavMeshObstacle>();
        obstacle.carving = true;
        obstacle.carveOnlyStationary = false;
    }
}
