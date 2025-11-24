using UnityEngine;
using Spine.Unity;
using System.Runtime.InteropServices;

public class skinTest : MonoBehaviour
{
    SkeletonAnimation skeletonAnimation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
