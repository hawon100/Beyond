using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowDragReceiverScrollRect : ScrollRect
{
    protected bool isParent;
    protected IBeginDragHandler beginDragHandler;
    protected IDragHandler dragHandler;
    protected IEndDragHandler endDragHandler;

    protected override void Awake()
    {
        base.Awake();

        var t = transform;
        while (t)
        {
            t = t.parent;

            if (beginDragHandler == null) beginDragHandler = t.GetComponent<IBeginDragHandler>();
            if (dragHandler == null) dragHandler = t.GetComponent<IDragHandler>();
            if (endDragHandler == null) endDragHandler = t.GetComponent<IEndDragHandler>();

            if (beginDragHandler != null && dragHandler != null && endDragHandler != null) break;
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        isParent = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);

        if (isParent)
        {
            beginDragHandler?.OnBeginDrag(eventData);
            return;
        }
        base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (isParent)
        {
            dragHandler?.OnDrag(eventData);
            return;
        }
        base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (isParent)
        {
            endDragHandler?.OnEndDrag(eventData);
            isParent = false;
            return;
        }
        base.OnEndDrag(eventData);
    }
}
