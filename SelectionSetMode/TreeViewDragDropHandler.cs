﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinForms = System.Windows.Forms;
using Outliner.Scene;
using Outliner.Controls.Tree;
using Outliner.Commands;

namespace Outliner.Modes.SelectionSet
{
public class TreeViewDragDropHandler : IDragDropHandler
{
   public TreeViewDragDropHandler() { }

   public bool AllowDrag
   {
      get { return false; }
   }

   public bool IsValidDropTarget(WinForms::IDataObject dragData)
   {
      return true;
   }

   public WinForms::DragDropEffects GetDragDropEffect(WinForms::IDataObject dragData)
   {
      if (this.IsValidDropTarget(dragData))
         return WinForms::DragDropEffects.Move;
      else
         return TreeView.NoneDragDropEffects;
   }

   public void HandleDrop(WinForms::IDataObject dragData)
   {
      if (!this.IsValidDropTarget(dragData))
         return;

      IEnumerable<TreeNode> draggedNodes = TreeView.GetTreeNodesFromDragData(dragData);
      if (draggedNodes == null)
         return;

      List<SelectionSetWrapper> selSets = new List<SelectionSetWrapper>();
      foreach (TreeNode tn in draggedNodes)
      {
         if (tn.Parent == null)
            continue;

         SelectionSetWrapper selSet = HelperMethods.GetMaxNode(tn.Parent) as SelectionSetWrapper;
         if (selSet != null && !selSets.Contains(selSet))
            selSets.Add(selSet);
      }

      IEnumerable<IMaxNode> draggedMaxNodes = HelperMethods.GetMaxNodes(draggedNodes);
      foreach (SelectionSetWrapper selSet in selSets)
      {
         IEnumerable<IMaxNode> newNodes = selSet.ChildNodes.Except(draggedMaxNodes);
         ModifySelectionSetCommand cmd = new ModifySelectionSetCommand(selSet, newNodes);
         cmd.Execute(false);
      }
   }
}
}
