using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace _MyScripts.Example_3
{
    public class RotationDiagram2D : MonoBehaviour
    {
        // Start is called before the first frame update
        public Vector2 ItemSize;//子物体尺寸
        public Sprite[] ItemSprites;//子物体的Sprite图集
        public float itemOffset;   //子物体之间的间距
        private List<RotationDiagramItem> itemsList;//轮转子物体脚本集合
        private List<ItemPosData> itemPosList;//子物体的坐标集合
        public float scaleMax;//最大缩放
        public float scaleMin;//最小缩放
        void Start()
        {
            itemsList = new List<RotationDiagramItem>();
            itemPosList = new List<ItemPosData>();
            CreateItem();
            CalulateData();
            SetItemData();
        }

        // Update is called once per frame
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
        
        /// <summary>
        /// 改变
        /// </summary>
        /// <param name="offsetX"></param>
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

            foreach (var item in itemsList)
            {
                item.SetPosData(itemPosList[item.PosId]);
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
        /// <summary>
        /// 得到x轴坐标
        /// </summary>
        /// <param name="radio">周长占比系数</param>
        /// <param name="length">周长</param>
        /// <returns></returns>
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
        /// <summary>
        /// 得到缩放系数
        /// </summary>
        /// <param name="radio">周长占比系数</param>
        /// <param name="max">缩放最大值</param>
        /// <param name="min">缩放最小值</param>
        /// <returns></returns>
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
