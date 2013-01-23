﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinForms = System.Windows.Forms;
using Outliner.Scene;
using Outliner.Commands;
using Outliner.Controls.Tree;

namespace Outliner.Modes.Layer
{
public class ILayerDragDropHandler : MaxNodeDragDropHandler
{
   private ILayerWrapper layer;
   public ILayerDragDropHandler(ILayerWrapper data)
      : base(data)
   {
      this.layer = data;
   }

   public override bool AllowDrag
   {
      get { return !this.layer.IsDefault; }
   }

   public override bool IsValidDropTarget(WinForms::IDataObject dragData)
   {
      return this.MaxNode.CanAddChildNodes(GetMaxNodesFromDragData(dragData));
   }

   public override WinForms.DragDropEffects DefaultDragDropEffect
   {
      get { return WinForms::DragDropEffects.Copy; }
   }

   public override void HandleDrop(WinForms::IDataObject dragData)
   {
      if (!this.IsValidDropTarget(dragData))
         return;

      IEnumerable<IMaxNode> draggedNodes = GetMaxNodesFromDragData(dragData);

      MoveMaxNodeCommand cmd = new MoveMaxNodeCommand( draggedNodes
                                                     , this.MaxNode
                                                     , Resources.Command_AddToLayer
                                                     , Resources.Command_UnlinkLayer);
      cmd.Execute(true);
   }
}
}
