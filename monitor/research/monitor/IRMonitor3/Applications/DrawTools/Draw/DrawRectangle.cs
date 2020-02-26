using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace DrawTools
{
    /// <summary>
    /// Rectangle graphic object
    /// </summary>
    [Serializable]
    public class DrawRectangle : DrawObject
    {
        private Rectangle rectangle;

        private const string entryRectangle = "Rect";

        /// <summary>
        ///  Graphic objects for hit test
        /// </summary>
        private GraphicsPath areaPath = null;
        private Region areaRegion = null;

        protected Rectangle Rectangle {
            get { return rectangle; }
            set { rectangle = value; }
        }

        /// <summary>
        /// Clone this instance
        /// </summary>
        public override DrawObject Clone()
        {
            DrawRectangle drawRectangle = new DrawRectangle();
            drawRectangle.rectangle = rectangle;

            FillDrawObjectFields(drawRectangle);
            return drawRectangle;
        }

        public DrawRectangle()
        {
            SetRectangle(0, 0, 1, 1);
        }

        public DrawRectangle(int x, int y, int width, int height)
        {
            Center = new System.Drawing.Point(x + (width / 2), y + (height / 2));
            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width = width;
            rectangle.Height = height;
            TipText = String.Format("Rectangle Center @ {0}, {1}", Center.X, Center.Y);
        }

        public DrawRectangle(int x, int y, int width, int height, Color lineColor, Color fillColor)
        {
            Center = new System.Drawing.Point(x + (width / 2), y + (height / 2));
            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width = width;
            rectangle.Height = height;
            Color = lineColor;
            FillColor = fillColor;
            PenWidth = -1;
            TipText = String.Format("Rectangle Center @ {0}, {1}", Center.X, Center.Y);
        }

        public DrawRectangle(int x, int y, int width, int height, Color lineColor, Color fillColor, bool filled)
        {
            Center = new System.Drawing.Point(x + (width / 2), y + (height / 2));
            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width = width;
            rectangle.Height = height;
            Color = lineColor;
            FillColor = fillColor;
            Filled = filled;
            PenWidth = -1;
            TipText = String.Format("Rectangle Center @ {0}, {1}", Center.X, Center.Y);
        }

        public DrawRectangle(int x, int y, int width, int height, DrawingPens.PenType pType, Color fillColor, bool filled)
        {
            Center = new System.Drawing.Point(x + (width / 2), y + (height / 2));
            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width = width;
            rectangle.Height = height;
            DrawPen = DrawingPens.SetCurrentPen(pType);
            PenType = pType;
            FillColor = fillColor;
            Filled = filled;
            TipText = String.Format("Rectangle Center @ {0}, {1}", Center.X, Center.Y);
        }

        public DrawRectangle(int x, int y, int width, int height, Color lineColor, Color fillColor, bool filled, int lineWidth)
        {
            Center = new System.Drawing.Point(x + (width / 2), y + (height / 2));
            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width = width;
            rectangle.Height = height;
            Color = lineColor;
            FillColor = fillColor;
            Filled = filled;
            PenWidth = lineWidth;
            TipText = String.Format("Rectangle Center @ {0}, {1}", Center.X, Center.Y);
        }

        /// <summary>
        /// Draw rectangle
        /// </summary>
        /// <param name="g"></param>
        public override void Draw(Graphics g, Rectangle boundary, Single xScale, Single yScale, Pen pencil)
        {
            Pen pen = (Pen)pencil.Clone();
            Brush b = new SolidBrush(FillColor);

            GraphicsPath gp = new GraphicsPath();

            Rectangle rect = new Rectangle(
                (Int32)((Rectangle.X * xScale + boundary.X)),
                (Int32)((Rectangle.Y * yScale + boundary.Y)),
                (Int32)(Rectangle.Width * xScale),
                (Int32)(Rectangle.Height * yScale));
            gp.AddRectangle(GetNormalizedRectangle(rect));
            // Rotate the path about it's center if necessary
            if (Rotation != 0) {
                RectangleF pathBounds = gp.GetBounds();
                Matrix m = new Matrix();
                m.RotateAt(Rotation, new PointF(pathBounds.Left + (pathBounds.Width / 2), pathBounds.Top + (pathBounds.Height / 2)), MatrixOrder.Append);
                gp.Transform(m);
            }

            g.DrawPath(pen, gp);
            if (Filled)
                g.FillPath(b, gp);

            gp.Dispose();
            pen.Dispose();
            b.Dispose();
        }

        protected void SetRectangle(int x, int y, int width, int height)
        {
            rectangle.X = x;
            rectangle.Y = y;
            rectangle.Width = width;
            rectangle.Height = height;
        }

        /// <summary>
        /// Get number of handles
        /// </summary>
        public override int HandleCount {
            get { return 8; }
        }
        /// <summary>
        /// Get number of connection points
        /// </summary>
        public override int ConnectionCount {
            get { return HandleCount; }
        }

        public override System.Drawing.Point GetConnection(int connectionNumber)
        {
            return GetHandle(connectionNumber);
        }

        /// <summary>
        /// Get handle point by 1-based number
        /// 画出八个点用于放大缩小
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public override System.Drawing.Point GetHandle(int handleNumber)
        {
            System.Drawing.Point[] points = null;
            if (Rotation != 0) {
                GraphicsPath gp = new GraphicsPath();
                gp.AddRectangle(rectangle);

                RectangleF pathBounds = gp.GetBounds();
                Matrix m = new Matrix();
                m.RotateAt(Rotation, new PointF(pathBounds.Left + (pathBounds.Width / 2), pathBounds.Top + (pathBounds.Height / 2)), MatrixOrder.Append);
                gp.Transform(m);

                points = new System.Drawing.Point[4];
                int i = 0;
                foreach (PointF point in gp.PathPoints) {
                    points[i++] = new System.Drawing.Point((Int32)point.X, (Int32)point.Y);
                }
                gp.Dispose();
            }
            else {
                points = new System.Drawing.Point[4];
                points[0] = new System.Drawing.Point(rectangle.Left, rectangle.Top);
                points[1] = new System.Drawing.Point(rectangle.Right, rectangle.Top);
                points[2] = new System.Drawing.Point(rectangle.Right, rectangle.Bottom);
                points[3] = new System.Drawing.Point(rectangle.Left, rectangle.Bottom);
            }

            int x = 0, y = 0;

            switch (handleNumber) {
                case 1:
                    x = points[0].X;
                    y = points[0].Y;
                    break;
                case 2:
                    x = (points[0].X + points[1].X) / 2;
                    y = (points[0].Y + points[1].Y) / 2;
                    break;
                case 3:
                    x = points[1].X;
                    y = points[1].Y;
                    break;
                case 4:
                    x = (points[1].X + points[2].X) / 2;
                    y = (points[1].Y + points[2].Y) / 2;
                    break;
                case 5:
                    x = points[2].X;
                    y = points[2].Y;
                    break;
                case 6:
                    x = (points[2].X + points[3].X) / 2;
                    y = (points[2].Y + points[3].Y) / 2;
                    break;
                case 7:
                    x = points[3].X;
                    y = points[3].Y;
                    break;
                case 8:
                    x = (points[0].X + points[3].X) / 2;
                    y = (points[0].Y + points[3].Y) / 2;
                    break;
            }
            return new System.Drawing.Point(x, y);
        }
        /// <summary>
        /// 画出用于放大缩小的矩形
        /// </summary>
        /// <param name="g"></param>
        /// <param name="boundary"></param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        public override void DrawTracker(Graphics g, Rectangle boundary, Single xScale, Single yScale)
        {
            if (!Selected)
                return;

            Brush b = new SolidBrush(Color.White);

            for (int i = 1; i <= HandleCount; i++) {
                GraphicsPath gp = new GraphicsPath();
                Rectangle rect = GetHandleRectangle(i);
                rect = new Rectangle(
                    (Int32)(rect.X * xScale + boundary.X - 1),
                    (Int32)(rect.Y * yScale + boundary.Y - 1),
                    (Int32)(rect.Width*1),
                    (Int32)(rect.Height*1));

                gp.AddRectangle(GetNormalizedRectangle(rect));
                g.FillPath(b, gp);
                gp.Dispose();
            }

            b.Dispose();
        }

        /// <summary>
        /// Hit test.
        /// Return value: -1 - no hit
        ///                0 - hit anywhere
        ///                > 1 - handle number
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override int HitTest(System.Drawing.Point point)
        {
            if (Selected) {
                for (int i = 1; i <= HandleCount; i++) {
                    if (GetHandleRectangle(i).Contains(point))
                        return i;
                }
            }

            if (PointInObject(point))
                return 0;
            return -1;
        }

        protected override bool PointInObject(System.Drawing.Point point)
        {
            if (Rotation != 0) {
                CreateObjects();
                return areaRegion.IsVisible(point);
            }
            else {
                return rectangle.Contains(point);
            }
        }

        /// <summary>
        /// Get cursor for the handle
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public override Cursor GetHandleCursor(int handleNumber)
        {
            switch (handleNumber) {
                case 1:
                    return Cursors.SizeNWSE;
                case 2:
                    return Cursors.SizeNS;
                case 3:
                    return Cursors.SizeNESW;
                case 4:
                    return Cursors.SizeWE;
                case 5:
                    return Cursors.SizeNWSE;
                case 6:
                    return Cursors.SizeNS;
                case 7:
                    return Cursors.SizeNESW;
                case 8:
                    return Cursors.SizeWE;
                default:
                    return Cursors.Default;
            }
        }

        /// <summary>
        /// Move handle to new point (resizing)
        /// </summary>
        /// <param name="point"></param>
        /// <param name="handleNumber"></param>
        public override void MoveHandleTo(System.Drawing.Point point, int handleNumber)
        {
            int left = Rectangle.Left;
            int top = Rectangle.Top;
            int right = Rectangle.Right;
            int bottom = Rectangle.Bottom;

            switch (handleNumber) {
                case 1:
                    left = point.X;
                    top = point.Y;
                    break;
                case 2:
                    top = point.Y;
                    break;
                case 3:
                    right = point.X;
                    top = point.Y;
                    break;
                case 4:
                    right = point.X;
                    break;
                case 5:
                    right = point.X;
                    bottom = point.Y;
                    break;
                case 6:
                    bottom = point.Y;
                    break;
                case 7:
                    left = point.X;
                    bottom = point.Y;
                    break;
                case 8:
                    left = point.X;
                    break;
            }
            Dirty = true;
            SetRectangle(left, top, right - left, bottom - top);
            Invalidate();
        }

        public override bool IntersectsWith(Rectangle rectangle)
        {
            if (Rotation != 0) {
                CreateObjects();
                return areaRegion.IsVisible(rectangle);
            }
            else {
                return Rectangle.IntersectsWith(rectangle);
            }
        }

        /// <summary>
        /// Move object
        /// </summary>
        /// <param name="deltaX"></param>
        /// <param name="deltaY"></param>
        public override void Move(int deltaX, int deltaY)
        {
            rectangle.X += deltaX;
            rectangle.Y += deltaY;
            Dirty = true;
            Invalidate();
        }

        public override void Dump()
        {
            base.Dump();

            Trace.WriteLine("rectangle.X = " + rectangle.X.ToString(CultureInfo.InvariantCulture));
            Trace.WriteLine("rectangle.Y = " + rectangle.Y.ToString(CultureInfo.InvariantCulture));
            Trace.WriteLine("rectangle.Width = " + rectangle.Width.ToString(CultureInfo.InvariantCulture));
            Trace.WriteLine("rectangle.Height = " + rectangle.Height.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Normalize rectangle
        /// </summary>
        public override void Normalize()
        {
            rectangle = GetNormalizedRectangle(rectangle);
        }

        /// <summary>
        /// Save objevt to serialization stream
        /// </summary>
        /// <param name="info">Contains all data being written to disk</param>
        /// <param name="orderNumber">Index of the Layer being saved</param>
        /// <param name="objectIndex">Index of the drawing object in the Layer</param>
        public override void SaveToStream(SerializationInfo info, int orderNumber, int objectIndex)
        {
            info.AddValue(
                String.Format(CultureInfo.InvariantCulture,
                              "{0}{1}-{2}",
                              entryRectangle, orderNumber, objectIndex),
                rectangle);

            base.SaveToStream(info, orderNumber, objectIndex);
        }

        /// <summary>
        /// LOad object from serialization stream
        /// </summary>
        /// <param name="info"></param>
        /// <param name="orderNumber"></param>
        /// <param name="objectIndex"></param>
        public override void LoadFromStream(SerializationInfo info, int orderNumber, int objectIndex)
        {
            rectangle = (Rectangle)info.GetValue(
                                    String.Format(CultureInfo.InvariantCulture,
                                                  "{0}{1}-{2}",
                                                  entryRectangle, orderNumber, objectIndex),
                                    typeof(Rectangle));

            base.LoadFromStream(info, orderNumber, objectIndex);
        }

        public override GraphicsData GetGraphicsData()
        {
            GraphicsData data = new GraphicsData();
            data.PointList = new List<System.Drawing.Point>();
            if (Rotation != 0) {
                GraphicsPath gp = new GraphicsPath();
                Matrix m = new Matrix();
                gp.AddRectangle(rectangle);
                RectangleF pathBounds = gp.GetBounds();
                m.RotateAt(Rotation, new PointF(pathBounds.Left + (pathBounds.Width / 2), pathBounds.Top + (pathBounds.Height / 2)), MatrixOrder.Append);
                gp.Transform(m);

                data.PointList.Add(System.Drawing.Point.Truncate(gp.PathPoints[0]));
                data.PointList.Add(System.Drawing.Point.Truncate(gp.PathPoints[1]));
                data.PointList.Add(System.Drawing.Point.Truncate(gp.PathPoints[2]));
                data.PointList.Add(System.Drawing.Point.Truncate(gp.PathPoints[3]));
                gp.Dispose();
            }
            else {
                data.PointList.Add(new System.Drawing.Point(rectangle.Left, rectangle.Top));
                data.PointList.Add(new System.Drawing.Point(rectangle.Right, rectangle.Top));
                data.PointList.Add(new System.Drawing.Point(rectangle.Right, rectangle.Bottom));
                data.PointList.Add(new System.Drawing.Point(rectangle.Left, rectangle.Bottom));
            }

            return data;
        }

        public override Boolean IsInRectngle(Rectangle rect, Single xScale, Single yScale)
        {
            Rectangle temp = new Rectangle(
                (Int32)((Rectangle.X * xScale + rect.X)),
                (Int32)((Rectangle.Y * yScale + rect.Y)),
                (Int32)(Rectangle.Width * xScale),
                (Int32)(Rectangle.Height * yScale));

            if (Rotation != 0) {
                GraphicsPath gp = new GraphicsPath();
                Matrix m = new Matrix();
                gp.AddRectangle(temp);
                RectangleF pathBounds = gp.GetBounds();
                m.RotateAt(Rotation, new PointF(pathBounds.Left + (pathBounds.Width / 2), pathBounds.Top + (pathBounds.Height / 2)), MatrixOrder.Append);
                gp.Transform(m);

                Boolean ret = true;
                for (Int32 i = 0; i < 4; i++) {
                    System.Drawing.Point p = System.Drawing.Point.Truncate(gp.PathPoints[i]);
                    if (!rect.Contains(p)) {
                        ret = false;
                        break;
                    }
                }

                gp.Dispose();
                return ret;
            }
            else {
                return rect.Contains(temp);
            }
        }

        /// <summary>
        /// Create graphic objects used for hit test.
        /// </summary>
        private void CreateObjects()
        {
            if (areaPath != null)
                return;

            // Create path which contains wide line
            // for easy mouse selection
            areaPath = new GraphicsPath();
            areaPath.AddRectangle(rectangle);

            // Rotate the path about it's center if necessary
            RectangleF pathBounds = areaPath.GetBounds();
            Matrix m = new Matrix();
            m.RotateAt(Rotation, new PointF(pathBounds.Left + (pathBounds.Width / 2), pathBounds.Top + (pathBounds.Height / 2)), MatrixOrder.Append);
            areaPath.Transform(m);
            m.Dispose();

            // Create region from the path
            areaRegion = new Region(areaPath);
        }

        /// <summary>
        /// Invalidate object.
        /// When object is invalidated, path used for hit test
        /// is released and should be created again.
        /// </summary>
        private void Invalidate()
        {
            if (areaPath != null) {
                areaPath.Dispose();
                areaPath = null;
            }

            if (areaRegion != null) {
                areaRegion.Dispose();
                areaRegion = null;
            }
        }

        #region Helper Functions
        public static Rectangle GetNormalizedRectangle(int x1, int y1, int x2, int y2)
        {
            if (x2 < x1) {
                int tmp = x2;
                x2 = x1;
                x1 = tmp;
            }

            if (y2 < y1) {
                int tmp = y2;
                y2 = y1;
                y1 = tmp;
            }
            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        public static Rectangle GetNormalizedRectangle(System.Drawing.Point p1, System.Drawing.Point p2)
        {
            return GetNormalizedRectangle(p1.X, p1.Y, p2.X, p2.Y);
        }

        public static Rectangle GetNormalizedRectangle(Rectangle r)
        {
            return GetNormalizedRectangle(r.X, r.Y, r.X + r.Width, r.Y + r.Height);
        }
        #endregion Helper Functions
    }
}