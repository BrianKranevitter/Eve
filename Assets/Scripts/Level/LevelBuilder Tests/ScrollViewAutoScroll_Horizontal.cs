using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Some notes:
//The anchor of the content items must be on the middle
public class ScrollViewAutoScroll_Horizontal : MonoBehaviour
{
    public ScrollRect scrollRect;

    private RectTransform _content;
    private RectTransform _viewport;

    public float offset;
    public float speed;
    
    private float _leftPos;
    private float _rightPos;

    private Coroutine coroutine;
    private void Start()
    {
        _content = scrollRect.content;
        _viewport = scrollRect.viewport;
    }

    public void Select(RectTransform rect)
    {
        bool inView = RectTransformUtility.RectangleContainsScreenPoint(_viewport, rect.position);
        
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        
        if (!inView)
        {
            _leftPos = -rect.anchoredPosition.x + offset;
            _rightPos = _viewport.rect.width - rect.anchoredPosition.x - offset;
            coroutine = StartCoroutine(MovePos());
        }
    }

    IEnumerator MovePos()
    {
        float timer = 0;
        Vector2 vector2 = _content.anchoredPosition;
        while (timer < 1)
        {
            if (_leftPos > _content.anchoredPosition.x)
            {
                _content.anchoredPosition = Vector2.Lerp(vector2, new Vector2(_leftPos, 0), timer);
            }
            else
            {
                _content.anchoredPosition = Vector2.Lerp(vector2, new Vector2(_rightPos, 0), timer);
            }

            timer += Time.deltaTime * speed;
            yield return null;
        }
    }
}
