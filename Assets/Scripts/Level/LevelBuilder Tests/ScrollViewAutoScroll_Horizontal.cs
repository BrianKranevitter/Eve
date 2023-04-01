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
    public ScrollType type;
    
    private RectTransform _content;
    private RectTransform _viewport;

    public float offset;
    public float speed;
    
    //horizontal
    private float _leftPos;
    private float _rightPos;
    
    //vertical
    private float _upPos;
    private float _downPos;

    private Coroutine coroutine;

    public enum ScrollType
    {
        Vertical, Horizontal
    }
    private void Start()
    {
        _content = scrollRect.content;
        _viewport = scrollRect.viewport;
    }

    public void Select(RectTransform rect)
    {
        bool inView = RectTransformUtility.RectangleContainsScreenPoint(_viewport, rect.position);
        //bool inView = RectTransformUtility.RectangleContainsScreenPoint(_viewport, rect.position + 
          //  (type == ScrollType.Vertical ? new Vector3(0,offset,0) : new Vector3(offset, 0,0)));
        
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        
        if (!inView)
        {
            switch (type)
            {
                case ScrollType.Horizontal:
                    _leftPos = -rect.anchoredPosition.x + offset;
                    _rightPos = _viewport.rect.width - rect.anchoredPosition.x - offset;
                    coroutine = StartCoroutine(MovePos_Horizontal());
                    break;
                case ScrollType.Vertical:
                    _upPos = -rect.anchoredPosition.y + offset;
                    _downPos = _viewport.rect.height - rect.anchoredPosition.y - offset;
                    coroutine = StartCoroutine(MovePos_Vertical());
                    break;
            }
            
            
        }
    }

    IEnumerator MovePos_Horizontal()
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
    
    IEnumerator MovePos_Vertical()
    {
        float timer = 0;
        Vector2 vector2 = _content.anchoredPosition;
        while (timer < 1)
        {
            if (_upPos > _content.anchoredPosition.y)
            {
                _content.anchoredPosition = Vector2.Lerp(vector2, new Vector2(0, _upPos), timer);
            }
            else
            {
                _content.anchoredPosition = Vector2.Lerp(vector2, new Vector2(0, _downPos), timer);
            }

            timer += Time.deltaTime * speed;
            yield return null;
        }
    }
}
