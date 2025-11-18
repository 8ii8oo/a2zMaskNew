using Spine.Unity;
using UnityEngine;

public class testSpine : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation spinePlayer; // 스파인 애니메이션
    private bool bReady = false;
    private int nEXP = 0;
    private int nLevel = 0;
    private int nMaxLevel = 0;

    private void Start()
    {
        spinePlayer.AnimationState.SetAnimation(0, "attack", false);
        spinePlayer.AnimationState.AddAnimation(0, "idle", true, 0f);

        spinePlayer.AnimationState.End += (entry) =>
        {
            if (entry.Animation.Name == "idle")
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

        // 플레이어 레벨에 맞는 테이블 데이터 가져오기
       
        

        // 0번트랙, shoot 애니메이션, 반복재생 안함
        spinePlayer.AnimationState.SetAnimation(6, "walk", false);
        // 애니메이션이 끝난 후 다시 idle 애니메이션 재생
        spinePlayer.AnimationState.AddAnimation(0, "idle", true, 0f);
    }

    public void ButtonLevelUp()
    {
        if(bReady == false)
        {
            return;
        }


            // 0번트랙, jump 애니메이션, 반복재생 안함
            spinePlayer.AnimationState.SetAnimation(0, "walk", false);
            // 애니메이션이 끝난 후 다시 idle 애니메이션 재생
            spinePlayer.AnimationState.AddAnimation(0, "idle", true, 0f);
    
    }



}