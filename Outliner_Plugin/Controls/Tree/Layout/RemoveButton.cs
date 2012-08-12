﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Outliner.Scene;
using Outliner.Commands;
using Autodesk.Max;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Outliner.Controls.Tree.Layout
{
public class RemoveButton : ImageButton
{
   public RemoveButton() 
      : base(NodeButtonImages.GetButtonImages(NodeButtonImages.Images.Remove)) { }

   [XmlAttribute("visible_types")]
   [DefaultValue(MaxNodeTypes.SelectionSet)]
   public override MaxNodeTypes VisibleTypes
   {
      get { return base.VisibleTypes & (MaxNodeTypes.SelectionSet); }
      set { base.VisibleTypes = value; }
   }

   public override bool IsEnabled(TreeNode tn)
   {
      if (this.Layout == null || this.Layout.TreeView == null)
         return false;

      SelectionSetWrapper node = HelperMethods.GetMaxNode(tn) as SelectionSetWrapper;
      ICollection<TreeNode> selTreeNodes = this.Layout.TreeView.SelectedNodes;
      if (node == null || selTreeNodes.Count == 0)
         return false;

      return node.CanRemoveChildNodes(HelperMethods.GetMaxNodes(selTreeNodes));
   }

   public override void HandleMouseUp(MouseEventArgs e, TreeNode tn)
   {
      if (this.Layout == null || this.Layout.TreeView == null)
         return;

      if (!this.IsEnabled(tn))
         return;

      SelectionSetWrapper selSet = HelperMethods.GetMaxNode(tn) as SelectionSetWrapper;
      if (selSet == null)
         return;

      IEnumerable<IMaxNodeWrapper> selNodes = HelperMethods.GetMaxNodes(this.Layout.TreeView.SelectedNodes);
      IEnumerable<IMaxNodeWrapper> newNodes = selSet.WrappedChildNodes.Except(selNodes);
      ModifySelectionSetCommand cmd = new ModifySelectionSetCommand(selSet, newNodes.ToList());
      cmd.Execute(true);
   }


   protected override string GetTooltipText(TreeNode tn)
   {
      IMaxNodeWrapper node = HelperMethods.GetMaxNode(tn);
      if (node == null || !this.IsEnabled(tn))
         return null;

      return OutlinerResources.Tooltip_Remove_SelSet;
   }
}
}