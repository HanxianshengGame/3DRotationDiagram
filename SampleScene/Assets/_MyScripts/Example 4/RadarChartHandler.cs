using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _MyScripts.Example_4
{
    public class RadarChartHandler : MonoBehaviour,IDragHandler
    {

        private RectTransform _rect;
        private Image _image;
    
        private RectTransform Rect
        {
            get { if (_rect == null)
                    _rect = GetComponent<RectTransform>();
                return _rect;

            }
        }
    
    
    
        private Image Image
        {
            get
            {
                if (_image== null)
                    _image = GetComponent<Image>();
                return _image;

            }
        }
        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }
        public void ChangeSprite(Sprite sprite)
        {
            Image.sprite = sprite;
        }
        public void ChangeColor(Color32 color32)
        {
            Image.color = color32;
        }
        public void SetSize(Vector2 sizeDelta)
        {
            Rect.sizeDelta = sizeDelta;
        }
        public void SetPos(Vector2 pos)
        {
            Rect.anchoredPosition = pos;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Rect.anchoredPosition += (eventData.delta/GetScale());
        }

        private float GetScale()
        {
            return Rect.lossyScale.x;
        }
    }
}
