using UnityEngine;
using UnityEngine.UI;

public class UIDifference : MonoBehaviour
{
    [SerializeField] Sprite successBackground;
    [SerializeField] Sprite failBackground;
    [SerializeField] GameObject successIcon;
    [SerializeField] GameObject failIcon;
    [SerializeField] int serialNumber;

    bool activeDifference = false;
    Image image;
    Animator anim;
    bool isAnimationPlaying = false;
    string currentAnimation = "Active";

    void Start()
    {
        image = GetComponent<Image>();
        anim = GetComponent<Animator>();  
    }
    
    void Update()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        
        if (stateInfo.IsName("SuccessUIDifference"))
        {
            if (stateInfo.normalizedTime >= 1.0f)
            {
                anim.SetBool(currentAnimation, false);
            }
        }
    }

    public void CatchHandler(bool success)
    {
        if (success)
        {
            image.sprite = successBackground;
            successIcon.SetActive(true);
        }
        else
        {
            image.sprite = failBackground;
            failIcon.SetActive(true);
        }

        SuccessAnimate();
        
        activeDifference = true;
    }

    public int GetSerialNumber()
    {
        return serialNumber;
    }

    public void SuccessAnimate()
    {
        if (anim != null)
        {
            anim.SetBool(currentAnimation, true);
        }
    }
}
