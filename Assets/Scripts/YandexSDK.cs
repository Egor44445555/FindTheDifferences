using UnityEngine;
// using YG;

public class YandexSDK : MonoBehaviour
{
    public static YandexSDK main;

    void Awake()
    {
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        print("GameReadyAPI");
        // YG2.GameReadyAPI();
    }

    void OnDestroy()
    {
        if (main == this)
        {
            main = null;
        }
    }
}