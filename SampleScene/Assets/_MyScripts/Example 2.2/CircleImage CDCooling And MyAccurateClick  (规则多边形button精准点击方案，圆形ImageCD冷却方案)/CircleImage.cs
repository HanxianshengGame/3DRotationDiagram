using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using System;
namespace Example2_2             //在CircleImage组件基础上实现技能CD效果，以及精确化点击区域。
{
    public class CircleImage:Image
    { 

        [SerializeField]
        private int segementCount = 100;             //将圆绘制成的3角块数
        private int circleVertexCount;      //圆顶点的数量
        [SerializeField]
        private float fillPercent = 1;           //填充度

        private List<UIVertex> uIVerticesList;

        /// <summary>
        /// 绘制顶点进行填充渲染
        /// </summary>
        /// <param name="vertexHelper"></param>
        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            uIVerticesList = new List<UIVertex>();
            circleVertexCount = segementCount + 2;         //渲染圆所需要的顶点数
            int meshVertexCount = Convert.ToInt32(segementCount * fillPercent + 2);  //渲染填充度为fillPercent的扇圆所需要的顶点数


            //重写后先将之前的vertexHelper存入的顶点信息和3角面信息清除
            vertexHelper.Clear();
            AddVertexs(vertexHelper, meshVertexCount);

            //vertexHelper添加3角面信息，在渲染流程中，以3个顶点顺时针添加的面渲染，3个顶点逆时针添加的面不渲染

            AddTriangle(vertexHelper);

        }

        /// <summary>
        /// 为VertexHelper添加三角面信息
        /// </summary>
        /// <param name="vertexHelper"></param>
        private void AddTriangle(VertexHelper vertexHelper)
        {
            for (int i = 1; i < circleVertexCount - 1; i++)
            {
                vertexHelper.AddTriangle(i, 0, i + 1);
            }
        }

        /// <summary>
        /// 为VertexHelper添加顶点信息
        /// </summary>
        /// <param name="vertexHelper"></param>
        /// <param name="meshVertexCount"></param>
        private void AddVertexs(VertexHelper vertexHelper, int meshVertexCount)
        {
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;
            Vector4 uv = (overrideSprite != null) ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;


            //利用当前UV坐标计算出uvCenter的坐标(uvCenterX,uvCenterY)
            float uvCenterX = (uv.x + uv.z) * 0.5f;
            float uvCenterY = (uv.y + uv.w) * 0.5f;

            //利用当前Image的宽高和UV宽高计算出    X,Y轴上的UV转换系数(uvScaleX,uvScaleY)(每1单位的position所占的uv值)
            float uvScaleX = (uv.z - uv.x) / width;
            float uvScaleY = (uv.w - uv.y) / height;


            //求每一块段区域所占圆的弧度(radian)与绘制的圆的半径(radius)
            float radian = 2 * Mathf.PI / segementCount;
            float radius = 0.5f * width;

            //创建圆心顶点，并给出Position，color，uvPosition(通过[position的x,y坐标  *   X,Y轴上的UV转换系数]可求得)
            Vector2 centerPos= new Vector2((0.5f - rectTransform.pivot.x) * width, (0.5f - rectTransform.pivot.y) * height);
            Vector2 centerUV0= new Vector2(uvCenterX, uvCenterY);
            Color32 centerColor;

            if (fillPercent < 1)
            {
                byte tempValue = (byte)(255 * fillPercent);
                centerColor = new Color32(tempValue, tempValue, tempValue, 255);
            }
            else
            {
                centerColor = color;
            }

            UIVertex centerVertex = GetUIVertex(centerPos, centerUV0, centerColor);
            vertexHelper.AddVert(centerVertex);

            float curRadian = 0;   //记作当前弧度，并添加其余圆弧上的顶点
            for (int i = 0; i < circleVertexCount - 1; i++)
            {
                float x = Mathf.Cos(curRadian) * radius;
                float y = Mathf.Sin(curRadian) * radius;
                Vector2 vertexPos= new Vector2(centerVertex.position.x + x, centerVertex.position.y + y);
                Vector2 vertexUV0= new Vector2(x * uvScaleX + uvCenterX, y * uvScaleY + uvCenterY);
                Color32 vertexColor;

                if (i < meshVertexCount - 1)
                    vertexColor = color;
                else
                    vertexColor = new Color32(0, 0, 0, 255);

                UIVertex vertex = GetUIVertex(vertexPos,vertexUV0,vertexColor);
                vertexHelper.AddVert(vertex);
                uIVerticesList.Add(vertex);
                curRadian += radian;
            }
        }

        /// <summary>
        /// 得到顶点信息
        /// </summary>
        /// <param name="meshVertexCount"></param>
        /// <param name="uvCenterX"></param>
        /// <param name="uvCenterY"></param>
        /// <param name="uvScaleX"></param>
        /// <param name="uvScaleY"></param>
        /// <param name="radius"></param>
        /// <param name="centerVertex"></param>
        /// <param name="curRadian"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private UIVertex GetUIVertex(Vector2 pos,Vector2 uvPos,Color32 color)
        {
            UIVertex vertex = new UIVertex();
            vertex.position = pos;
            vertex.color = color;
            vertex.uv0 = uvPos;
            return vertex;
        }


        /// <summary>
        /// 判段射线检测点是否有效
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <param name="eventCamera"></param>
        /// <returns></returns>
        public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out localPoint);
         //   Debug.Log(localPoint + " " + screenPoint);
            return ClickPointIsVaild(localPoint);
        }
        /// <summary>
        /// 点击点击是否有效
        /// </summary>
        /// <param name="localPoint"></param>
        /// <returns></returns>
        private bool ClickPointIsVaild(Vector2 localPoint)
        {
            return GetCrossPointNum(localPoint,uIVerticesList) % 2 == 1;
        }
        /// <summary>
        /// 得到交点数
        /// </summary>
        /// <param name="localPoint"></param>
        /// <param name="vertexList"></param>
        /// <returns></returns>
        private int GetCrossPointNum(Vector2 localPoint,List<UIVertex> vertexList )
        {
            UIVertex vertex1;
            UIVertex vertex2;
            int vertexsCount = vertexList.Count;
            int crossPointNum = 0;
            for (int i = 0; i < vertexsCount; i++)
            {
                vertex1 = vertexList[i];
                vertex2 = vertexList[(i + 1) % vertexsCount];
                if(IsYInRange(localPoint,vertex1,vertex2))
                {
                    float k = (vertex2.position.y - vertex1.position.y) / (vertex2.position.x - vertex1.position.x);
                    float b = vertex2.position.y - k * vertex2.position.x;
                    float x = (localPoint.y - b) / k;
                    if(x>=localPoint.x)
                    {
                        crossPointNum++;
                    }
                }
            }
            return crossPointNum;
        }

        /// <summary>
        /// 是否该坐标的Y值在范围内
        /// </summary>
        /// <param name="localPoint"></param>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <returns></returns>
        private bool IsYInRange(Vector2 localPoint,UIVertex vertex1,UIVertex vertex2)
        {
            if(vertex1.position.y>vertex2.position.y)
            {
                return localPoint.y >= vertex2.position.y && localPoint.y <= vertex1.position.y;
            }
            else
            {
                return localPoint.y >= vertex1.position.y && localPoint.y <= vertex2.position.y;
            }
        }

    }
}
