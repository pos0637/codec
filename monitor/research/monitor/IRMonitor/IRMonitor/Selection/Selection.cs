using Common;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
        public int mXStart;
        public int mXEnd;
        public int mYAxis;
    }

    /// <summary>
    /// 选定区域类
    /// </summary>
    [DataContract]
    public abstract class Selection
    {
        /// <summary>
        /// 类型
        /// </summary>
        [DataMember(Name = "SelectionType")]
        public SelectionType mType;

        /// <summary>
        /// 索引
        /// </summary>
        [DataMember(Name = "SelectionId")]
        public long mSelectionId;

        /// <summary>
        /// 名称
        /// </summary>
        [DataMember(Name = "SelectionName")]
        public string mSelectionName;

        /// <summary>
        /// 是否为全局选区
        /// </summary>
        [DataMember(Name = "IsGlobalSelection")]
        public bool mIsGlobalSelection = false;

        /// <summary>
        /// 告警配置
        /// </summary>
        [DataMember(Name = "AlarmConfig")]
        public SelectionAlarmConfig mAlarmConfig = new SelectionAlarmConfig();

        /// <summary>
        /// 温度数据
        /// </summary>
        [NonSerialized]
        public SelectionTemperature mTemperatureData = new SelectionTemperature();

        /// <summary>
        /// 告警信息
        /// </summary>
        [NonSerialized]
        public SelectionAlarmInfo mAlarmInfo = new SelectionAlarmInfo();

        /// <summary>
        /// 区域内像素点的数量
        /// </summary>
        [NonSerialized]
        public int mPixelCount;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="SelectionType">图形类型</param>
        public Selection(SelectionType SelectionType)
        {
            mType = SelectionType;
            mPixelCount = 1;
        }

        /// <summary>
        /// 获取图形类型
        /// </summary>
        /// <param name="data">图形信息</param>
        /// <returns>图形类型</returns>
        public static SelectionType GetType(string data)
        {
            if (string.IsNullOrEmpty(data)) {
                return SelectionType.Unknown;
            }

            int pos = data.IndexOf("SelectionType") + "SelectionType".Length + 2;
            if (pos < 0) {
                return SelectionType.Unknown;
            }

            string s = data.Substring(pos);
            pos = s.IndexOf(",");
            if (pos < 0) {
                return SelectionType.Unknown;
            }

            s = s.Substring(0, pos);
            if (!int.TryParse(s, out int type)) {
                return SelectionType.Unknown;
            }
            else {
                return ((SelectionType)type);
            }
        }

        /// <summary>
        /// 设置索引
        /// </summary>
        /// <param name="id">索引</param>
        /// <returns>选定区域</returns>
        public virtual Selection SetId(long id)
        {
            mSelectionId = id;
            mTemperatureData.mSelectionId = id;

            return this;
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
        /// 计算温度
        /// </summary>
        /// <param name="buffer">温度数据</param>
        /// <param name="width">宽度</param>
        /// <param name="stride">对齐宽度</param>
        /// <param name="height">高度</param>
        /// <returns>计算结果</returns>
        public virtual ARESULT CalcTemperature(
             float[] buffer,
             int width,
             int stride,
             int height)
        {
            return ARESULT.E_FAIL;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns>序列化信息串</returns>
        public virtual string Serialize()
        {
            return null;
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="data">序列化信息串</param>
        /// <returns>结果</returns>
        public virtual ARESULT Deserialize(string data)
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

        public override ARESULT CalcTemperature(
             float[] buffer,
             int width,
             int stride,
             int height)
        {
            if (buffer == null) {
                return ARESULT.E_INVALIDARG;
            }

            float temp = buffer[stride * mPoint.Y + mPoint.X];
            if (mTemperatureData == null) {
                mTemperatureData = new SelectionTemperature();
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

        public override string Serialize()
        {
            return JsonUtils.ObjectToJson(this);
        }

        public override ARESULT Deserialize(string data)
        {
            PointSelection selection;
            try {
                selection = JsonUtils.ObjectFromJson<PointSelection>(data);
                if (selection == null) {
                    return ARESULT.E_FAIL;
                }
            }
            catch {
                return ARESULT.E_FAIL;
            }

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
        [DataMember]
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

        public override ARESULT CalcTemperature(
             float[] buffer,
             int width,
             int stride,
             int height)
        {
            if (buffer == null) {
                return ARESULT.E_INVALIDARG;
            }

            int position = stride * mRectangle.Top + mRectangle.Left;
            float tempMin = buffer[position];
            float tempMax = buffer[position];
            Point minP = new Point(mRectangle.Left, mRectangle.Top);
            Point maxP = new Point(mRectangle.Left, mRectangle.Top);

            float totalGray = 0;
            int pitch = stride * mRectangle.Top;

            for (int m = mRectangle.Top; m < mRectangle.Bottom; ++m) {
                for (int n = mRectangle.Left; n < mRectangle.Right; ++n) {
                    float temp = buffer[pitch + n];
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

        public override string Serialize()
        {
            return JsonUtils.ObjectToJson(this);
        }

        public override ARESULT Deserialize(string data)
        {
            RectangleSelection selection;
            try {
                selection = JsonUtils.ObjectFromJson<RectangleSelection>(data);
                if (selection == null) {
                    return ARESULT.E_FAIL;
                }
            }
            catch {
                return ARESULT.E_FAIL;
            }

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

        /// <summary>
        /// X轴
        /// </summary>
        [NonSerialized]
        protected int mXAxis;

        /// <summary>
        /// Y轴
        /// </summary>
        [NonSerialized]
        protected int mYAxis;

        /// <summary>
        /// 圆心
        /// </summary>
        [NonSerialized]
        protected Point mCentrePoint;

        /// <summary>
        /// 椭圆所在区域点的数组
        /// </summary>
        protected Segment[] mArea;

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

            int length = mYAxis + 1;
            int startY = mCentrePoint.Y - (mYAxis / 2);

            // 申请存放图形区域的动态数组
            mArea = new Segment[length];
            double ellipseA, ellipseB, ellipseX, ellipseY;

            ellipseA = mXAxis / 2;
            ellipseA *= ellipseA;
            ellipseB = mYAxis / 2;
            ellipseB *= ellipseB;
            mPixelCount = 0;

            // 获取椭圆图形区域
            for (int i = 0; i < length; ++i) {
                ellipseY = (double)mCentrePoint.Y - startY - i;
                ellipseY *= ellipseY;
                ellipseX = ellipseA - ((ellipseY * ellipseA) / ellipseB);
                ellipseX = Math.Sqrt(ellipseX);
                mArea[i].mXStart = (int)(mCentrePoint.X - ellipseX);
                mArea[i].mXEnd = (int)(mCentrePoint.X + ellipseX);
                mArea[i].mYAxis = (int)(startY + i);
                mPixelCount += mArea[i].mXEnd - mArea[i].mXStart;
            }

            return ARESULT.S_OK;
        }

        public override ARESULT CalcTemperature(
             float[] buffer,
             int width,
             int stride,
             int height)
        {
            if (buffer == null) {
                return ARESULT.E_INVALIDARG;
            }

            if (mArea == null) {
                return ARESULT.E_INVALID_OPERATION;
            }

            int length = mYAxis + 1;
            int position = mArea[0].mXStart + stride * mArea[0].mYAxis;
            float tempMin = buffer[position];
            float tempMax = buffer[position];
            Point minP = new Point {
                X = mArea[0].mXStart,
                Y = mArea[0].mYAxis
            };
            Point maxP = new Point {
                X = mArea[0].mXStart,
                Y = mArea[0].mYAxis
            };
            float totalGray = 0;
            int pitch = stride * mArea[0].mYAxis;

            for (int m = 0; m < length; ++m) {
                for (int n = mArea[m].mXStart; n < mArea[m].mXEnd; ++n) {
                    float temp = buffer[n + pitch];
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

        public override string Serialize()
        {
            return JsonUtils.ObjectToJson(this);
        }

        public override ARESULT Deserialize(string data)
        {
            EllipseSelection selection;
            try {
                selection = JsonUtils.ObjectFromJson<EllipseSelection>(data);
                if (selection == null) {
                    return ARESULT.E_FAIL;
                }
            }
            catch {
                return ARESULT.E_FAIL;
            }

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
            if ((mPoint == null) || (mPoint.Count < 2)) {
                return ARESULT.E_FAIL;
            }

            mStartPoint = mPoint[0];
            mEndPoint = mPoint[1];

            int x = mEndPoint.X - mStartPoint.X;
            int y = mEndPoint.Y - mEndPoint.Y;
            mPixelCount = (int)Math.Sqrt(x * x + y * y) + 1;

            return ARESULT.S_OK;
        }

        public override ARESULT CalcTemperature(
             float[] buffer,
             int width,
             int stride,
             int height)
        {
            if (buffer == null) {
                return ARESULT.E_INVALIDARG;
            }

            int position = stride * mStartPoint.Y + mStartPoint.X;
            float tempMin = buffer[position];
            float tempMax = buffer[position];
            Point minP = new Point(mStartPoint.X, mStartPoint.Y);
            Point maxP = new Point(mStartPoint.X, mStartPoint.Y);

            // a:直线斜率
            float a;
            if (mStartPoint.X != mEndPoint.X) {
                a = (mStartPoint.Y - mEndPoint.Y) * 1.0f / (mStartPoint.X - mEndPoint.X);
            }
            else {
                a = 0;
            }

            // b:直线截距
            float b = mEndPoint.Y - a * mEndPoint.X;
            int beginX = mStartPoint.X > mEndPoint.X ? mEndPoint.X : mStartPoint.X;
            int endX = mStartPoint.X < mEndPoint.X ? mEndPoint.X : mStartPoint.X;
            int beginY = mStartPoint.Y > mEndPoint.Y ? mEndPoint.Y : mStartPoint.Y;
            int endY = mStartPoint.Y < mEndPoint.Y ? mEndPoint.Y : mStartPoint.Y;
            int pitch = stride * beginY;
            float totalGray = 0;

            for (int m = beginY; m <= endY; ++m) {
                for (int n = beginX; n <= endX; ++n) {
                    if ((int)(a * n + b) == m) {
                        float val = buffer[pitch + n];
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

        public override string Serialize()
        {
            return JsonUtils.ObjectToJson(this);
        }

        public override ARESULT Deserialize(string data)
        {
            LineSelection selection;
            try {
                selection = JsonUtils.ObjectFromJson<LineSelection>(data);
                if (selection == null) {
                    return ARESULT.E_FAIL;
                }
            }
            catch {
                return ARESULT.E_FAIL;
            }

            mSelectionId = selection.mSelectionId;
            mSelectionName = selection.mSelectionName;
            mType = selection.mType;
            mPoint = selection.mPoint;
            mAlarmConfig = selection.mAlarmConfig;

            return MakeSelectionArea();
        }
    }
}
