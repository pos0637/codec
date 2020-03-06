using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DrawTools
{
    /// <summary>
    /// Ellipse graphic object
    /// </summary>
    [Serializable]
    public class DrawEllipse : DrawRectangle
    {
        public DrawEllipse()
        {
            SetRectangle(0, 0, 1, 1);
            Initialize();
        }

        /// <summary>
        /// Clone this instance
        /// </summary>
        public override DrawObject Clone()
        {
            DrawEllipse drawEllipse = new DrawEllipse();
            drawEllipse.Rectangle = Rectangle;

            FillDrawObjectFields(drawEllipse);
            return drawEllipse;
        }

        public DrawEllipse(int x, int y, int width, int height)
        {
            Rectangle = new Rectangle(x, y, width, height);
            Center = new Point(x + (width / 2), y + (height / 2));
            TipText = String.Format("Ellipse Center @ {0}, {1}", Center.X, Center.Y);
            Initialize();
        }

        public DrawEllipse(int x, int y, int width, int height, Color lineColor, Color fillColor, bool filled)
        {
            Rectangle = new Rectangle(x, y, width, height);
            Center = new Point(x + (width / 2), y + (height / 2));
            TipText = String.Format("Ellipse Center @ {0}, {1}", Center.X, Center.Y);
            Color = lineColor;
            FillColor = fillColor;
            Filled = filled;
            Initialize();
        }

        public DrawEllipse(int x, int y, int width, int height, DrawingPens.PenType pType, Color fillColor, bool filled)
        {
            Rectangle = new Rectangle(x, y, width, height);
            Center = new Point(x + (width / 2), y + (height / 2));
            TipText = String.Format("Ellipse Center @ {0}, {1}", Center.X, Center.Y);
            DrawPen = DrawingPens.SetCurrentPen(pType);
            PenType = pType;
            FillColor = fillColor;
            Filled = filled;
            Initialize();
        }

        public DrawEllipse(int x, int y, int width, int height, Color lineColor, Color fillColor, bool filled, int lineWidth)
        {
            Rectangle = new Rectangle(x, y, width, height);
            Center = new Point(x + (width / 2), y + (height / 2));
            TipText = String.Format("Ellipse Center @ {0}, {1}", Center.X, Center.Y);
            Color = lineColor;
            FillColor = fillColor;
            Filled = filled;
            PenWidth = lineWidth;
            Initialize();
        }

        public override void Draw(Graphics g, Rectangle boundary, Single xScale, Single yScale, Pen pen)
        {
            Brush b = new SolidBrush(FillColor);
            GraphicsPath gp = new GraphicsPath();

            Rectangle rect = new Rectangle(
                (Int32)((Rectangle.X * xScale + boundary.X)),
                (Int32)((Rectangle.Y * yScale + boundary.Y)),
                (Int32)(Rectangle.Width * xScale),
                (Int32)(Rectangle.Height * yScale));

            gp.AddEllipse(GetNormalizedRectangle(rect));
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

        public override GraphicsData GetGraphicsData()
        {
            GraphicsData data = new GraphicsData();
            data.IsEllipse = true;
            data.Axes = new Size(Rectangle.Width / 2, Rectangle.Height / 2);
            data.Angle = Rotation;
            data.Center = new Point(Rectangle.X + Rectangle.Width / 2, Rectangle.Y + Rectangle.Height / 2);

            return data;
        }
    }
}