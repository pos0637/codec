using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace DrawTools2
{
    /// <summary>
    /// DrawPolygon graphic object - a PolyLine is a series of connected lines
    /// </summary>
    //[Serializable]
    public class DrawPolygon : DrawLine
    {
        // Last Segment start and end points
        private Point startPoint;
        private Point endPoint;

        private ArrayList pointArray; // list of points
        private Cursor handleCursor;

        private const string entryLength = "Length";
        private const string entryPoint = "Point";

        private bool _drawedInProcess = false;

        public Point StartPoint {
            get { return startPoint; }
            set { startPoint = value; }
        }

        public Point EndPoint {
            get { return endPoint; }
            set { endPoint = value; }
        }

        /// <summary>
        /// Clone this instance
        /// </summary>
        public override DrawObject Clone()
        {
            DrawPolygon drawPolyLine = new DrawPolygon();

            drawPolyLine.startPoint = startPoint;
            drawPolyLine.endPoint = endPoint;
            drawPolyLine.pointArray = pointArray;
            drawPolyLine._drawedInProcess = _drawedInProcess;

            FillDrawObjectFields(drawPolyLine);
            return drawPolyLine;
        }

        public DrawPolygon()
        {
            pointArray = new ArrayList();

            LoadCursor();
            Initialize();
        }

        public DrawPolygon(int x1, int y1, int x2, int y2)
        {
            pointArray = new ArrayList();
            pointArray.Add(new Point(x1, y1));
            pointArray.Add(new Point(x2, y2));

            LoadCursor();
            Initialize();
        }

        public DrawPolygon(int x1, int y1, int x2, int y2, DrawingPens.PenType p)
        {
            pointArray = new ArrayList();
            pointArray.Add(new Point(x1, y1));
            pointArray.Add(new Point(x2, y2));
            DrawPen = DrawingPens.SetCurrentPen(p);
            PenType = p;

            LoadCursor();
            Initialize();
        }

        public DrawPolygon(int x1, int y1, int x2, int y2, Color lineColor, int lineWidth)
        {
            pointArray = new ArrayList();
            pointArray.Add(new Point(x1, y1));
            pointArray.Add(new Point(x2, y2));
            Color = lineColor;
            PenWidth = lineWidth;

            LoadCursor();
            Initialize();
        }

        public override void Draw(Graphics g, Rectangle boundary, Single xScale, Single yScale)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Pen pen;

            if (DrawPen == null)
                pen = new Pen(Color, PenWidth);
            else
                pen = DrawPen.Clone() as Pen;

            Point[] pts = new Point[pointArray.Count];
            for (int i = 0; i < pointArray.Count; i++) {
                Point px = (Point)pointArray[i];
                px.X = (Int32)(px.X * xScale + boundary.X);
                px.Y = (Int32)(px.Y * yScale + boundary.Y);
                pts[i] = px;
            }
            byte[] types = new byte[pointArray.Count];
            for (int i = 0; i < pointArray.Count; i++)
                types[i] = (byte)PathPointType.Line;

            if (_drawedInProcess)
                types[pointArray.Count - 1] = 129;

            //types[pointArray.Count - 1] = 129;
            GraphicsPath gp = new GraphicsPath(pts, types);
            // Rotate the path about it's center if necessary
            if (Rotation != 0) {
                RectangleF pathBounds = gp.GetBounds();
                Matrix m = new Matrix();
                m.RotateAt(Rotation, new PointF(pathBounds.Left + (pathBounds.Width / 2), pathBounds.Top + (pathBounds.Height / 2)), MatrixOrder.Append);
                gp.Transform(m);
            }
            g.DrawPath(pen, gp);
            //g.DrawCurve(pen, pts);
            gp.Dispose();
            if (pen != null)
                pen.Dispose();
        }

        public void AddPoint(Point point)
        {
            pointArray.Add(point);
        }

        public override int HandleCount {
            get { return pointArray.Count; }
        }

        /// <summary>
        /// Get handle point by 1-based number
        /// </summary>
        /// <param name="handleNumber"></param>
        /// <returns></returns>
        public override Point GetHandle(int handleNumber)
        {
            if (handleNumber < 1)
                handleNumber = 1;
            if (handleNumber > pointArray.Count)
                handleNumber = pointArray.Count;

            if (Rotation != 0) {
                Point[] pts = new Point[pointArray.Count];
                for (int i = 0; i < pointArray.Count; i++) {
                    pts[i] = (Point)pointArray[i];
                }

                byte[] types = new byte[pointArray.Count];
                for (int i = 0; i < pointArray.Count; i++)
                    types[i] = (byte)PathPointType.Line;

                GraphicsPath gp = new GraphicsPath(pts, types);
                RectangleF pathBounds = gp.GetBounds();
                Matrix m = new Matrix();
                m.RotateAt(Rotation, new PointF(pathBounds.Left + (pathBounds.Width / 2), pathBounds.Top + (pathBounds.Height / 2)), MatrixOrder.Append);
                gp.Transform(m);

                Point p = new Point();
                p.X = (Int32)gp.PathPoints[handleNumber - 1].X;
                p.Y = (Int32)gp.PathPoints[handleNumber - 1].Y;

                gp.Dispose();
                return p;
            }
            else {
                return ((Point)pointArray[handleNumber - 1]);
            }
        }

        public override Cursor GetHandleCursor(int handleNumber)
        {
            return handleCursor;
        }

        public override void MoveHandleTo(Point point, int handleNumber)
        {
            if (handleNumber < 1)
                handleNumber = 1;

            if (handleNumber > pointArray.Count)
                handleNumber = pointArray.Count;
            pointArray[handleNumber - 1] = point;
            Dirty = true;
            Invalidate();
        }

        public override void Move(int deltaX, int deltaY)
        {
            int n = pointArray.Count;

            for (int i = 0; i < n; i++) {
                Point point;
                point = new Point(((Point)pointArray[i]).X + deltaX, ((Point)pointArray[i]).Y + deltaY);
                pointArray[i] = point;
            }
            Dirty = true;
            Invalidate();
        }

        public override void SaveToStream(SerializationInfo info, int orderNumber, int objectIndex)
        {
            info.AddValue(
                String.Format(CultureInfo.InvariantCulture,
                              "{0}{1}-{2}",
                              entryLength, orderNumber, objectIndex),
                pointArray.Count);

            int i = 0;
            foreach (Point p in pointArray) {
                info.AddValue(
                    String.Format(CultureInfo.InvariantCulture,
                                  "{0}{1}-{2}-{3}",
                                  new object[] { entryPoint, orderNumber, objectIndex, i++ }),
                    p);
            }
            base.SaveToStream(info, orderNumber, objectIndex);
        }

        public override void LoadFromStream(SerializationInfo info, int orderNumber, int objectIndex)
        {
            _drawedInProcess = true;
            int n = info.GetInt32(
                String.Format(CultureInfo.InvariantCulture,
                              "{0}{1}-{2}",
                              entryLength, orderNumber, objectIndex));

            for (int i = 0; i < n; i++) {
                Point point;
                point = (Point)info.GetValue(
                                   String.Format(CultureInfo.InvariantCulture,
                                                 "{0}{1}-{2}-{3}",
                                                 new object[] { entryPoint, orderNumber, objectIndex, i }),
                                   typeof(Point));
                pointArray.Add(point);
            }
            base.LoadFromStream(info, orderNumber, objectIndex);
        }

        public override GraphicsData GetGraphicsData()
        {
            GraphicsData data = new GraphicsData();
            data.PointList = new List<Point>();
            if (Rotation != 0) {
                Point[] pts = new Point[pointArray.Count];
                for (int i = 0; i < pointArray.Count; i++) {
                    pts[i] = (Point)pointArray[i];
                }

                byte[] types = new byte[pointArray.Count];
                for (int i = 0; i < pointArray.Count; i++)
                    types[i] = (byte)PathPointType.Line;

                GraphicsPath gp = new GraphicsPath(pts, types);
                RectangleF pathBounds = gp.GetBounds();
                Matrix m = new Matrix();
                m.RotateAt(Rotation, new PointF(pathBounds.Left + (pathBounds.Width / 2), pathBounds.Top + (pathBounds.Height / 2)), MatrixOrder.Append);
                gp.Transform(m);

                foreach (PointF point in gp.PathPoints) {
                    data.PointList.Add(Point.Truncate(point));
                }

                gp.Dispose();
            }
            else {
                for (int i = 0; i < pointArray.Count; i++) {
                    Point point = (Point)pointArray[i];
                    data.PointList.Add(point);
                }
            }

            return data;
        }

        public override Boolean IsInRectngle(Rectangle rect, Single xScale, Single yScale)
        {
            Boolean ret = true;
            Point[] pts = new Point[pointArray.Count];
            for (int i = 0; i < pointArray.Count; i++) {
                Point px = (Point)pointArray[i];
                px.X = (Int32)(px.X * xScale + rect.X);
                px.Y = (Int32)(px.Y * yScale + rect.Y);
                pts[i] = px;
            }

            if (Rotation != 0) {
                byte[] types = new byte[pointArray.Count];
                for (int i = 0; i < pointArray.Count; i++)
                    types[i] = (byte)PathPointType.Line;

                GraphicsPath gp = new GraphicsPath(pts, types);
                RectangleF pathBounds = gp.GetBounds();
                Matrix m = new Matrix();
                m.RotateAt(Rotation, new PointF(pathBounds.Left + (pathBounds.Width / 2), pathBounds.Top + (pathBounds.Height / 2)), MatrixOrder.Append);
                gp.Transform(m);

                foreach (PointF point in gp.PathPoints) {
                    if (!rect.Contains(Point.Truncate(point))) {
                        ret = false;
                        break;
                    }
                }

                gp.Dispose();
            }
            else {
                for (int i = 0; i < pts.Length; i++) {
                    Point point = (Point)pts[i];
                    if (!rect.Contains(point)) {
                        ret = false;
                        break;
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Create graphic object used for hit test
        /// </summary>
        protected override void CreateObjects()
        {
            if (AreaPath != null)
                return;

            // Create closed path which contains all polygon vertexes
            AreaPath = new GraphicsPath();

            int x1 = 0, y1 = 0; // previous point

            IEnumerator enumerator = pointArray.GetEnumerator();

            if (enumerator.MoveNext()) {
                x1 = ((Point)enumerator.Current).X;
                y1 = ((Point)enumerator.Current).Y;
            }

            while (enumerator.MoveNext()) {
                int x2, y2; // current point
                x2 = ((Point)enumerator.Current).X;
                y2 = ((Point)enumerator.Current).Y;

                AreaPath.AddLine(x1, y1, x2, y2);

                x1 = x2;
                y1 = y2;
            }

            AreaPath.CloseFigure();

            // Create region from the path
            AreaRegion = new Region(AreaPath);
        }

        public void DrawFinish()
        {
            _drawedInProcess = true;
        }

        private void LoadCursor()
        {
            handleCursor = new Cursor(GetType(), "PolyHandle.cur");
        }
    }
}