using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

namespace Example3
{
    public class RotationDiagram2D : MonoBehaviour
    {
        // Start is called before the first frame update
        public Vector2 ItemSize;
        public Sprite[] ItemSprites;
        public float itemOffset;   //间距
        private List<RotationDiagramItem> itemsList;
        private List<ItemPosData> itemPosList;
        public float scaleMax;
        public float scaleMin;
        void Start()
        {
            itemsList = new List<RotationDiagramItem>();
            itemPosList = new List<ItemPosData>();
            CreateItem();
            CalulateData();
            SetItemData();
        }

        // Update is called once per frame
        void Update()
        {

        }
        private void SetItemData()
        {
            for (int i = 0; i < itemsList.Count; i++)
            {
                itemsList[i].SetPosData(itemPosList[i]);
            }
        }

        /// <summary>
        /// 创建模板Template物体
        /// </summary>
        /// <returns></returns>
        private GameObject CreateTemplate()
        {
            GameObject item = new GameObject("Template");
            item.AddComponent<RectTransform>().sizeDelta = ItemSize;
            item.AddComponent<RotationDiagramItem>();
            item.AddComponent<Image>();
            return item;
        }
        private void CreateItem()
        {
            GameObject template = CreateTemplate();
            RotationDiagramItem itemTemp = null;
            //resources->prefab->->GameObject->init
            foreach (Sprite sprite in ItemSprites)
            {
                itemTemp = Instantiate(template).GetComponent<RotationDiagramItem>();
                itemTemp.SetParent(transform);
                itemTemp.SetSprite(sprite);
                itemTemp.AddMovelistener(Change);
                itemsList.Add(itemTemp);
            }
            Destroy(template);
        }
        private void Change(float offsetX)
        {
            int symbol = offsetX > 0 ? 1 : -1;
            Change(symbol);
        }
        private void Change(int symbol)
        {
            foreach (var item in itemsList)
            {
                item.ChangeId(symbol, itemsList.Count);
            }
            for (int i = 0; i < itemsList.Count; i++)
            {
                itemsList[i].SetPosData(itemPosList[itemsList[i].PosId]);
            }
        }

        private void CalulateData()
        {
            List<ItemData> itemDatasList = new List<ItemData>();

            float length = (ItemSize.x + itemOffset) * 3;      //计算总长度
            float radioOffset = 1.0f / itemsList.Count;        //偏移比例系数   计算平均分配的系数累加度
            float tempRadio = 0;
            for (int i = 0; i < itemsList.Count; i++)
            {
                ItemData itemData = new ItemData();
                ItemPosData itemPosData = new ItemPosData();
                itemData.PosId = i;
                itemDatasList.Add(itemData);
                itemsList[i].PosId = i;


                itemPosData.X = GetX(tempRadio, length);
                itemPosData.ScaleTimes = GetScaleTimes(tempRadio, scaleMax, scaleMin);
                itemPosList.Add(itemPosData);
                tempRadio += radioOffset;
            }

            itemDatasList = itemDatasList.OrderBy(u => itemPosList[u.PosId].ScaleTimes).ToList();  //待解决
            for (int i = 0; i < itemDatasList.Count; i++)
            {
                itemPosList[itemDatasList[i].PosId].Order = i;
            }

        }

        private float GetX(float radio,float length)
        {
            if(radio>1||radio<0)
            {
                Debug.LogError("当前比例必须是0-1的值");
                return 0;
            }

            if(radio>=0&&radio<=0.25f)
            {
                return radio * length;
            }
            else if(radio>0.25f&&radio<0.75f)
            {
                return (0.5f - radio) * length;
            }
            else
            {
                return (radio-1) * length;
            }
        }
        private float GetScaleTimes(float radio,float max,float min)
        {
            if (radio > 1 || radio < 0)
            {
                Debug.LogError("当前比例必须是0-1的值");
                return 0;
            }
            float scalePercent = (max - min) / 0.5f;
            if(radio==0||radio==1)
            {
                return 1;
            }
            else if(radio>0&&radio<=0.5f)
            {
                return (1-radio) * scalePercent;
            }
            else
            {
                return radio * scalePercent;
            }

        }

    }
    public class ItemPosData   //类 
    {
        public float X;
        public float ScaleTimes;
        public int Order;
    }
    public struct  ItemData          //值类型传递   调用时为拷贝数据   无法改变原始的对象变量
    {
        public int PosId;
    }
}
