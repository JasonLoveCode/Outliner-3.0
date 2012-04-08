﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Max;
using Outliner.Controls;

namespace Outliner.Scene
{
   public class IILayerWrapper : IMaxNodeWrapper
   {
      private IILayer layer;
      //private IIFPLayerManager manager;
      private IILayerManager manager;

      public IILayerWrapper(IILayer layer)
      {
         this.layer = layer;
         IGlobal g = GlobalInterface.Instance;
         //IInterface_ID int_ID = g.Interface_ID.Create((uint)BuiltInInterfaceIDA.LAYERMANAGER_INTERFACE, 
         //                                             (uint)BuiltInInterfaceIDB.LAYERMANAGER_INTERFACE);
         //this.manager = (IIFPLayerManager)g.GetCOREInterface(int_ID);
         this.manager = (IILayerManager)g.COREInterface.ScenePointer.GetReference(10);
      }

      public override object WrappedNode
      {
         get { return this.layer; }
      }

      /// <summary>
      /// Gets whether this layer is the default (0) layer.
      /// </summary>
      public Boolean IsDefault
      {
         get
         {
            if (this.manager == null)
               return false;
            else
               return this.manager.RootLayer.Handle == this.layer.Handle;
         }
      }

      public Boolean IsCurrent
      {
         get { return this.manager.CurrentLayer.Handle == this.layer.Handle; }
         set
         {
            if (!value)
               throw new ArgumentException("Cannot set IsCurrent to false. Instead, use IsCurrent = true on the new current layer.");

            if (this.manager != null)
            {
               String name = this.Name;
               this.manager.SetCurrentLayer(ref name);
            }
         }
      }

      public override string Name
      {
         get { return layer.Name; }
         set { layer.SetName(ref value); }
      }

      public override string DisplayName
      {
         get
         {
            if (this.IsDefault)
               return this.Name + " (default)";
            else
               return this.Name;
         }
      }

      public override bool CanEditName
      {
         get { return !this.IsDefault; }
      }

      public override IEnumerable<Object> ChildNodes
      {
         get
         {
            List<Object> nodes = new List<Object>();
            //for (int i = 0; i < this.node.NumberOfChildren; i++)
            //   nodes.Add(IMaxNodeWrapper.Create(this.node.GetChildNode(i)));
            return nodes;
         }
      }

      public override bool CanAddChildNode(IMaxNodeWrapper node)
      {
         if (node is IINodeWrapper)
         {
            IINode n = (IINode)node.WrappedNode;
            IILayer l = (IILayer)n.GetReference((int)ReferenceNumbers.NodeLayerRef);
            return this.layer.Handle != l.Handle;
         }
         else
            return false;
      }

      public override IClass_ID ClassID
      {
         get { return layer.ClassID; }
      }

      public override SClass_ID SuperClassID
      {
         get { return layer.SuperClassID; }
      }


      public override bool Selected
      {
         get { return false; }
      }

      public override bool IsNodeType(MaxNodeTypes types)
      {
         return types.HasFlag(MaxNodeTypes.Layer);
      }


      public override bool IsHidden
      {
         get { return this.layer.IsHidden; }
         set { this.layer.IsHidden = value; }
      }

      public override bool IsFrozen
      {
         get { return this.layer.IsFrozen; }
         set { this.layer.IsFrozen = value; }
      }

      public override bool BoxMode
      {
         get { return this.layer.BoxMode; }
         set { this.layer.BoxMode = value; }
      }

      public override System.Drawing.Color WireColor
      {
         get { return ColorHelpers.FromMaxColor(this.layer.WireColor); }
         set { this.layer.WireColor = value; }
      }

      public override bool Renderable
      {
         get { return this.layer.Renderable; }
         set { this.layer.Renderable = value; }
      }


      public override bool IsValid
      {
         get
         {
            if (!base.IsValid)
               return false;

            try { return !this.layer.TestAFlag(AnimatableFlags.IsDeleted); }
            catch { return false; }
         }
      }


      public const String IMGKEY_LAYER        = "layer";
      public const String IMGKEY_LAYER_ACTIVE = "layer_active";
      public override string ImageKey
      {
         get
         {
            if (this.IsCurrent)
               return IMGKEY_LAYER_ACTIVE;
            else
               return IMGKEY_LAYER;
         }
      }
   }
}
