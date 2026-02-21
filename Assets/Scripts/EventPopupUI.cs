using UnityEngine;
using TMPro;
using System.Collections;

public class EventPopupUI : MonoBehaviour
{
    [Header("References")]
    public RectTransform banner;
    public TextMeshProUGUI eventText;

    [Header("Positions")]
    public Vector2 startPosition;   // Hidden position
    public Vector2 endPosition;     // Visible position

    [Header("Animation")]
    public float slideSpeed = 800f;

    private Coroutine currentMove;

    private void Start()
    {
        // Start at hidden position
        banner.anchoredPosition = startPosition;
    }

    public void ShowEvent(GameEventSO gameEvent)
    {
        eventText.text = gameEvent.eventName + "\n\n" + gameEvent.description;

        MoveTo(endPosition);
    }

    public void Hide()
    {
        MoveTo(startPosition);
    }

    public void Toggle()
    {
        if (banner.anchoredPosition == endPosition)
            MoveTo(startPosition);
        else
            MoveTo(endPosition);
    }

    void MoveTo(Vector2 target)
    {
        if (currentMove != null)
            StopCoroutine(currentMove);

        currentMove = StartCoroutine(Slide(target));
    }

    IEnumerator Slide(Vector2 target)
    {
        while (Vector2.Distance(banner.anchoredPosition, target) > 0.1f)
        {
            banner.anchoredPosition = Vector2.MoveTowards(
                banner.anchoredPosition,
                target,
                slideSpeed * Time.deltaTime
            );

            yield return null;
        }

        banner.anchoredPosition = target;
    }
}
