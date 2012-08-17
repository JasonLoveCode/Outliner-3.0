﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outliner.Scene;

namespace Outliner.Commands
{
   /// <summary>
   /// A generic command used to link nodes, move them to a new layer, or 
   /// reparent layers.
   /// </summary>
   public class MoveMaxNodeCommand : Command
   {
      private IEnumerable<IMaxNodeWrapper> nodes;
      private IMaxNodeWrapper newParent;
      private Dictionary<IMaxNodeWrapper, IMaxNodeWrapper> oldParents;
      private String linkDescription;
      private String unlinkDescription;

      /// <summary>
      /// Creates a new MoveMaxNodeCommand.
      /// </summary>
      /// <param name="nodes">The nodes to move.</param>
      /// <param name="newParent">The new parent for the nodes. Use null to unlink.</param>
      /// <param name="linkDescription">The command description for a link action.</param>
      /// <param name="unlinkDescription">The command description for an unlink action.</param>
      public MoveMaxNodeCommand( IEnumerable<IMaxNodeWrapper> nodes
                               , IMaxNodeWrapper newParent
                               , String linkDescription
                               , String unlinkDescription)
      {
         ExceptionHelper.ThrowIfArgumentIsNull(nodes, "nodes");
         ExceptionHelper.ThrowIfArgumentIsNull(linkDescription, "linkDescription");
         ExceptionHelper.ThrowIfArgumentIsNull(unlinkDescription, "unlinkDescription");
         
         this.nodes = nodes;
         this.newParent = newParent;
         this.linkDescription = linkDescription;
         this.unlinkDescription = unlinkDescription;
      }

      public override string Description
      {
         get
         {
            if (this.newParent != null)
               return this.linkDescription;
            else
               return this.unlinkDescription;
         }
      }

      public override void Do()
      {
         if (this.nodes == null)
            return;

         this.oldParents = new Dictionary<IMaxNodeWrapper, IMaxNodeWrapper>(this.nodes.Count());
         foreach (IMaxNodeWrapper node in this.nodes)
            this.oldParents.Add(node, node.Parent);

         if (this.newParent != null)
            this.newParent.AddChildNodes(this.nodes);
         else
            this.nodes.ForEach(n => n.Parent.RemoveChildNode(n));
      }

      public override void Undo()
      {
         if (this.nodes == null || this.oldParents == null)
            return;

         foreach (KeyValuePair<IMaxNodeWrapper, IMaxNodeWrapper> n in this.oldParents)
            n.Value.AddChildNode(n.Key);
      }
   }
}
