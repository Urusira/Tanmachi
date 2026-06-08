using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SeatingRig : MonoBehaviour
{
    public event System.Action OnSeat;
    public event System.Action OnStand;
    
    [SerializeField] private RigBuilder rigBuilder;
    [SerializeField] private TwoBoneIKConstraint leftLegConstraint;
    [SerializeField] private TwoBoneIKConstraint rightLegConstraint;

    public void SitDown()
    {
        rigBuilder.enabled = true;
        OnSeat?.Invoke();
    }

    public void StandUp()
    {
        rigBuilder.enabled = false;
        OnStand?.Invoke();
    }

    public void SetLegsAnchors(Transform leftLegAnchor, Transform rightLegAnchor)
    {
        if (leftLegConstraint != null)
        {
            leftLegConstraint.data.target = leftLegAnchor;
            rightLegConstraint.data.target = rightLegAnchor;
        }
    }
}