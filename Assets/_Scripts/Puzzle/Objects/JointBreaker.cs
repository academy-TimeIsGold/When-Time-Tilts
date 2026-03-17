using UnityEngine;

public class JointBreaker : MonoBehaviour
{
    public void BreakJoint()
    {
        HingeJoint2D joint = GetComponent<HingeJoint2D>();
        if (joint != null)
        {
            Destroy(joint);
        }
    }
}
