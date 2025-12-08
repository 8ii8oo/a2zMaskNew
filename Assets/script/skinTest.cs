using UnityEngine;
using Spine.Unity;
using System.Runtime.InteropServices;

public class skinTest : MonoBehaviour
{
    SkeletonAnimation skeletonAnimation;
    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        skeletonAnimation.skeleton.SetSkin("red");    
        skeletonAnimation.skeleton.SetupPoseSlots();
    }

    void Update()
    {
        
    }
}
