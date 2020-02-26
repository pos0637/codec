using System;
using System.Windows.Forms;

namespace DrawTools
{
    /// <summary>
    /// Base class for all tools which create new graphic object
    /// </summary>
    internal abstract class ToolObject : Tool
    {
        /// <summary>
        /// Tool cursor.
        /// </summary>
        private Cursor cursor;

        /// <summary>
        /// graphic object
        /// </summary>
        private DrawObject mDrawObject;

        /// <summary>
        /// Tool cursor.
        /// </summary>
        protected Cursor Cursor
        {
            get { return cursor; }
            set { cursor = value; }
        }

        /// <summary>
        /// Left mouse is released.
        /// New object is created and resized.
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="e"></param>
        public override void OnMouseUp(DrawArea drawArea, MouseEventArgs e)
        {
            int al = drawArea.TheLayers.ActiveLayerIndex;
            if (drawArea.TheLayers[al].Graphics.Count > 0)
                drawArea.TheLayers[al].Graphics[0].Normalize();

            if ((drawArea.ActiveTool != DrawArea.DrawToolType.Polygon)
                || ( (drawArea.ActiveTool == DrawArea.DrawToolType.Polygon) && (e.Button == MouseButtons.Right))) {
                drawArea.ActiveTool = DrawArea.DrawToolType.Pointer;
                drawArea.AddDrawObject?.BeginInvoke(mDrawObject, null, null);
                drawArea.AddCommandToHistory(new CommandAdd(mDrawObject));
                mDrawObject = null;
            }

            drawArea.Capture = false;
            drawArea.Invalidate();
        }

        /// <summary>
        /// Add new object to draw area.
        /// Function is called when user left-clicks draw area,
        /// and one of ToolObject-derived tools is active.
        /// </summary>
        /// <param name="drawArea"></param>
        /// <param name="o"></param>
        protected void AddNewObject(DrawArea drawArea, DrawObject o)
        {
            int al = drawArea.TheLayers.ActiveLayerIndex;
            drawArea.TheLayers[al].Graphics.UnselectAll();

            o.Selected = true;
            o.Dirty = true;
            // Set the object id now
            o.ID = DrawObject.sCurrentDrawObjectId++;
            drawArea.TheLayers[al].Graphics.Add(o);

            drawArea.Capture = true;
            mDrawObject = o;
            drawArea.Invalidate();
        }
    }
}