using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _MyScripts.Example_3 {
    public class RotationDiagramItem : MonoBehaviour,IDragHandler,IEndDragHandler
    {
        // Start is called before the first frame update
        private int posId;
        private Image myImage;
        private float offsetX;
        private Action<float> moveAction;

        private Image MyImage
        {
            get {
                if (myImage == null)
                    myImage = GetComponent<Image>();
                return myImage;      
               }
        }
        private RectTransform myRect;
        private RectTransform MyRect
        {
            get
            {
                if (myRect == null)
                    myRect = GetComponent<RectTransform>();
                return myRect;
            }
        }

        public int PosId { get => posId; set => posId = value; }

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }
        public void SetSprite(Sprite sprite)
        {
            MyImage.sprite = sprite;
        }
        public void SetPosData(ItemPosData itemPosData)
        {

            MyRect.DOAnchorPos(Vector2.right * itemPosData.X, 1.0f);
            MyRect.DOScale(Vector2.one * itemPosData.ScaleTimes,1.0f);
            StartCoroutine(Wait(itemPosData.Order));
        }

        IEnumerator Wait(int order)
        {
            yield return new WaitForSeconds(0.5f);
            transform.SetSiblingIndex(order);
        }
        public void OnDrag(PointerEventData eventData)
        {
            offsetX += eventData.delta.x;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            moveAction(offsetX);
            offsetX = 0;
        }
        public void AddMovelistener(Action<float> onMove)
        {
            moveAction = onMove;
        }
        public void ChangeId(int symbol,int totalItemNum)
        {
            int id = PosId;
            id += symbol;
            if(id<0)
            {
                id += totalItemNum;
            }
            PosId = id % totalItemNum;
        }
    }
}
