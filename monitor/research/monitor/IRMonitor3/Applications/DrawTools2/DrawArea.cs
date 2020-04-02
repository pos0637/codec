using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace DrawTools2
{
    /// <summary>
    /// Working area.
    /// Handles mouse input and draws graphics objects.
    /// </summary>
    public partial class DrawArea : UserControl
    {
        public delegate void DrawAreaAction();
        public delegate void AreaMouseDown(Int32 x, Int32 y);
        public delegate void AddDrawObjectAction(DrawObject drawObject);
        public delegate void UpdateDrawObjectAction(List<DrawObject> drawObjectList);
        public delegate void DeleteDrawObjectAction(List<DrawObject> drawObjectList);

        public DrawAreaAction DrawAreaMouseDown;
        public AreaMouseDown MouseDownArea;

        public AddDrawObjectAction AddDrawObject;
        public UpdateDrawObjectAction UpdateDrawObject;
        public DeleteDrawObjectAction DeleteDrawObject;

        #region Constructor, Dispose
        public DrawArea()
        {
            // create list of Layers, with one default active visible layer
            _layers = new Layers();
            _layers.CreateNewLayer("Default");
            _panning = false;
            _panX = 0;
            _panY = 0;

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            // set default tool
            activeTool = DrawToolType.Pointer;

            // Create undo manager
            undoManager = new UndoManager(_layers);

            LineColor = Color.Black;
            FillColor = Color.White;
            LineWidth = -1;

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

        }
        #endregion Constructor, Dispose

        #region Enumerations
        public enum DrawToolType
        {
            Pointer,
            Point,
            Rectangle,
            Ellipse,
            Line,
            Polygon,
            NumberOfDrawTools
        };
        #endregion Enumerations

        #region Members
        private float _zoom = 1.0f;
        private int _panX = 0;
        private int _panY;
        private int _originalPanY;
        private bool _panning = false;
        private Point lastPoint;
        private Color _lineColor = Color.Black;
        private Color _fillColor = Color.White;
        private bool _drawFilled = false;
        private int _lineWidth = -1;
        private Pen _currentPen;
        private DrawingPens.PenType _penType;

        // Define the Layers collection
        private Layers _layers;

        private DrawToolType activeTool; // active drawing tool
        private Tool[] tools; // array of tools

        // group selection rectangle
        private Rectangle netRectangle;
        private bool drawNetRectangle = false;

        private UndoManager undoManager;

        private Bitmap mBitmap;

        private Size mResolution = new Size();
        private Rectangle mBoundary = new Rectangle();
        private Rectangle mOrignBoundary = new Rectangle();
        private Point mLeftTopPoint = new Point();
        private Size mSize = new Size();
        private Byte[] mBuffer;

        private float mScaleX;
        private float mScaleY;
        private bool _controlKey = false;
        private DrawToolbar mDrawToolbar;
        #endregion Members

        #region Properties
        /// <summary>
        /// Allow tools and objects to see the type of pen set
        /// </summary>
        public DrawingPens.PenType PenType {
            get { return _penType; }
            set { _penType = value; }
        }

        /// <summary>
        /// Current Drawing Pen
        /// </summary>
        public Pen CurrentPen {
            get { return _currentPen; }
            set { _currentPen = value; }
        }

        /// <summary>
        /// Current Line Width
        /// </summary>
        public int LineWidth {
            get { return _lineWidth; }
            set { _lineWidth = value; }
        }

        /// <summary>
        /// Flag determines if objects will be drawn filled or not
        /// </summary>
        public bool DrawFilled {
            get { return _drawFilled; }
            set { _drawFilled = value; }
        }

        /// <summary>
        /// Color to draw filled objects with
        /// </summary>
        public Color FillColor {
            get { return _fillColor; }
            set { _fillColor = value; }
        }

        /// <summary>
        /// Color for drawing lines
        /// </summary>
        public Color LineColor {
            get { return _lineColor; }
            set { _lineColor = value; }
        }

        /// <summary>
        /// Original Y position - used when panning
        /// </summary>
        public int OriginalPanY {
            get { return _originalPanY; }
            set { _originalPanY = value; }
        }

        /// <summary>
        /// Flag is true if panning active
        /// </summary>
        public bool Panning {
            get { return _panning; }
            set { _panning = value; }
        }

        /// <summary>
        /// Current pan offset along X-axis
        /// </summary>
        public int PanX {
            get { return _panX; }
            set {
                Rectangle rect = new Rectangle(mLeftTopPoint.X, mLeftTopPoint.Y, mSize.Width, mSize.Height);
                Rectangle boundary = Rectangle.Inflate(mOrignBoundary, (Int32)(mOrignBoundary.Width * (_zoom - 1) / 2), (Int32)(mOrignBoundary.Height * (_zoom - 1) / 2)); ;

                Int32 deltaX = 0;
                if (boundary.Width > rect.Width) {
                    boundary.X += value;
                    if (boundary.Left > rect.Left) {
                        deltaX = boundary.Left - rect.Left;
                        boundary.X -= deltaX;
                    }
                    else if (boundary.Right < rect.Right) {
                        deltaX = boundary.Right - rect.Right;
                        boundary.X -= deltaX;
                    }

                    _panX = (value - deltaX);
                }

                boundary.Y += _panY;
                mBoundary = boundary;
            }
        }

        /// <summary>
        /// Current pan offset along Y-axis
        /// </summary>
        public int PanY {
            get { return _panY; }
            set {
                Rectangle rect = new Rectangle(mLeftTopPoint.X, mLeftTopPoint.Y, mSize.Width, mSize.Height);
                Rectangle boundary = Rectangle.Inflate(mOrignBoundary, (Int32)(mOrignBoundary.Width * (_zoom - 1) / 2), (Int32)(mOrignBoundary.Height * (_zoom - 1) / 2));

                Int32 deltaY = 0;
                if (boundary.Height > rect.Height) {
                    boundary.Y += value;
                    if (boundary.Top > rect.Top) {
                        deltaY = boundary.Top - rect.Top;
                        boundary.Y -= deltaY;
                    }
                    else if (boundary.Bottom < rect.Bottom) {
                        deltaY = boundary.Bottom - rect.Bottom;
                        boundary.Y -= deltaY;
                    }

                    _panY = (value - deltaY);
                }

                boundary.X += _panX;
                mBoundary = boundary;
            }
        }

        /// <summary>
        /// Current Zoom factor
        /// </summary>
        public float Zoom {
            get { return _zoom; }
            set {
                _zoom = value;

                Rectangle boundary = mOrignBoundary;

                if (_zoom < 1) {
                    _panX = 0;
                    _panY = 0;
                }
                else {
                    boundary.X += _panX;
                    boundary.Y += _panY;
                }

                boundary = Rectangle.Inflate(boundary, (Int32)(mOrignBoundary.Width * (_zoom - 1) / 2), (Int32)(mOrignBoundary.Height * (_zoom - 1) / 2));
                Rectangle rect = new Rectangle(mLeftTopPoint.X, mLeftTopPoint.Y, mSize.Width, mSize.Height);
                if (boundary.IntersectsWith(rect) && !boundary.Contains(rect)) {
                    boundary.X -= _panX;
                    boundary.Y -= _panY;

                    _panX = 0;
                    _panY = 0;
                }

                mBoundary = boundary;
                mScaleX = (float)mBoundary.Width / mResolution.Width;
                mScaleY = (float)mBoundary.Height / mResolution.Height;
            }
        }

        /// <summary>
        /// Group selection rectangle. Used for drawing.
        /// </summary>
        public Rectangle NetRectangle {
            get { return netRectangle; }
            set { netRectangle = value; }
        }

        /// <summary>
        /// Flag is set to true if group selection rectangle should be drawn.
        /// </summary>
        public bool DrawNetRectangle {
            get { return drawNetRectangle; }
            set { drawNetRectangle = value; }
        }

        /// <summary>
        /// Active drawing tool.
        /// </summary>
        public DrawToolType ActiveTool {
            get { return activeTool; }
            set { activeTool = value; }
        }

        /// <summary>
        /// List of Layers in the drawing
        /// </summary>
        public Layers TheLayers {
            get { return _layers; }
            set { _layers = value; }
        }

        /// <summary>
        /// Return True if Undo operation is possible
        /// </summary>
        public bool CanUndo {
            get {
                if (undoManager != null) {
                    return undoManager.CanUndo;
                }

                return false;
            }
        }

        /// <summary>
        /// Return True if Redo operation is possible
        /// </summary>
        public bool CanRedo {
            get {
                if (undoManager != null) {
                    return undoManager.CanRedo;
                }

                return false;
            }
        }

        /// <summary>
        /// X轴放大缩小
        /// </summary>
        public float ScaleX {
            get { return mScaleX; }
        }

        /// <summary>
        /// Y轴放大缩小
        /// </summary>
        public float ScaleY {
            get { return mScaleY; }
        }

        /// <summary>
        /// 绘制边界
        /// </summary>
        public Rectangle Boundary {
            get { return mBoundary; }
        }

        /// <summary>
        /// 是否启用背景渲染
        /// </summary>
        public Boolean BackgroundRenderEnabled { get; set; }

        /// <summary>
        /// 是否能使用/操作
        /// </summary>
        public Boolean CanControl { get; set; }

        /// <summary>
        /// 是否按比例绘制
        /// </summary>
        public Boolean IsZoomDrawed = true;

        #endregion

        #region public functions
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="formOwner">Reference to the owner form</param>
        /// <param name="drawToolbar">Reference to drawToolbar</param>
        /// <param name="resolution">image resolution</param>
        public void Initialize( DrawToolbar drawToolbar, Size resolution)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
     ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            Invalidate();

            if (mBitmap != null)
                return;

            // create array of drawing tools
            tools = new Tool[(int)DrawToolType.NumberOfDrawTools];
            tools[(int)DrawToolType.Pointer] = new ToolPointer();
            tools[(int)DrawToolType.Point] = new ToolPoint();
            tools[(int)DrawToolType.Rectangle] = new ToolRectangle();
            tools[(int)DrawToolType.Ellipse] = new ToolEllipse();
            tools[(int)DrawToolType.Line] = new ToolLine();
            tools[(int)DrawToolType.Polygon] = new ToolPolygon();

            mResolution = resolution;
            mDrawToolbar = drawToolbar;

            CalcBoundary();
            mBitmap = new Bitmap(mResolution.Width, mResolution.Height, PixelFormat.Format32bppArgb);
            mBuffer = new Byte[mResolution.Width * mResolution.Height * 4];
            //formOwner.Shown += DrawArea_Shown;
           // formOwner.Move += DrawArea_Move;
           // formOwner.ResizeEnd += DrawArea_Resize;
        }

        /// <summary>
        /// 更新图像
        /// </summary>
        /// <param name="buffer"></param>
        public void UpdateViewData(Byte[] buffer)
        {
            if ((buffer == null) || (mBuffer == null) || (buffer.Length != mBuffer.Length))
                return;

            Array.Copy(buffer, mBuffer, buffer.Length);
        }

        /// <summary>
        /// 获取绘制图形的数据列表
        /// </summary>
        /// <returns></returns>
        public List<GraphicsData> GetGraphicsDataList()
        {
            List<GraphicsData> dataList = new List<GraphicsData>();
            if (_layers != null) {
                int lc = _layers.Count;
                for (int i = 0; i < lc; i++) {
                    if (_layers[i].IsVisible) {
                        if (_layers[i].Graphics != null) {
                            List<GraphicsData> list = _layers[i].Graphics.GetGraphicsDataList();
                            dataList.AddRange(list);
                        }
                    }
                }
            }

            return dataList;
        }

        /// <summary>
        /// 获取绘制图形列表
        /// </summary>
        /// <returns></returns>
        public List<DrawObject> GetDrawObjectList()
        {
            List<DrawObject> drawObjectList = new List<DrawObject>();
            if (_layers != null) {
                int lc = _layers.Count;
                for (int i = 0; i < lc; i++) {
                    if (_layers[i].IsVisible) {
                        if (_layers[i].Graphics != null) {
                            Int32 count = _layers[i].Graphics.Count;
                            for (Int32 n = 0; n < count; n++) {
                                DrawObject drawObject = _layers[i].Graphics[n];
                                drawObjectList.Add(drawObject);
                            }
                        }
                    }
                }
            }

            return drawObjectList;
        }

        /// <summary>
        /// 清空所有图形数据
        /// </summary>
        /// <returns></returns>
        public void ClearAll()
        {
            ClearHistory();
            _layers.Clear();

            Invalidate();
        }

        /// <summary>
        /// Delete selected items
        /// </summary>
        public void DeleteSelection()
        {
            if (DeleteDrawObject != null) {
                List<DrawObject> drawObjectList = new List<DrawObject>();
                foreach (DrawObject o in TheLayers[TheLayers.ActiveLayerIndex].Graphics.Selection) {
                    drawObjectList.Add(o);
                }

                DeleteDrawObject.BeginInvoke(drawObjectList, null, null);
            }

            TheLayers[TheLayers.ActiveLayerIndex].Graphics.DeleteSelection();
            Invalidate();
        }

        /// <summary>
        /// 移除绘制对象
        /// </summary>
        /// <param name="o"></param>
        public void RemoveDrawObject(DrawObject o)
        {
            TheLayers[TheLayers.ActiveLayerIndex].Graphics.DeleteDrawObject(o);
            Invalidate();
        }

        /// <summary>
        /// Add Graphics Object
        /// </summary>
        /// <param name="o"></param>
        public void AddGraphicsObject(DrawObject o)
        {
            o.ID = DrawObject.sCurrentDrawObjectId++;
            TheLayers[TheLayers.ActiveLayerIndex].Graphics.Add(o);
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            ClearHistory();
            _layers.Clear();

            _zoom = 1.0f;
            _panX = 0;
            _panY = 0;

            CalcBoundary();
        }

        public void SelectByTag(Object tag)
        {
            if (_layers != null) {
                Int32 count = _layers.Count;
                for (Int32 i = 0; i < count; i++) {
                    Layer layer = _layers[i];
                    if (layer != null) {
                        layer.Graphics.UnselectAll();
                    }
                }

                int lc = _layers.Count;
                for (int i = 0; i < lc; i++) {
                    if (_layers[i].IsVisible) {
                        if (_layers[i].Graphics != null) {
                            count = _layers[i].Graphics.Count;
                            for (Int32 n = 0; n < count; n++) {
                                DrawObject drawObject = _layers[i].Graphics[n];
                                if (tag == drawObject.Tag) {
                                    drawObject.Selected = true;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region internal functions
        /// <summary>
        /// Back Track the Mouse to return accurate coordinates regardless of zoom or pan effects.
        /// Courtesy of BobPowell.net <seealso cref="http://www.bobpowell.net/backtrack.htm"/>
        /// </summary>
        /// <param name="p">Point to backtrack</param>
        /// <returns>Backtracked point</returns>
        internal Point BackTrackMouse(Point p)
        {
            p = this.ConvertPoint(p);
            p.X = (Int32)((p.X - this.Boundary.X) / this.ScaleX);
            p.Y = (Int32)((p.Y - this.Boundary.Y) / this.ScaleY);

            return p;
        }

        /// <summary>
        /// Add command to history.
        /// </summary>
        internal void AddCommandToHistory(Command command)
        {
            undoManager.AddCommandToHistory(command);
        }

        /// <summary>
        /// Clear Undo history.
        /// </summary>
        internal void ClearHistory()
        {
            undoManager.ClearHistory();
        }

        /// <summary>
        /// Undo
        /// </summary>
        internal void Undo()
        {
            undoManager.Undo();
            Invalidate();
        }

        /// <summary>
        /// Redo
        /// </summary>
        internal void Redo()
        {
            undoManager.Redo();

             Invalidate();
        }

        /// <summary>
        ///  Draw group selection rectangle
        /// </summary>
        /// <param name="g"></param>
        internal void DrawNetSelection(Graphics g)
        {
            if (!DrawNetRectangle)
                return;

            Rectangle rect = new Rectangle(
                (Int32)(NetRectangle.X * mScaleX + mBoundary.X),
                (Int32)(NetRectangle.Y * mScaleY + mBoundary.Y),
                (Int32)(NetRectangle.Width * mScaleX),
                (Int32)(NetRectangle.Height * mScaleY));
            ControlPaint.DrawFocusRectangle(g, rect, Color.Black, Color.Transparent);

        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Draw graphic objects and group selection rectangle (optionally)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawArea_Paint(object sender, PaintEventArgs e)
        {
            RenderView(e.Graphics);
        }

        /// <summary>
        /// Mouse down.
        /// Left button down event is passed to active tool.
        /// Right button down event is handled in this class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawArea_MouseDown(object sender, MouseEventArgs e)
        {
            if (!CanControl)
                return;

            if ((mDrawToolbar != null) && (mDrawToolbar.DrawArea != this)) {
                mDrawToolbar.DrawArea = this;
                DrawAreaMouseDown?.Invoke();
            }

            lastPoint = this.ConvertPoint(e.Location);
            if (!mBoundary.Contains(lastPoint)) {
                Cursor = Cursors.Default;
                return;
            }

            lastPoint.X = (Int32)((lastPoint.X - mBoundary.X) / mScaleX);
            lastPoint.Y = (Int32)((lastPoint.Y - mBoundary.Y) / mScaleY);
            if (e.Button ==
              MouseButtons.Left)
                tools[(int)activeTool].OnMouseDown(this, e);
            else if (e.Button ==
                 MouseButtons.Right) {
                if (_panning)
                    _panning = false;
                if (activeTool == DrawToolType.Polygon)
                    tools[(int)activeTool].OnMouseDown(this, e);
                //ActiveTool = DrawToolType.Pointer;
            }
        }

        /// <summary>
        /// Mouse move.
        /// Moving without button pressed or with left button pressed
        /// is passed to active tool.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (!CanControl)
                return;

            Point curLoc = this.ConvertPoint(e.Location);
            if (!mBoundary.Contains(curLoc)) {
                Cursor = Cursors.Default;
                return;
            }

            if (e.Button == MouseButtons.Left ||
              e.Button == MouseButtons.None)
                if (e.Button == MouseButtons.Left && _panning) {
                    Cursor = Cursors.Hand;
                    curLoc.X = (Int32)((curLoc.X - mBoundary.X) / mScaleX);
                    curLoc.Y = (Int32)((curLoc.Y - mBoundary.Y) / mScaleY);

                    Int32 panX = 0;
                    Int32 panY = 0;
                    if (curLoc.X != lastPoint.X)
                        panX = curLoc.X - lastPoint.X;

                    if (curLoc.Y != lastPoint.Y)
                        panY = curLoc.Y - lastPoint.Y;

                    PanX += (Int32)(panX * ScaleX);
                    PanY += (Int32)(panY * ScaleY);
                    Invalidate();
                }
                else {
                    tools[(int)activeTool].OnMouseMove(this, e);
                }
            else
                Cursor = Cursors.Default;

            lastPoint = BackTrackMouse(e.Location);
           // this.toolTip1.SetToolTip(this, e.X + "\t" + e.Y);
        }

        /// <summary>
        /// Mouse up event.
        /// Left button up event is passed to active tool.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawArea_MouseUp(object sender, MouseEventArgs e)
        {
            if (!CanControl)
                return;

            Point p = this.ConvertPoint(e.Location);
            if (!mBoundary.Contains(p)) {
                Cursor = Cursors.Default;
                return;
            }

            //lastPoint = BackTrackMouse(e.Location);
            if ((e.Button == MouseButtons.Left) || ((e.Button == MouseButtons.Right) && (activeTool == DrawToolType.Polygon))) {
                //this.AddCommandToHistory(new CommandAdd(this.TheLayers[al].Graphics[0]));
                tools[(int)activeTool].OnMouseUp(this, e);
            }
        }

        private void DrawArea_Shown(object sender, EventArgs e)
        {
            CalcBoundary();
        }

        private void DrawArea_Move(object sender, EventArgs e)
        {
            Int32 deltaX = 0, deltaY = 0;
            Point point = this.ConvertPoint(ClientRectangle.Location);
            if (point.X != mLeftTopPoint.X || point.Y != mLeftTopPoint.Y) {
                deltaX = point.X - mLeftTopPoint.X;
                deltaY = point.Y - mLeftTopPoint.Y;

                mLeftTopPoint = point;

                mBoundary.X = mBoundary.X + deltaX;
                mBoundary.Y = mBoundary.Y + deltaY;

                mOrignBoundary = mBoundary;
            }
        }

        private void DrawArea_Resize(object sender, EventArgs e)
        {
            Int32 deltaWidth = 0, deltaHeight = 0;
            if (mSize.Width != ClientSize.Width || mSize.Height != ClientSize.Height) {
                deltaWidth = ClientSize.Width - mSize.Width;
                deltaHeight = ClientSize.Height - mSize.Height;
                mSize = ClientSize;
            }

            mBoundary.X = mBoundary.X + deltaWidth / 2;
            mBoundary.Y = mBoundary.Y + deltaHeight / 2;

            mOrignBoundary = mBoundary;

            mLeftTopPoint = this.ConvertPoint(ClientRectangle.Location);
            //
            //mResolution = this.ClientSize;
            CalcBoundary();
            Invalidate();
        }

        private void DrawArea_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!CanControl)
                return;

            if (e.Delta != 0) {
                if (_controlKey) {
                    // We are panning up or down using the wheel
                    if (e.Delta > 0)
                        PanY += 10;
                    else
                        PanY -= 10;

                    Invalidate();
                }
                else {
                    // We are zooming in or out using the wheel
                    if (e.Delta > 0)
                        AdjustZoom(.1f);
                    else
                        AdjustZoom(-.1f);
                }
            }
        }

        private void DrawArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (!CanControl)
                return;

            int al = TheLayers.ActiveLayerIndex;
            switch (e.KeyCode) {
                case Keys.Delete:
                    if (DeleteDrawObject != null) {
                        List<DrawObject> drawObjectList = new List<DrawObject>();
                        foreach (DrawObject o in TheLayers[al].Graphics.Selection) {
                            drawObjectList.Add(o);
                        }

                        DeleteDrawObject.BeginInvoke(drawObjectList, null, null);
                    }

                    AddCommandToHistory(new CommandDelete(TheLayers));
                    TheLayers[al].Graphics.DeleteSelection();
                    break;
                case Keys.Right:
                    PanX -= 10;
                    break;
                case Keys.Left:
                    PanX += 10;
                    break;
                case Keys.Up:
                    if (e.KeyCode == Keys.Up &&
                        e.Shift)
                        AdjustZoom(.1f);
                    else
                        PanY += 10;
                    break;
                case Keys.Down:
                    if (e.KeyCode == Keys.Down &&
                        e.Shift)
                        AdjustZoom(-.1f);
                    else
                        PanY -= 10;
                    break;
                case Keys.ControlKey:
                    _controlKey = true;
                    break;
                default:
                    break;
            }

            Invalidate();
            if (mDrawToolbar != null)
                mDrawToolbar.SetStateOfControls();
        }

        private void DrawArea_KeyUp(object sender, KeyEventArgs e)
        {
            if (!CanControl)
                return;

            _controlKey = false;
        }
        #endregion

        #region Other Functions
        /// <summary>
        /// 渲染视图
        /// </summary>
        private void RenderView(Graphics g)
        {
            using (SolidBrush brush = new SolidBrush(Color.Transparent)) {
                g.FillRectangle(brush, ClientRectangle);
            }

            Rectangle boundary = mBoundary;
            if (BackgroundRenderEnabled && (mBitmap != null)) {
                BitmapData bitmapData = mBitmap.LockBits(new Rectangle(0, 0, mBitmap.Width, mBitmap.Height), ImageLockMode.WriteOnly, mBitmap.PixelFormat);
                Marshal.Copy(mBuffer, 0, bitmapData.Scan0, mBuffer.Length);
                mBitmap.UnlockBits(bitmapData);
                g.DrawImage(mBitmap, boundary);
            }

            // Draw objects on each layer, in succession so we get the correct layering. Only draw layers that are visible
            if (_layers != null) {
                int lc = _layers.Count;
                for (int i = 0; i < lc; i++) {
                    if (_layers[i].IsVisible) {
                        if (_layers[i].Graphics != null)
                            _layers[i].Graphics.Draw(g, boundary, mScaleX, mScaleY);
                    }
                }
            }

            DrawNetSelection(g);
        }

        /// <summary>
        /// Adjust the zoom by the amount given, within reason
        /// </summary>
        /// <param name="_amount">float value to adjust zoom by - may be positive or negative</param>
        private void AdjustZoom(float _amount)
        {
            Zoom += _amount;
            if (Zoom < .1f)
                Zoom = .1f;
            if (Zoom > 10)
                Zoom = 10f;

            Invalidate();
            if (mDrawToolbar != null)
                mDrawToolbar.SetStateOfControls();
        }

        /// <summary>
        /// Rotate the selected Object(s)
        /// </summary>
        /// <param name="p">Amount to rotate. Negative is Left, Positive is Right, Zero indicates Reset to zero</param>
        private void RotateObject(int p)
        {
            int al = TheLayers.ActiveLayerIndex;
            for (int i = 0; i < TheLayers[al].Graphics.Count; i++) {
                if (TheLayers[al].Graphics[i].Selected)
                    if (p == 0)
                        TheLayers[al].Graphics[i].Rotation = 0;
                    else
                        TheLayers[al].Graphics[i].Rotation += p;
            }

            Invalidate();
            if (mDrawToolbar != null)
                mDrawToolbar.SetStateOfControls();
        }

        /// <summary>
        /// 计算边界
        /// </summary>
        private void CalcBoundary()
        {
            if (mResolution.IsEmpty)
                return;

            // 不加锁保护以加速渲染
            mLeftTopPoint = this.ConvertPoint(ClientRectangle.Location);
            int w = 0, h = 0;
            int width = mResolution.Width, height = mResolution.Height;
            int destWidth = ClientRectangle.Width, destHeight = ClientRectangle.Height;
            if (IsZoomDrawed) {
                if (height > destHeight || width > destWidth) {
                    if ((width * destHeight) > (height * destWidth)) {
                        w = destWidth;
                        h = (destWidth * height) / width;
                    }
                    else {
                        h = destHeight;
                        w = (width * destHeight) / height;
                    }
                }
                else {
                    w = width;
                    h = height;
                }
            }
            else {
                w = ClientRectangle.Width;
                h = ClientRectangle.Height;
            }

            w = ClientRectangle.Width;
            h = ClientRectangle.Height;
            mBoundary.X = mLeftTopPoint.X + (destWidth - w) / 2;
            mBoundary.Y = mLeftTopPoint.Y + (destHeight - h) / 2;
            mBoundary.Width = mSize.Width;
            mBoundary.Height = h;

            mOrignBoundary = mBoundary;

            mSize = ClientSize;

            mScaleX = (float)w / mResolution.Width;
            mScaleY = (float)h / mResolution.Height;
        }

        /// <summary>
        /// 转换坐标点
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private Point ConvertPoint(Point p)
        {
            return p;
        }
        #endregion
    }
}