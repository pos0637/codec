using System;
using System.Drawing;
using System.Windows.Forms;

namespace DrawTools2
{
    /// <summary>
    /// Point tool
    /// </summary>
    internal class ToolPoint : ToolObject
    {
        public ToolPoint()
        {
            Cursor = new Cursor(GetType(), "Point.cur");
        }

        public override void OnMouseDown(DrawArea drawArea, MouseEventArgs e)
        {
            Point p = drawArea.BackTrackMouse(new Point(e.X, e.Y));
            if (drawArea.PenType ==
                DrawingPens.PenType.Generic)
                AddNewObject(drawArea, new DrawPoint(p.X, p.Y, drawArea.LineColor, drawArea.LineWidth));
            else
                AddNewObject(drawArea, new DrawPoint(p.X, p.Y, drawArea.PenType));
        }

        public override void OnMouseMove(DrawArea drawArea, MouseEventArgs e)
        {
            drawArea.Cursor = Cursor;

            if (e.Button ==
                MouseButtons.Left) {
                Point point = drawArea.BackTrackMouse(new Point(e.X, e.Y));
                int al = drawArea.TheLayers.ActiveLayerIndex;
                drawArea.TheLayers[al].Graphics[0].MoveHandleTo(point, 1);
                
                drawArea.Invalidate();
            }
        }
    }
}