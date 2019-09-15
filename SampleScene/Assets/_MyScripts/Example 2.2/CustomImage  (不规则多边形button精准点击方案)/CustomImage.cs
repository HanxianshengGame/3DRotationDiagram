using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Example2_2
{
    public class CustomImage : Image
    {
        private PolygonCollider2D polygonCollider;
        public PolygonCollider2D PolygonCollider2D
        {
            get
            {
                polygonCollider = GetComponent<PolygonCollider2D>();
                return polygonCollider;
            }
        }

        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            Vector3 tempPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPoint, eventCamera, out tempPos);

            return PolygonCollider2D.OverlapPoint(tempPos);
        }
    }

}