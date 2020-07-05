@[TOC]( UGUI实现3D轮转图效果?)

<br>


# (一)  &ensp;3D轮转图效果展示
<br>

**&ensp;&ensp;&ensp;<font size=4>在我们制作一些场景选关或者是做一些选择类似的UI时，我们会觉得想制作一种3D效果的选择方式，3D轮转图往往就是最恰当的一种,下面我将会介绍3D轮转图的具体原理及实现：**

![在这里插入图片描述](https://img-blog.csdnimg.cn/20190915173933282.gif)





---



<br>
<br>

# (二)  &ensp;3D轮转图实现
<br>
<br>

### <font color=red> 实现原理
  **<font color=black> &ensp;&ensp;&ensp;&ensp;利用父物体生成子物体选项，子物体利用x轴上的间距和 最中心靠前的物体最大显示(缩放最大)，两侧物体逐渐变小(缩放逐渐变小)的原理制造视觉差，从而形成3D效果，之后利用DragHandler事件制作拖动旋转即可，**
   
   <br>
   
**原理1. 关于3D轮转图的Item的Position讲解**（图片放大后观看）：



3D图讲解
![在这里插入图片描述](https://img-blog.csdnimg.cn/20190915183409648.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2Nob25nemlfZGFpbWE=,size_16,color_FFFFFF,t_70)
排好Position的2D图显示
![在这里插入图片描述](https://img-blog.csdnimg.cn/20190915201322194.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2Nob25nemlfZGFpbWE=,size_16,color_FFFFFF,t_70)
**原理2： 关于3D轮转图的Item的Scale讲解**（图片放大后进行观看）：
3D图讲解

![在这里插入图片描述](https://img-blog.csdnimg.cn/20190915203358475.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2Nob25nemlfZGFpbWE=,size_16,color_FFFFFF,t_70)
排好scale和position的2D图显示：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20190915203610986.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2Nob25nemlfZGFpbWE=,size_16,color_FFFFFF,t_70)
<br>

### <font color=red> 实现步骤
<br>
&nbsp;&nbsp;这里只讲一些原理步骤，代码都在github上，自行下载学习观看！！！


<br>

**（1）创造子Item物体：创建你需要的选择item物体们，直接在父物体下写这个2D模拟3D轮转图的脚本即可，可以选择公开子物体属性，例如：item间距，缩放最大最小值，数量，对应的sprite，size。**

![在这里插入图片描述](https://img-blog.csdnimg.cn/20190915204959725.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2Nob25nemlfZGFpbWE=,size_16,color_FFFFFF,t_70)
<br>
**（2）&ensp;Item坐标缩放分配：原理在上面已经讲过，按照上面的缩放，坐标分配原理，对item的anchorPosition，localscale进行赋值，之和就能得到不能拖动的轮转图基本样子。**
```
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
```

![在这里插入图片描述](https://img-blog.csdnimg.cn/20190915210355595.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2Nob25nemlfZGFpbWE=,size_16,color_FFFFFF,t_70)
<br>

**（3）**<font size=4.5>  **层级分配：我们发现后面的图片会进行遮挡第一张图片的显示，这是因为image的层级问题，我们采用的为自然层级解决这个问题，<font color=red>利用transform.SetSiblingIndex(order) 设置子物体在父物体下的角标，角标越大越后渲染，显示在最前方。**


自然层级：即生成在父物体下的子物体排在前面的先渲染，后面的后渲染，后渲染的会进行遮挡先渲染的image，因此我们就是改变子物体在父物体下的先后排序问题。
![在这里插入图片描述](https://img-blog.csdnimg.cn/20190915210814175.png)
![在这里插入图片描述](https://img-blog.csdnimg.cn/20190915210843918.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2Nob25nemlfZGFpbWE=,size_16,color_FFFFFF,t_70)
<br>
**（4）**<font size=4.5>  **拖动Item实现：子物体实现IDragHandler与IDragEndHandler，之和利用累加的  <font color=red>offsetX += eventData.delta.x
判断拖动结束后offsetX的正负，正即向右拖动，</font>之后将全部item的信息全部替换为相邻右边的image的信息，在改变缩放和position可以选用dotween进行完成，比较适合ui动画。**
![在这里插入图片描述](https://img-blog.csdnimg.cn/20190915211507677.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2Nob25nemlfZGFpbWE=,size_16,color_FFFFFF,t_70)
**（5）**<font size=4.5>  **完成效果展示**：
![在这里插入图片描述](https://img-blog.csdnimg.cn/20190915173933282.gif)



<br>


---


<br>
<br>
<br>
<br>


# (三) 项目工程链接地址(GitHub)
<br>

求github小星星哈，谢谢啦?
[3DRotationDiagram项目工程链接](https://github.com/HanxianshengGame/3DRotationDiagram)(https://github.com/HanxianshengGame/3DRotationDiagram)


**参考 scene Example 3与Script Example3 进行学习：**


![在这里插入图片描述](https://img-blog.csdnimg.cn/20190915211630271.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2Nob25nemlfZGFpbWE=,size_16,color_FFFFFF,t_70)
![在这里插入图片描述](https://img-blog.csdnimg.cn/20190915211643516.png?x-oss-process=image/watermark,type_ZmFuZ3poZW5naGVpdGk,shadow_10,text_aHR0cHM6Ly9ibG9nLmNzZG4ubmV0L2Nob25nemlfZGFpbWE=,size_16,color_FFFFFF,t_70)

<br>
<br>

 --------------------------------------------------------我是有底线的-------------------------------------------------------------
  
 &ensp;&ensp;&ensp;&ensp;感谢能够观看博客的各位Unity开发爱好者们，有问题发表评论呐，*★,°*:.☆(￣▽￣)/$:*.°★* 。                                                      
