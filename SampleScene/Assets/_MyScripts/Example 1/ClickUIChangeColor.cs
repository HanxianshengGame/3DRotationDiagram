using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ClickUIChangeColor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public void OnPointerDown(PointerEventData eventData)
    {
        GetComponent<Image>().color = Color.black;
        ExecuteAllRayEvents(eventData, 1);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GetComponent<Image>().color = Color.red;
        ExecuteAllRayEvents(eventData, 2);
    }

    /// <summary>
    /// 执行所有由RayCastAll检测到的RayResults的响应事件
    /// </summary>
    /// <param name="eventData">点击事件数据</param>
    /// <param name="executeEventsIndex">执行事件的角标（由于ExecuteEvents为静态类，为了达到可复用，采取角标传参）</param>
    private void ExecuteAllRayEvents(PointerEventData eventData,int executeEventsIndex)
    {
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        foreach (RaycastResult result in raycastResults)
        {
            if(result.gameObject!=gameObject)
            {
                switch (executeEventsIndex)
                {
                    case 1: ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.pointerDownHandler);
                        break;
                    case 2: ExecuteEvents.Execute(result.gameObject, eventData, ExecuteEvents.pointerUpHandler);
                        break;
                    default:
                        break;
                }
            }
        }
    }

}
