using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CanvasImageSlider : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Компоненты")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    
    [Header("Настройки")]
    [SerializeField] private float snapSpeed = 10f;
    [SerializeField] private float autoSlideInterval = 5f;
    [SerializeField] private bool enableAutoSlide = true;
    
    private float[] slidePositions;
    private int currentIndex = 0;
    private bool isDragging = false;
    private float autoSlideTimer;
    
    private void Start()
    {
        // Получаем все слайды
        int slideCount = content.childCount;
        slidePositions = new float[slideCount];
        
        // Вычисляем позиции слайдов
        float slideWidth = content.GetChild(0).GetComponent<RectTransform>().rect.width;
        float contentWidth = slideWidth * slideCount;
        content.sizeDelta = new Vector2(contentWidth, content.sizeDelta.y);
        
        for (int i = 0; i < slideCount; i++)
        {
            slidePositions[i] = (i * slideWidth) / contentWidth;
        }
        
        // Подписываемся на кнопки
        if (prevButton != null)
            prevButton.onClick.AddListener(PrevSlide);
        if (nextButton != null)
            nextButton.onClick.AddListener(NextSlide);
        
        autoSlideTimer = autoSlideInterval;
    }
    
    private void Update()
    {
        if (!isDragging && enableAutoSlide)
        {
            autoSlideTimer -= Time.deltaTime;
            if (autoSlideTimer <= 0)
            {
                NextSlide();
                autoSlideTimer = autoSlideInterval;
            }
        }
        
        // Плавная привязка к слайду
        if (!isDragging)
        {
            float targetPosition = slidePositions[currentIndex];
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
                scrollRect.horizontalNormalizedPosition, 
                targetPosition, 
                snapSpeed * Time.deltaTime
            );
        }
    }
    
    public void NextSlide()
    {
        currentIndex = (currentIndex + 1) % slidePositions.Length;
        autoSlideTimer = autoSlideInterval;
    }
    
    public void PrevSlide()
    {
        currentIndex = (currentIndex - 1 + slidePositions.Length) % slidePositions.Length;
        autoSlideTimer = autoSlideInterval;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        autoSlideTimer = autoSlideInterval;
    }
    
    public void OnDrag(PointerEventData eventData) { }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        
        // Определяем ближайший слайд
        float currentPos = scrollRect.horizontalNormalizedPosition;
        float closestDistance = float.MaxValue;
        
        for (int i = 0; i < slidePositions.Length; i++)
        {
            float distance = Mathf.Abs(currentPos - slidePositions[i]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentIndex = i;
            }
        }
    }
}