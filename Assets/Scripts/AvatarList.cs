using UnityEngine;

public class AvatarList : MonoBehaviour
{
    [SerializeField] Transform sliderContent;
    [SerializeField] GameObject slidePrefab;
    [SerializeField] GameObject avatarPrefab;
    [SerializeField] Sprite[] avatars;

    void Start()
    {
        // if (avatars.Length > 0 && slidePrefab != null && avatarPrefab != null && sliderContent != null)
        // {
        //     int index = 0;
        //     GameObject currentSlideObject = null;

        //     foreach(Sprite _avatar in avatars)
        //     {
        //         if (index % 5 == 0)
        //         {
        //             currentSlideObject = Instantiate(slidePrefab, sliderContent);
                    
        //             if (currentSlideObject == null)
        //             {
        //                 index++;
        //                 continue;
        //             }
        //         }
                
        //         if (currentSlideObject != null && _avatar != null)
        //         {
        //             GameObject avatarObject = Instantiate(avatarPrefab, currentSlideObject.transform);
                    
        //             if (avatarObject == null)
        //             {
        //                 index++;
        //                 continue;
        //             }
                    
        //             AvatarInnerSlider avatarComponent = avatarObject.GetComponent<AvatarInnerSlider>();
                    
        //             if (avatarComponent != null)
        //             {
        //                 avatarComponent.SetImage(_avatar);
        //             }
        //         }

        //         index++;
        //     }
        // }
    }
}
