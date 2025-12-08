using Spine.Unity;
using UnityEngine;

public class testSpine : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation spinePlayer;
    private bool bReady = false;
    private int nEXP = 0;
    private int nLevel = 0;
    private int nMaxLevel = 0;

    private void Start()
    {
      
        spinePlayer.AnimationState.AddAnimation(0, "attack", true, 0f);

        spinePlayer.AnimationState.End += (entry) =>
        {
            if (entry.Animation.Name == "attack")
            {
                bReady = true;
            }
        };

    }
    void Update()
    {
        
    }

    public void ButtonGetEXP()
    {
        if(bReady == false)
        {
            return;
        }

    
        spinePlayer.AnimationState.SetAnimation(0, "attack", false);

        spinePlayer.AnimationState.AddAnimation(0, "attack", true, 0f);
    }

    public void ButtonLevelUp()
    {
        if(bReady == false)
        {
            return;
        }


     
            spinePlayer.AnimationState.SetAnimation(0, "attack", false);
          
            spinePlayer.AnimationState.AddAnimation(0, "attack", true, 0f);
    
    }



}