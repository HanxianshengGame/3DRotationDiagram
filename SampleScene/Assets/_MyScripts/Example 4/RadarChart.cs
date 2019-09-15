using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace _MyScripts.Example_4
{
    public class RadarChart : Image
    {
        // Start is called before the first frame update
        [SerializeField]
        private int _pointCount;
        [SerializeField]
        private List<RectTransform> _points;
        [SerializeField]
        private Vector2 _pointSize = new Vector2(10, 10);
        [SerializeField]
        private Sprite _pointSprite;
        [SerializeField]
        private Color _pointColor = Color.white;
        [SerializeField]
        private List<RadarChartHandler> _handlers;
        [SerializeField]
        private float[] _handlerRadio;

        private void Update()
        {
            SetVerticesDirty();         //实时对顶点数据刷新
        }

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();
           AddVerts(vertexHelper);
           // AddVertsPlate(vertexHelper);
            AddTriangles(vertexHelper);
        }

        /// <summary>
        /// 添加顶点
        /// </summary>
        /// <param name="vh"></param>
        private void AddVerts(VertexHelper vh)
        {
            vh.AddVert(transform.localPosition,color,Vector2.one);
            foreach (var handler in _handlers)
            {
               vh.AddVert(handler.transform.localPosition, color, Vector2.zero);
            }
        }

        /// <summary>
        /// 添加顶点模板 （制作带sprite的雷达图效果）
        /// </summary>
        /// <param name="vh"></param>
        private void AddVertsPlate(VertexHelper vh)
        {
            vh.AddVert(_handlers[0].transform.localPosition,color,new Vector2(0.5f,1.0f));
            vh.AddVert(_handlers[1].transform.localPosition,color,new Vector2(0f,1.0f));
            vh.AddVert(_handlers[2].transform.localPosition,color,new Vector2(0f,0f));
            vh.AddVert(_handlers[3].transform.localPosition,color,new Vector2(1.0f,0f));
            vh.AddVert(_handlers[4].transform.localPosition,color,new Vector2(1.0f,1.0f));
        }

        private void AddTriangles(VertexHelper vh)      //UI.Default.shader 默认剔除是关闭的,模型黑面,很奇怪（法向量搞反）
        {
            for (int i = 1; i <=_pointCount-1; i++)
            {
                 vh.AddTriangle(0,i+1,i);
            }
            vh.AddTriangle(0,1,5);
        }

        public void InitPoints()
        {
            ClearPoints();
            _points = new List<RectTransform>();
            _handlers = new List<RadarChartHandler>();
            SpawnPoints();
            SetPointPos();
        }
        private void SpawnPoints()  //生成 point
        {
            for (var i = 0; i < _pointCount; i++)
            {
                var point = new GameObject("Point" + i);
                point.transform.SetParent(transform);
                _points.Add(point.AddComponent<RectTransform>());
            }
        }
        private void ClearPoints()
        {
            if (_points == null)     return;
            foreach (var point in _points.Where(point => point != null))
            {
                DestroyImmediate(point);         //在编译时也能进行
            }
        }
        private void ClearHandles()
        {
            if (_handlers == null)   return;
            foreach (var handler in _handlers.Where(handler => handler != null))
            {
                DestroyImmediate(handler);         //在编译时也能进行
            }
        }
        private void SetPointPos()          //设置点坐标
        {
            float radian = 2 * Mathf.PI / _pointCount;  //计算出当前多边形的每一段对应填充圆的弧长
            float radius = 130; //暂定当前正多边形的所填充圆的半径为100.
            float curRadian = 2 * Mathf.PI / 4.0f;     //计算出初始弧度位置 为正五边形的正上方  （起点选择与坐标轴延伸线相接的地方）
            for (int i = 0; i < _pointCount; i++)
            {
                float x = Mathf.Cos(curRadian) * radius;
                float y = Mathf.Sin(curRadian) * radius;
                curRadian += radian;
                _points[i].anchoredPosition = new Vector2(x, y);
            }
        }
        public void InitHandlers()
        {
            ClearHandles();
            SpawnHandlers();
            SetHandlerPos();
        }
        private void SpawnHandlers()
        {
            for (int i = 0; i < _pointCount; i++)
            {
                var go = new GameObject("Handler" + i);
                go.AddComponent<RectTransform>();
                go.AddComponent<Image>();
                var handler = go.AddComponent<RadarChartHandler>();
                handler.SetParent(transform);
                handler.ChangeColor(_pointColor);
                handler.ChangeSprite(_pointSprite);
                handler.SetSize(_pointSize);
                _handlers.Add(handler);
            }
        }
        private void SetHandlerPos()
        {
            if(_handlerRadio==null || _handlerRadio.Length!=_pointCount)
            {
                for (var i = 0; i < _pointCount; i++)
                {
                    _handlers[i].SetPos(_points[i].anchoredPosition);
                }
            }
            else
            {
                for (var i = 0; i < _pointCount; i++)
                {
                    _handlers[i].SetPos(_points[i].anchoredPosition * _handlerRadio[i]);
                }
            }
        }
    }
}
