using Miscs;
using Repository.Entities;
using System.Collections.Generic;
using System.Linq;
using static IRService.Miscs.MethodUtils;

namespace IRService.Services.Cell
{
    /// <summary>
    /// 设备单元服务选区操作
    /// </summary>
    public partial class CellService
    {
        /// <summary>
        /// 获取选区
        /// </summary>
        /// <param name="name">选区名称</param>
        /// <returns>选区</returns>        
        [Synchronized("selections")]
        private Models.Selections.Selection GetSelection(string name)
        {
            return selections.First(selection => selection.Entity.name.Equals(name));
        }

        /// <summary>
        /// 获取选区列表
        /// </summary>
        /// <returns>选区列表</returns>
        [Synchronized("selections")]
        public List<Models.Selections.Selection> GetSelections()
        {
            return selections.Clone();
        }

        /// <summary>
        /// 更新选区
        /// </summary>
        /// <param name="selections">选区配置列表</param>
        [Synchronized("selections")]
        public void UpdateSelections(List<Selections.Selection> selections)
        {
            this.selections.Clear();
            AddSelections(selections);
        }

        /// <summary>
        /// 加载选区列表
        /// </summary>
        /// <returns>是否成功</returns>
        [Synchronized("selections")]
        private bool LoadSelections()
        {
            var selections = Repository.Repository.LoadSelections();
            if (selections == null) {
                return false;
            }

            UpdateSelections(selections.selections);

            return true;
        }

        /// <summary>
        /// 添加选区列表
        /// </summary>
        /// <param name="selections">选区配置列表</param>
        [Synchronized("selections")]
        private void AddSelections(List<Selections.Selection> selections)
        {
            foreach (var selection in selections) {
                this.selections.Add(CreateSelection(selection));
            }
        }

        /// <summary>
        /// 创建选区
        /// </summary>
        /// <param name="selection">选区配置</param>
        /// <returns>选区</returns>
        private static Models.Selections.Selection CreateSelection(Selections.Selection selection)
        {
            Models.Selections.Selection model;
            switch (selection.type) {
                case Selections.SelectionType.Point:
                    model = new Models.Selections.PointSelection() { entity = selection as Selections.PointSelection };
                    break;
                case Selections.SelectionType.Line:
                    model = new Models.Selections.LineSelection() { entity = selection as Selections.LineSelection };
                    break;
                case Selections.SelectionType.Rectangle:
                    model = new Models.Selections.RectangleSelection() { entity = selection as Selections.RectangleSelection };
                    break;
                case Selections.SelectionType.Ellipse:
                    model = new Models.Selections.EllipseSelection() { entity = selection as Selections.EllipseSelection };
                    break;
                default:
                    return null;
            }

            model.Initialize();
            return model;
        }
    }
}
