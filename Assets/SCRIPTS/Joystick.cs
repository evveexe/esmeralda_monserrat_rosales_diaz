using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private float radialLimit = 100f;
    [SerializeField] private RectTransform joystickHandle;
    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private float resetSpeed = 10f;

    [HideInInspector] public Vector2 axis;

    private Vector2 startPosition;
    private bool isDragging = false;

    private void Start()
    {
        startPosition = joystickHandle.anchoredPosition;
    }

    private void Update()
    {
        if (!isDragging)
        {
            joystickHandle.anchoredPosition = Vector2.Lerp(
                joystickHandle.anchoredPosition,
                startPosition,
                resetSpeed * Time.deltaTime);

            if (Vector2.Distance(joystickHandle.anchoredPosition, startPosition) < 0.1f)
            {
                joystickHandle.anchoredPosition = startPosition;
                axis = Vector2.zero;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint);

        localPoint = Vector2.ClampMagnitude(localPoint, radialLimit);
        joystickHandle.anchoredPosition = localPoint;

        axis = localPoint / radialLimit;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }
}


//Joysticks como visto en clase, los drags se basan desde los sprites que utilizamos en pantalla y los radiales tambien se ajustaron para que se vea un movimiento realista del joystick