﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Outliner.Scene;
using Outliner.Commands;

namespace Outliner.Controls.Tree.DragDropHandlers
{
public class IINodeDragDropHandler : DragDropHandler
{
   public IINodeDragDropHandler(IMaxNodeWrapper data) : base(data) { }

   public override bool AllowDrag
   {
      get { return true; }
   }

   public override bool IsValidDropTarget(IDataObject dragData)
   {
      IEnumerable<TreeNode> draggedNodes = this.GetNodesFromDataObject(dragData);
      if (draggedNodes == null)
         return false;

      return this.Data.CanAddChildNodes(HelperMethods.GetMaxNodes(draggedNodes));
   }

   public override DragDropEffects GetDragDropEffect(IDataObject dragData)
   {
      if (this.IsValidDropTarget(dragData))
         return DragDropEffects.Link;
      else
         return DragDropEffects.None;
   }

   public override void HandleDrop(IDataObject dragData)
   {
      if (!this.IsValidDropTarget(dragData))
         return;

      IEnumerable<TreeNode> draggedNodes = this.GetNodesFromDataObject(dragData);
      if (draggedNodes == null)
         return;

      LinkIINodeCommand cmd = new LinkIINodeCommand(HelperMethods.GetMaxNodes(draggedNodes), this.Data);
      cmd.Execute(true);
   }
}
}
