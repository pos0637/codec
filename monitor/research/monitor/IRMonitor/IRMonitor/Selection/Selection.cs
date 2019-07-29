using Common;
using System;
using System.Runtime.Serialization;
using System.Drawing;
using System.Collections.Generic;

namespace IRMonitor
{
    /// <summary>
    /// 区域类型
    /// </summary>
    public enum SelectionType
    {
        Unknown = 0,
        Point,
        Rectangle,
        Ellipse,
        Line
    }

    /// <summary>
    /// 线段的结构体
    /// </summary>
    public struct Segment
    {
        public Int32 mXStart;
        public Int32 mXEnd;
        public Int32 mYAxis;
    }

    /// <summary>
    /// 选定区域类
    /// </summary>
    [DataContract]
    public abstract class Selection
    {
        [DataMember(Name = "SelectionType")]
        public SelectionType mType; // 图形的类型

        [DataMember(Name = "SelectionId")]
        public Int64 mSelectionId; // 图形索引

        [DataMember(Name = "SelectionName")]
        public String mSelectionName;

        [DataMember(Name = "IsGlobalSelection")]
        public Boolean mIsGlobalSelection = false;

        [DataMember(Name = "AlarmConfig")]
        public SelectionAlarmConfig mAlarmConfig = new SelectionAlarmConfig();

        [NonSerialized]
        public SelectionTempData mTemperatureData = new SelectionTempData();

        [NonSerialized]
        public SelectionAlarmInfo mAlarmInfo = new SelectionAlarmInfo();

        [NonSerialized]
        public Int32 mPixelCount; // 区域内像素点的数量

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="SelectionType">图形类型</param>
        public Selection(SelectionType SelectionType)
        {
            mType = SelectionType;
            mPixelCount = 1; // 像素点数量
        }

        /// <summary>
        /// 获取图形类型
        /// </summary>
        /// <param name="data">图形信息</param>
        /// <returns>图形类型</returns>
        public static SelectionType GetType(
             String data)
        {
            if (String.IsNullOrEmpty(data))
                return SelectionType.Unknown;

            Int32 pos = data.IndexOf("SelectionType") + "SelectionType".Length + 2;
            if (pos < 0)
                return SelectionType.Unknown;

            String s = data.Substring(pos);
            pos = s.IndexOf(",");
            if (pos < 0)
                return SelectionType.Unknown;

            s = s.Substring(0, pos);
            Int32 type;
            if (!Int32.TryParse(s, out type))
                return SelectionType.Unknown;
            else
                return ((SelectionType)type);
        }

        /// <summary>
        /// 创建区域
        /// </summary>
        /// <returns>创建结果</returns>
        public virtual ARESULT MakeSelectionArea()
        {
            return ARESULT.E_FAIL;
        }

        /// <summary>
        /// 计算图形区域内的值
        /// </summary>
        /// <param name="buffer">图形</param>
        /// <param name="width">图形宽度</param>
        /// <param name="stride">图形对齐宽度</param>
        /// <param name="height">图形高度</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="average">平均值</param>
        /// <param name="minPoint">最小值位置</param>
        /// <param name="maxPoint">最大值位置</param>
        /// <returns>计算结果</returns>
        public virtual ARESULT CalcSelectionAreaValue(
             Single[] buffer,
             Int32 width,
             Int32 stride,
             Int32 height)
        {
            return ARESULT.E_FAIL;
        }

        /// <summary>
        /// 序列化图形信息
        /// </summary>
        /// <param name="buffer">传入一块内存</param>
        /// <param name="length">内存大小</param>
        /// <param name="buffer">传出写入的长度</param>
        /// <returns>序列化信息串</returns>
        public virtual String Serialize()
        {
            return null;
        }

        /// <summary>
        /// 反序列化图形信息
        /// </summary>
        /// <param name="data">序列化信息串</param>
        /// <returns>结果</returns>
        public virtual ARESULT Deserialize(
            String data)
        {
            return ARESULT.E_FAIL;
        }

        private Selection()
        {
        }
    }

    /// <summary>
    /// 点
    /// </summary>
    [DataContract]
    public class PointSelection : Selection
    {
        [DataMember(Name = "Point")]
        public Point mPoint; // 点坐标

        public PointSelection()
            : base(SelectionType.Point)
        {
        }

        public override ARESULT MakeSelectionArea()
        {
            mPixelCount = 1;
            return ARESULT.S_OK;
        }

        public override ARESULT CalcSelectionAreaValue(
             Single[] buffer,
             Int32 width,
             Int32 stride,
             Int32 height)
        {
            if (buffer == null)
                return ARESULT.E_INVALIDARG;

            Single temp = buffer[stride * mPoint.Y + mPoint.X];
            if (mTemperatureData == null) {
                mTemperatureData = new SelectionTempData();
                if (mTemperatureData == null)
                    return ARESULT.E_FAIL;
            }

            mTemperatureData.mMinTemperature = temp;
            mTemperatureData.mMaxTemperature = temp;
            mTemperatureData.mAvgTemperature = temp;

            mTemperatureData.mMinPoint.X = mPoint.X;
            mTemperatureData.mMinPoint.Y = mPoint.Y;

            mTemperatureData.mMaxPoint.X = mPoint.X;
            mTemperatureData.mMaxPoint.Y = mPoint.Y;

            return ARESULT.S_OK;
        }

        public override String Serialize()
        {
            return JsonUtils.ObjectToJson(this);
        }

        public override ARESULT Deserialize(
            String data)
        {
            PointSelection selection;
            try {
                selection = JsonUtils.ObjectFromJson<PointSelection>(data);
            }
            catch {
                return ARESULT.E_FAIL;
            }

            if (selection == null)
                return ARESULT.E_FAIL;

            mSelectionId = selection.mSelectionId;
            mSelectionName = selection.mSelectionName;
            mType = selection.mType;
            mPoint = selection.mPoint;
            mAlarmConfig = selection.mAlarmConfig;

            return MakeSelectionArea();
        }

    };

    /// <summary>
    /// 矩形
    /// </summary>
    [DataContract]
    public class RectangleSelection : Selection
    {
        [DataMember(Name = "Rectangle")]
        public Rectangle mRectangle;

        public RectangleSelection()
            : base(SelectionType.Rectangle)
        {
        }

        public override ARESULT MakeSelectionArea()
        {
            mPixelCount = mRectangle.Width * mRectangle.Height;

            return ARESULT.S_OK;
        }

        public override ARESULT CalcSelectionAreaValue(
             Single[] buffer,
             Int32 width,
             Int32 stride,
             Int32 height)
        {
            if (buffer == null)
                return ARESULT.E_INVALIDARG;

            Single temp = 0;
            Int32 position = stride * mRectangle.Top + mRectangle.Left;
            Single tempMin = buffer[position];
            Single tempMax = buffer[position];
            Point minP = new Point(mRectangle.Left, mRectangle.Top);
            Point maxP = new Point(mRectangle.Left, mRectangle.Top);

            Single totalGray = 0;
            Int32 pitch = stride * mRectangle.Top;

            for (Int32 m = mRectangle.Top; m < mRectangle.Bottom; ++m) {
                for (Int32 n = mRectangle.Left; n < mRectangle.Right; ++n) {
                    temp = buffer[pitch + n];
                    if (tempMin > temp) {
                        tempMin = temp;
                        minP.X = n;
                        minP.Y = m;
                    }
                    else if (tempMax < temp) {
                        tempMax = temp;
                        maxP.X = n;
                        maxP.Y = m;
                    }
                    totalGray += temp;
                }
                pitch += stride;
            }

            mTemperatureData.mMinTemperature = tempMin;
            mTemperatureData.mMaxTemperature = tempMax;
            mTemperatureData.mAvgTemperature = totalGray / mPixelCount;

            mTemperatureData.mMinPoint.X = minP.X;
            mTemperatureData.mMinPoint.Y = minP.Y;

            mTemperatureData.mMaxPoint.X = maxP.X;
            mTemperatureData.mMaxPoint.Y = maxP.Y;

            return ARESULT.S_OK;
        }

        public override String Serialize()
        {
            return JsonUtils.ObjectToJson(this);
        }

        public override ARESULT Deserialize(
            String data)
        {
            RectangleSelection selection;
            try {
                selection = JsonUtils.ObjectFromJson<RectangleSelection>(data);
            }
            catch {
                return ARESULT.E_FAIL;
            }

            if (selection == null)
                return ARESULT.E_FAIL;

            mIsGlobalSelection = selection.mIsGlobalSelection;
            mSelectionId = selection.mSelectionId;
            mSelectionName = selection.mSelectionName;
            mType = selection.mType;
            mRectangle = selection.mRectangle;
            mAlarmConfig = selection.mAlarmConfig;

            return MakeSelectionArea();
        }
    }

    /// <summary>
    /// 椭圆
    /// </summary>
    [DataContract]
    public class EllipseSelection : Selection
    {
        [DataMember(Name = "Rectangle")]
        public Rectangle mRectangle;

        [NonSerialized]
        protected Int32 mXAxis; // X轴

        [NonSerialized]
        protected Int32 mYAxis; // Y轴

        [NonSerialized]
        protected Point mCentrePoint; // 圆心

        protected Segment[] mArea; // 椭圆所在区域点的数组

        public EllipseSelection()
            : base(SelectionType.Ellipse)
        {
            mArea = null;
        }

        public override ARESULT MakeSelectionArea()
        {
            mXAxis = mRectangle.Width;
            mYAxis = mRectangle.Height;
            mCentrePoint.X = mRectangle.X + mRectangle.Width / 2;
            mCentrePoint.Y = mRectangle.Y + mRectangle.Height / 2;

            Int32 length = mYAxis + 1;
            Int32 startY = mCentrePoint.Y - (mYAxis / 2);

            // 申请存放图形区域的动态数组
            mArea = new Segment[length];
            if (mArea == null)
                return ARESULT.E_OUTOFMEMORY;

            Double ellipseA, ellipseB, ellipseX, ellipseY;

            ellipseA = mXAxis / 2;
            ellipseA *= ellipseA;
            ellipseB = mYAxis / 2;
            ellipseB *= ellipseB;
            mPixelCount = 0;

            // 获取椭圆图形区域
            for (Int32 i = 0; i < length; ++i) {
                ellipseY = (Double)mCentrePoint.Y - startY - i;
                ellipseY *= ellipseY;
                ellipseX = ellipseA - ((ellipseY * ellipseA) / ellipseB);
                ellipseX = Math.Sqrt(ellipseX);
                mArea[i].mXStart = (Int32)(mCentrePoint.X - ellipseX);
                mArea[i].mXEnd = (Int32)(mCentrePoint.X + ellipseX);
                mArea[i].mYAxis = (Int32)(startY + i);
                mPixelCount += mArea[i].mXEnd - mArea[i].mXStart;
            }

            return ARESULT.S_OK;
        }

        public override ARESULT CalcSelectionAreaValue(
             Single[] buffer,
             Int32 width,
             Int32 stride,
             Int32 height)
        {
            if (buffer == null)
                return ARESULT.E_INVALIDARG;

            if (mArea == null)
                return ARESULT.E_INVALID_OPERATION;

            Int32 length = mYAxis + 1;
            Single temp = 0;
            Int32 position = mArea[0].mXStart + stride * mArea[0].mYAxis;
            Single tempMin = buffer[position];
            Single tempMax = buffer[position];
            Point minP = new Point(), maxP = new Point();
            minP.X = mArea[0].mXStart;
            minP.Y = mArea[0].mYAxis;
            maxP.X = mArea[0].mXStart;
            maxP.Y = mArea[0].mYAxis;
            Single totalGray = 0;
            Int32 pitch = stride * mArea[0].mYAxis;

            for (Int32 m = 0; m < length; ++m) {
                for (Int32 n = mArea[m].mXStart; n < mArea[m].mXEnd; ++n) {
                    temp = buffer[n + pitch];
                    if (tempMin > temp) {
                        tempMin = temp;
                        minP.X = n;
                        minP.Y = mArea[m].mYAxis;
                    }
                    else if (tempMax < temp) {
                        tempMax = temp;
                        maxP.X = n;
                        maxP.Y = mArea[m].mYAxis;
                    }
                    totalGray += temp;
                }
                pitch += stride;
            }

            mTemperatureData.mMinTemperature = tempMin;
            mTemperatureData.mMaxTemperature = tempMax;
            mTemperatureData.mAvgTemperature = totalGray / mPixelCount;

            mTemperatureData.mMinPoint.X = minP.X;
            mTemperatureData.mMinPoint.Y = minP.Y;

            mTemperatureData.mMaxPoint.X = maxP.X;
            mTemperatureData.mMaxPoint.Y = maxP.Y;

            return ARESULT.S_OK;
        }

        public override String Serialize()
        {
            return JsonUtils.ObjectToJson(this);
        }

        public override ARESULT Deserialize(
            String data)
        {
            EllipseSelection selection;
            try {
                selection = JsonUtils.ObjectFromJson<EllipseSelection>(data);
            }
            catch {
                return ARESULT.E_FAIL;
            }

            if (selection == null)
                return ARESULT.E_FAIL;

            mSelectionId = selection.mSelectionId;
            mSelectionName = selection.mSelectionName;
            mType = selection.mType;
            mRectangle = selection.mRectangle;
            mAlarmConfig = selection.mAlarmConfig;

            return MakeSelectionArea();
        }
    }

    /// <summary>
    /// 线
    /// </summary>
    [DataContract]
    public class LineSelection : Selection
    {
        [DataMember(Name = "Line")]
        public List<Point> mPoint = new List<Point>();

        [NonSerialized]
        public Point mStartPoint; // 起始点

        [NonSerialized]
        public Point mEndPoint; // 结束点

        public LineSelection()
            : base(SelectionType.Line)
        {
        }

        public override ARESULT MakeSelectionArea()
        {
            if ((mPoint == null) || (mPoint.Count < 2))
                return ARESULT.E_FAIL;

            mStartPoint = mPoint[0];
            mEndPoint = mPoint[1];

            Int32 x = mEndPoint.X - mStartPoint.X;
            Int32 y = mEndPoint.Y - mEndPoint.Y;
            mPixelCount = (Int32)Math.Sqrt(x * x + y * y) + 1;

            return ARESULT.S_OK;
        }

        public override ARESULT CalcSelectionAreaValue(
             Single[] buffer,
             Int32 width,
             Int32 stride,
             Int32 height)
        {
            if (buffer == null)
                return ARESULT.E_INVALIDARG;

            Int32 position = stride * mStartPoint.Y + mStartPoint.X;
            Single tempMin = buffer[position];
            Single tempMax = buffer[position];
            Point minP = new Point(mStartPoint.X, mStartPoint.Y);
            Point maxP = new Point(mStartPoint.X, mStartPoint.Y);

            // a:直线斜率
            Single a;
            if (mStartPoint.X != mEndPoint.X)
                a = (mStartPoint.Y - mEndPoint.Y) * 1.0f / (mStartPoint.X - mEndPoint.X);
            else
                a = 0;

            // b:直线截距
            Single b = mEndPoint.Y - a * mEndPoint.X;

            Int32 beginX = mStartPoint.X > mEndPoint.X ? mEndPoint.X : mStartPoint.X;
            Int32 endX = mStartPoint.X < mEndPoint.X ? mEndPoint.X : mStartPoint.X;
            Int32 beginY = mStartPoint.Y > mEndPoint.Y ? mEndPoint.Y : mStartPoint.Y;
            Int32 endY = mStartPoint.Y < mEndPoint.Y ? mEndPoint.Y : mStartPoint.Y;
            Int32 pitch = stride * beginY;
            Single totalGray = 0;

            for (Int32 m = beginY; m <= endY; ++m) {
                for (Int32 n = beginX; n <= endX; ++n) {
                    if ((Int32)(a * n + b) == m) {
                        Single val = buffer[pitch + n];
                        if (tempMin > val) {
                            tempMin = val;
                            minP.X = n;
                            minP.Y = m;
                        }
                        else if (tempMax < val) {
                            tempMax = val;
                            maxP.X = n;
                            maxP.Y = m;
                        }
                        totalGray += val;
                    }
                }
                pitch += stride;
            }

            mTemperatureData.mMinTemperature = tempMin;
            mTemperatureData.mMaxTemperature = tempMax;
            mTemperatureData.mAvgTemperature = totalGray / mPixelCount;

            mTemperatureData.mMinPoint.X = minP.X;
            mTemperatureData.mMinPoint.Y = minP.Y;

            mTemperatureData.mMaxPoint.X = maxP.X;
            mTemperatureData.mMaxPoint.Y = maxP.Y;

            return ARESULT.S_OK;
        }

        public override String Serialize()
        {
            return JsonUtils.ObjectToJson(this);
        }

        public override ARESULT Deserialize(
            String data)
        {
            LineSelection selection;
            try {
                selection = JsonUtils.ObjectFromJson<LineSelection>(data);
            }
            catch {
                return ARESULT.E_FAIL;
            }

            if (selection == null)
                return ARESULT.E_FAIL;

            mSelectionId = selection.mSelectionId;
            mSelectionName = selection.mSelectionName;
            mType = selection.mType;
            mPoint = selection.mPoint;
            mAlarmConfig = selection.mAlarmConfig;

            return MakeSelectionArea();
        }
    }
}
