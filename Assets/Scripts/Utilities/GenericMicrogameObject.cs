using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericMicrogameObject : MonoBehaviour
{
    public UnityEvent
        collisionEnter2D,
        collisionExit2D,
        collisionStay2D,
        triggerEnter2D,
        triggerExit2D,
        triggerStay2D,
        mouseDown,
        mouseDrag,
        mouseEnter,
        mouseExit,
        mouseOver,
        mouseUp;

    protected void OnCollisionEnter2D()
    {
        collisionEnter2D.Invoke();
    }
    protected void OnCollisionExit2D()
    {
        collisionExit2D.Invoke();
    }
    protected void OnCollisionStay2D()
    {
        collisionStay2D.Invoke();
    }
    protected void OnTriggerEnter2D()
    {
        triggerEnter2D.Invoke();
    }
    protected void OnTriggerExit2D()
    {
        triggerExit2D.Invoke();
    }
    protected void OnTriggerStay2D()
    {
        triggerStay2D.Invoke();
    }
    protected void OnMouseDown()
    {
        mouseDown.Invoke();
    }
    protected void OnMouseDrag()
    {
        mouseDrag.Invoke();
    }
    protected void OnMouseEnter()
    {
        mouseEnter.Invoke();
    }
    protected void OnMouseExit()
    {
        mouseExit.Invoke();
    }
    protected void OnMouseOver()
    {
        mouseOver.Invoke();
    }
    protected void OnMouseUp()
    {
        mouseUp.Invoke();
    }
}
