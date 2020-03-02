using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace DrawTools
{
    /// <summary>
    /// Point graphic object
    /// </summary>
    //[Serializable]
    public class DrawPoint : DrawObject
    {
        private Point Point;

        private const string entryPoint = "Point";

        public DrawPoint()
        {
            Point.X = 0;
            Point.Y = 0;
            ZOrder = 0;
            Initialize();
        }

        public DrawPoint(int x1, int y1)
        {
            Point.X = x1;
            Point.Y = y1;
            ZOrder = 0;
            TipText = String.Format("Point @ {0}-{1}", x1, y1);
            Initialize();
        }

        public DrawPoint(int x1, int y1, DrawingPens.PenType p)
        {
            Point.X = x1;
            Point.Y = y1;
            DrawPen = DrawingPens.SetCurrentPen(p);
            PenType = p;
            ZOrder = 0;
            TipText = String.Format("Point @ {0}-{1}", x1, y1);
            Console.WriteLine(TipText);
            Initialize();
        }

        public DrawPoint(int x1, int y1, Color lineColor, int lineWidth)
        {
            Point.X = x1;
            Point.Y = y1;
            Color = lineColor;
            PenWidth = lineWidth;
            ZOrder = 0;
            TipText = String.Format("Point @ {0}-{1}", x1, y1);
            Console.WriteLine(TipText);

            Initialize();
        }


        public override void Draw(Graphics g, Rectangle boundary, Single xScale, Single yScale, Pen pencil)
        {
            Pen pen = (Pen)pencil.Clone();
            Brush b = new SolidBrush(pen.Color);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Point point = new Point();
            point.X = (Int32)((Point.X - 2) * xScale + boundary.X);
            point.Y = (Int32)((Point.Y - 2) * yScale + boundary.Y);

            g.FillRectangle(b, point.X, point.Y, 4 * xScale, 4 * yScale);
        }

        /// <summary>
        /// Clone this instance
        /// </summary>
        public override DrawObject Clone()
        {
            DrawPoint drawPoint = new DrawPoint();
            drawPoint.Point = Point;

            FillDrawObjectFields(drawPoint);
            return drawPoint;
        }

        public override int HandleCount {
            get { return 1; }
        }

        /// <summary>
        /// Get handle point by 1-based number
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public override Point GetHandle(int handleNumber)
        {
            return Point;
        }

        /// <summary>
        /// Get handle rectangle by 1-based number
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns>Rectangle structure to draw the handle</returns>
        public override Rectangle GetHandleRectangle(int handleNumber)
        {
            Point point = GetHandle(handleNumber);
            // Take into account width of pen
            return new Rectangle(point.X - (PenWidth + 3), point.Y - (PenWidth + 3), (PenWidth + 3) * 2 + 4, (PenWidth + 3) * 2 + 4);
        }

        /// <summary>
        /// Draw tracker for selected object
        /// </summary>
        /// <param name="g">Graphics to draw on</param>
        public override void DrawTracker(Graphics g, Rectangle boundary, Single xScale, Single yScale)
        {
            if (!Selected)
                return;

            SolidBrush brush = new SolidBrush(Color.Blue);

            for (int i = 1; i <= HandleCount; i++) {
                Rectangle rect = GetHandleRectangle(i);
                rect = new Rectangle(
                    (Int32)(rect.X * xScale + boundary.X - 1),
                    (Int32)(rect.Y * yScale + boundary.Y - 1),
                    (Int32)(rect.Width * xScale),
                    (Int32)(rect.Height * yScale));
                g.FillRectangle(brush, rect);
            }
            brush.Dispose();
        }

        /// <summary>
        /// Hit test.
        /// Return value: -1 - no hit
        ///                0 - hit anywhere
        ///                > 1 - handle number
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override int HitTest(Point point)
        {
            // OK, so the point is not on a selection handle, is it anywhere else on the line?
            if (PointInObject(point))
                return 0;

            return -1;
        }

        protected override bool PointInObject(Point point)
        {
            Rectangle rect = GetHandleRectangle(1);
            return rect.Contains(point);
        }

        public override bool IntersectsWith(Rectangle rectangle)
        {
            Rectangle rect = GetHandleRectangle(1);
            return rect.IntersectsWith(rectangle);
        }

        public override Cursor GetHandleCursor(int handleNumber)
        {
            switch (handleNumber) {
                case 1:
                    return Cursors.SizeAll;
                default:
                    return Cursors.Default;
            }
        }

        public override void MoveHandleTo(Point point, int handleNumber)
        {
            Point = point;
            Dirty = true;
        }

        public override void Move(int deltaX, int deltaY)
        {
            Point.X += deltaX;
            Point.Y += deltaY;
            Dirty = true;
        }

        public override void SaveToStream(SerializationInfo info, int orderNumber, int objectIndex)
        {
            info.AddValue(
                String.Format(CultureInfo.InvariantCulture,
                              "{0}{1}-{2}",
                              entryPoint, orderNumber, objectIndex),
                Point);

            base.SaveToStream(info, orderNumber, objectIndex);
        }

        public override void LoadFromStream(SerializationInfo info, int orderNumber, int objectIndex)
        {
            Point = (Point)info.GetValue(
                                    String.Format(CultureInfo.InvariantCulture,
                                                  "{0}{1}-{2}",
                                                  entryPoint, orderNumber, objectIndex),
                                    typeof(Point));

            base.LoadFromStream(info, orderNumber, objectIndex);
        }

        public override GraphicsData GetGraphicsData()
        {
            GraphicsData data = new GraphicsData();
            data.PointList = new List<Point>();
            data.PointList.Add(Point);

            return data;
        }

        public override Boolean IsInRectngle(Rectangle rect, Single xScale, Single yScale)
        {
            Point point = new Point();
            point.X = (Int32)(Point.X * xScale + rect.X);
            point.Y = (Int32)(Point.Y * yScale + rect.Y);

            return rect.Contains(point);
        }
    }
}