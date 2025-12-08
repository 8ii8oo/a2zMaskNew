using UnityEngine;
using Spine.Unity;
using Spine; 

public class MaskChange : MonoBehaviour
{
   
    public SkeletonGraphic skeletonGraphic;

    private const string SLOT_NAME = "black_mask";
    private const string BLACK_MASK_ATTACHMENT = "black_mask";
    private const string BLUE_MASK_ATTACHMENT = "blue_mask"; 
    private const string RED_MASK_ATTACHMENT = "red_mask";   

    void Start()
    {
        if (skeletonGraphic == null)
        {
            skeletonGraphic = GetComponent<SkeletonGraphic>();
        }
        
        if (skeletonGraphic == null)
        {
            Debug.LogError("SkeletonGraphic 컴포넌트를 찾을 수 없습니다.");
        }
    }    

    public void SetMaskAttachment(string attachmentName)
    {
        if (skeletonGraphic == null || skeletonGraphic.Skeleton == null) 
        {
            Debug.LogError("SkeletonGraphic 컴포넌트 또는 스켈레톤 데이터가 준비되지 않았습니다.");
            return;
        }

        Spine.Slot slot = skeletonGraphic.Skeleton.FindSlot(SLOT_NAME);

        
    }

    public void ChangeToBlueMask()
    {
        SetMaskAttachment(BLUE_MASK_ATTACHMENT);
    }
    
    public void ChangeToRedMask()
    {
        SetMaskAttachment(RED_MASK_ATTACHMENT);
    }

    public void ChangeToBlackMask()
    {
        SetMaskAttachment(BLACK_MASK_ATTACHMENT);
    }

}