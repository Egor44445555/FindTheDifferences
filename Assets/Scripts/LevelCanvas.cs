using UnityEngine;

public class LevelCanvas : MonoBehaviour
{
    Canvas canvas;
    
    void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.sortingOrder = -10;
        
        if (canvas.worldCamera == null)
        {
            canvas.worldCamera = Camera.main;
        }
        
        canvas.planeDistance = 1;
    }
        
    void OnEnable()
    {
        canvas.sortingOrder = -10;
    }
}
