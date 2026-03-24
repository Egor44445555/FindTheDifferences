using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageTouchMove : MonoBehaviour
{
    public static ImageTouchMove main;
    [SerializeField] float minZoom = 3.0f;
    [SerializeField] float maxZoom = 20.0f;

    [SerializeField] RectTransform Mask1;
    [SerializeField] RectTransform Mask2;
    [SerializeField] RectTransform Image1;
    [SerializeField] RectTransform Image2;

    [HideInInspector] public bool cameraMove = true;
    [HideInInspector] public float moveSpeed = 0.03f;    
    [HideInInspector] public float moveSpeedMouse = 0.05f;    
    [HideInInspector] public float zoomSpeed = 0.01f;
    [HideInInspector] public float zoomSpeedMouse = 0.5f;

    Camera cam;
    Vector2 touchStartPos;
    Vector3 cameraStartPos;
    Vector2 minBounds;
    Vector2 maxBounds;
    float baseOrthographicSize;

    Vector3 image1StartPos;
    Vector3 image2StartPos;
    Vector2 image1Size;
    Vector2 image2Size;

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
        cam = Camera.main;
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, -100);
        baseOrthographicSize = cam.orthographicSize;

        UpdateImageSizes();
    }

    void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            UpdateImageSizes();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            touchStartPos = mouseScreenPosition;
            
            if (Image1 != null && Image2 != null)
            {
                image1StartPos = Image1.transform.position;
                image2StartPos = Image2.transform.position;
            }
        }

        if (cameraMove && Input.GetMouseButton(0) && UIManager.main != null && !UIManager.main.IsGamePause())
        {
            if (Image1 == null || Image2 == null) return;
            
            Vector2 mouseScreenPosition = Input.mousePosition;
            Vector2 delta = mouseScreenPosition - touchStartPos;
            
            float zoomFactor = 1f;

            if (cam != null && baseOrthographicSize > 0)
            {
                zoomFactor = cam.orthographicSize / baseOrthographicSize;
            }
            
            float adjustedMoveSpeed = moveSpeedMouse * zoomFactor;
            
            if (float.IsNaN(adjustedMoveSpeed) || float.IsInfinity(adjustedMoveSpeed))
            {
                adjustedMoveSpeed = moveSpeedMouse;
            }
            
            Vector3 offset = new Vector3(-delta.x * adjustedMoveSpeed, -delta.y * adjustedMoveSpeed, 0);
            
            if (float.IsNaN(offset.x) || float.IsNaN(offset.y) || float.IsNaN(offset.z))
            {
                return;
            }
            
            Vector3 newPos1 = image1StartPos - offset;
            Vector3 newPos2 = image2StartPos - offset;
            
            newPos1.z = 0;
            newPos2.z = 0;
            
            if (!float.IsNaN(newPos1.x) && !float.IsNaN(newPos1.y) && !float.IsNaN(newPos1.z) &&
                !float.IsNaN(newPos2.x) && !float.IsNaN(newPos2.y) && !float.IsNaN(newPos2.z))
            {
                newPos1 = ClampPosition(Image1, Mask1, newPos1);
                newPos2 = ClampPosition(Image2, Mask2, newPos2);
                
                Image1.transform.position = newPos1;
                Image2.transform.position = newPos2;
            }
        }

        float scroll = Input.mouseScrollDelta.y;
        float currentScroll = Mathf.Clamp(Image1.transform.localScale.x + scroll * zoomSpeedMouse, minZoom, maxZoom);

        if (scroll != 0 && GameObject.FindGameObjectWithTag("Popup") == null)
        {
            Vector3 oldPos1 = Image1.transform.position;
            Vector3 oldPos2 = Image2.transform.position;
            
            Image1.transform.localScale = new Vector3(currentScroll, currentScroll, currentScroll);
            Image2.transform.localScale = new Vector3(currentScroll, currentScroll, currentScroll);
            
            Image1.transform.position = ClampPosition(Image1, Mask1, oldPos1);
            Image2.transform.position = ClampPosition(Image2, Mask2, oldPos2);
        }
    }

    Vector3 ClampPosition(RectTransform image, RectTransform mask, Vector3 desiredPosition)
    {
        if (image == null || mask == null)
        {
            return desiredPosition;
        }
        
        Vector3[] maskCorners = new Vector3[4];
        mask.GetWorldCorners(maskCorners);
        float maskWidth = Vector3.Distance(maskCorners[0], maskCorners[3]);
        float maskHeight = Vector3.Distance(maskCorners[0], maskCorners[1]);
        
        Vector3[] imageCorners = new Vector3[4];
        image.GetWorldCorners(imageCorners);
        float imageWidth = Vector3.Distance(imageCorners[0], imageCorners[3]);
        float imageHeight = Vector3.Distance(imageCorners[0], imageCorners[1]);
        
        Vector3 maskCenter = mask.transform.position;
        
        float maxXOffset = (imageWidth - maskWidth) / 2f;
        float maxYOffset = (imageHeight - maskHeight) / 2f;
        
        if (maxXOffset < 0) maxXOffset = 0;
        if (maxYOffset < 0) maxYOffset = 0;
        
        Vector3 clampedPosition = desiredPosition;
        clampedPosition.x = Mathf.Clamp(desiredPosition.x, maskCenter.x - maxXOffset, maskCenter.x + maxXOffset);
        clampedPosition.y = Mathf.Clamp(desiredPosition.y, maskCenter.y - maxYOffset, maskCenter.y + maxYOffset);
        
        return clampedPosition;
    }

    void UpdateImageSizes()
    {
        if (Image1 != null)
        {
            image1Size = GetImageSize(Image1);
        }
        
        if (Image2 != null)
        {
            image2Size = GetImageSize(Image2);
        }
    }
    
    Vector2 GetImageSize(RectTransform image)
    {
        Vector2 size = Vector2.zero;
        
        RectTransform rectTransform = image.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Vector2 rectSize = rectTransform.rect.size;
            Vector3 scale = image.transform.lossyScale;
            size.x = rectSize.x * scale.x;
            size.y = rectSize.y * scale.y;
        }
        
        return size;
    }
}
