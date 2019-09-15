using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Click3DGameObjectChangeColor : MonoBehaviour
{
    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    GetComponent<MeshRenderer>().material.color = Color.black;
    //    Debug.Log("33");
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    GetComponent<MeshRenderer>().material.color = Color.white;
    //    Debug.Log("44");
    //}

    // Start is called before the first frame update
    private GraphicRaycaster graphicRaycaster;
    void Start()
    {
        graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
    }

    //void OnMouseDown()
    //{
    //    GetComponent<MeshRenderer>().material.color = Color.black;
    //}
    //void OnMouseUp()
    //{
    //    GetComponent<MeshRenderer>().material.color = Color.white;
    //}

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)&&!IsTriggerUI())
        {
            GetComponent<MeshRenderer>().material.color = Color.black;
        }
        if(Input.GetMouseButtonUp(0)&&!IsTriggerUI())
        {
            GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }

    /// <summary>
    /// 是否当前(菜单快捷键：鼠标左右键)点击检测到UI物体
    /// </summary>
    /// <returns></returns>
    private bool IsTriggerUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.pressPosition = Input.mousePosition;
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, raycastResults);
        return raycastResults.Count > 0;
    }

}
