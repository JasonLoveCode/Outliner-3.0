﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinForms = System.Windows.Forms;
using PJanssen.Outliner.Controls.Tree;
using PJanssen.Outliner.Commands;
using PJanssen.Outliner.Scene;
using PJanssen.Outliner.Controls;

namespace PJanssen.Outliner.Modes.Hierarchy
{
public class GroupDragDropHandler : INodeDragDropHandler
{
   public GroupDragDropHandler(IMaxNode node) : base(node) { }

   public override bool AllowDrag
   {
      get { return true; }
   }

   public override WinForms::DragDropEffects GetDragDropEffect(WinForms::IDataObject dragData)
   {
      if (ControlHelpers.ShiftPressed)
         return base.GetDragDropEffect(dragData);
      else
      {
         if (this.IsValidDropTarget(dragData))
            return WinForms.DragDropEffects.Copy;
         else
            return TreeView.NoneDragDropEffects;
      }
   }

   public override void HandleDrop(WinForms::IDataObject dragData)
   {
      if (ControlHelpers.ShiftPressed)
         base.HandleDrop(dragData);
      else
      {
         if (!this.IsValidDropTarget(dragData))
            return;

         IEnumerable<IMaxNode> nodes = GetMaxNodesFromDragData(dragData);
         ChangeGroupCommand cmd = new ChangeGroupCommand(nodes, this.MaxNode, true);
         cmd.Execute(true);
      }
   }
}
}
