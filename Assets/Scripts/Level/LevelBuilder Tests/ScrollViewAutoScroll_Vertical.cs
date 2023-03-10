using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewAutoScroll_Vertical : MonoBehaviour
{
    [SerializeField] private RectTransform _viewportRectTransform;
    [SerializeField] private RectTransform _content;
    [SerializeField] private float _transitionDuration = 0.2f;

    private TransitionHelper _transitionHelper = new TransitionHelper();

    private void Update()
    {
        if (_transitionHelper.InProgress)
        {
            _transitionHelper.Update();

            _content.transform.localPosition = _transitionHelper.PosCurrent;
        }
    }

    public void HandleOnSelectChanged(GameObject obj)
    {
        float viewportTopBorderY = GetBorderTopYLocal(_viewportRectTransform.gameObject);
        float viewportBottomBorderY = GetBorderBottomYLocal(_viewportRectTransform.gameObject);
        
        //Top
        float targetTopBorderY = GetBorderTopYRelative(obj);
        float targetTopYWithViewportOffset = targetTopBorderY + viewportTopBorderY;
        
        //bottom
        float targetBottomBorderY = GetBorderBottomYRelative(obj);
        float targetBottomYWithViewportOffset = targetBottomBorderY - viewportTopBorderY;
        
        //Top difference
        float topDiff = targetTopYWithViewportOffset - viewportTopBorderY;
        if (topDiff > 0f)
        {
            MoveContentObjectByAmount((topDiff * 100f) + GetVerticalLayoutGroup().padding.top);
        }
        
        //bottom difference
        float botDiff = targetBottomYWithViewportOffset - viewportBottomBorderY;
        if (botDiff < 0f)
        {
            MoveContentObjectByAmount((botDiff * 100f) + GetVerticalLayoutGroup().padding.bottom);
        }
    }

    private float GetBorderTopYLocal(GameObject obj)
    {
        Vector3 pos = obj.transform.localPosition / 100f;
        return pos.y;
    }

    private float GetBorderBottomYLocal(GameObject obj)
    {
        Vector2 rectSize = obj.GetComponent<RectTransform>().rect.size * 0.01f;
        Vector3 pos = obj.transform.localPosition / 100f;
        pos.y -= rectSize.y;
        
        return pos.y;
    }

    private float GetBorderTopYRelative(GameObject obj)
    {
        float contentY = _content.transform.localPosition.y / 100f;
        float targetBorderUpYLocal = GetBorderTopYLocal(obj);
        float targetBorderUpYRelative = targetBorderUpYLocal + contentY;

        return targetBorderUpYRelative;
    }

    private float GetBorderBottomYRelative(GameObject obj)
    {
        float contentY = _content.transform.localPosition.y / 100f;
        float targetBorderBottomYLocal = GetBorderBottomYLocal(obj);
        float targetBorderBottomYRelative = targetBorderBottomYLocal + contentY;

        return targetBorderBottomYRelative;
    }

    private void MoveContentObjectByAmount(float amount)
    {
        Vector2 posScrollFrom = _content.transform.localPosition;
        Vector2 posScrollTo = posScrollFrom;
        posScrollTo.y -= amount;
        
        _transitionHelper.TransitionPositionFromTo(posScrollFrom, posScrollTo, _transitionDuration);
    }

    private VerticalLayoutGroup GetVerticalLayoutGroup()
    {
        VerticalLayoutGroup verticalLayoutGroup = _content.GetComponent<VerticalLayoutGroup>();
        return verticalLayoutGroup;
    }
    private class TransitionHelper
    {
        private float _duration = 0f;
        private float _timeElapsed = 0f;
        private float _progress = 0f;
        private bool _inProgress = false;

        private Vector2 _posCurrent;
        private Vector2 _posFrom;
        private Vector2 _posTo;
        
        public bool InProgress { get => _inProgress; }
        
        public Vector2 PosCurrent { get => _posCurrent; }

        public void Update()
        {
            Tick();

            CalculatePosition();
        }

        public void Clear()
        {
            _duration = 0f;
            _timeElapsed = 0f;
            _progress = 0f;

            _inProgress = false;
        }

        public void TransitionPositionFromTo(Vector2 posFrom, Vector2 posTo, float duration)
        {
            Clear();

            _posFrom = posFrom;
            _posTo = posTo;
            _duration = duration;

            _inProgress = true;
        }
        private void CalculatePosition()
        {
            _posCurrent.x = Mathf.Lerp(_posFrom.x, _posTo.x, _progress);
            _posCurrent.y = Mathf.Lerp(_posFrom.y, _posTo.y, _progress);
        }


        private void Tick()
        {
            if (_inProgress == false)
            {
                return;
            }

            _timeElapsed += Time.deltaTime;
            _progress = _timeElapsed / _duration;
            if (_progress > 1f)
            {
                _progress = 1f;
            }

            if (_progress >= 1f)
            {
                TransitionComplete();
            }
        }

        private void TransitionComplete()
        {
            _inProgress = false;
        }
    }
}
