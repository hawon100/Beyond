using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowDragReceiver : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected bool isParent;
    protected IBeginDragHandler beginDragHandler;
    protected IDragHandler dragHandler;
    protected IEndDragHandler endDragHandler;

    protected virtual void Awake()
    {
        var t = transform;
        while(t)
        {
            t = t.parent;

            if(beginDragHandler == null) beginDragHandler = t.GetComponent<IBeginDragHandler>();
            if(dragHandler == null) dragHandler = t.GetComponent<IDragHandler>();
            if(endDragHandler == null) endDragHandler = t.GetComponent<IEndDragHandler>();
            
            if(beginDragHandler != null && dragHandler != null && endDragHandler != null) break;
        }
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        isParent = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);

        if(isParent)
        {
            beginDragHandler?.OnBeginDrag(eventData);
            return;
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if(isParent)
        {
            dragHandler?.OnDrag(eventData);
            return;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if(isParent)
        {
            endDragHandler?.OnEndDrag(eventData);
            isParent = false;
            return;
        }
    }
}
