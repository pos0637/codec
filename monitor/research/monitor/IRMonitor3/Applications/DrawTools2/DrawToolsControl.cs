using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DrawTools2
{
    public partial class DrawToolsControl : UserControl
    {
        public DrawToolsControl()
        {
            InitializeComponent();
            drawArea1.BackgroundRenderEnabled = false;
            drawArea1.CanControl = true;
            drawToolbar1.CanControl = false;
            drawToolbar1.PolygonEnabled = true;
            drawToolbar1.EllipseEnabled = true;
            drawToolbar1.RectangleEnabled = true;
            drawToolbar1.CanControl = true;
            drawToolbar1.LineEnabled = true;
            drawToolbar1.DrawArea = drawArea1;
        }
    }
}
