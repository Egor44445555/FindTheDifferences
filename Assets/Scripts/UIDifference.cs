using UnityEngine;
using UnityEngine.UI;

public class UIDifference : MonoBehaviour
{
    [SerializeField] Sprite successBackground;
    [SerializeField] Sprite[] numberIcons;
    [SerializeField] int serialNumber;

    bool activeDifference = false;
    Image image;
    Animator anim;
    bool isAnimationPlaying = false;
    string currentAnimation = "Active";

    void Awake()
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
        }

        SuccessAnimate();
        
        activeDifference = true;
    }

    public void SetSerialNumber(int _serialNumber)
    {
        serialNumber = _serialNumber;

        if (numberIcons.Length - 1 > 0 && numberIcons.Length - 1 >= serialNumber && serialNumber >= 0 && image != null)
        {
            image.sprite = numberIcons[serialNumber];            
        }
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
