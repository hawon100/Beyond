using UnityEngine;

public class Window : WindowDragReceiver
{
    public int WindowIndex { get; set; }

    public virtual void Refresh() { }
}