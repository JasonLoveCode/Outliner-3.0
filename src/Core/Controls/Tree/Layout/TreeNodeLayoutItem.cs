﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.ComponentModel;
using PJanssen.Outliner.Scene;
using PJanssen.Outliner.Modes;

namespace PJanssen.Outliner.Controls.Tree.Layout
{
   /// <summary>
   /// Defines a base class for drawing TreeNodes in a TreeView.
   /// </summary>
   [XmlInclude(typeof(ExpandButton))]
   [XmlInclude(typeof(EmptySpace))]
   [XmlInclude(typeof(TreeNodeIndent))]
   [XmlInclude(typeof(TreeNodeText))]
   public abstract class TreeNodeLayoutItem
   {
      /// <summary>
      /// Initializes a new instance of the TreeNodeLayoutItem
      /// </summary>
      protected TreeNodeLayoutItem()
      {
         this.VisibleTypes = MaxNodeType.All;
         this.Width = AutoWidth;
      }


      /// <summary>
      /// Creates a deep copy of this TreeNodeLayoutItem.
      /// </summary>
      public abstract TreeNodeLayoutItem Copy();

      /// <summary>
      /// Gets the Layout to which this TreeNodeLayoutItem belongs.
      /// </summary>
      [XmlIgnore]
      [Browsable(false)]
      public TreeNodeLayout Layout { get; internal set; }

      /// <summary>
      /// The number of blank pixels to the left of the layout item.
      /// </summary>
      [XmlAttribute("padding_left")]
      [DefaultValue(0)]
      public virtual Int32 PaddingLeft { get; set; }

      /// <summary>
      /// The number of blank pixels to the right of the layout item.
      /// </summary>
      [XmlAttribute("padding_right")]
      [DefaultValue(0)]
      public virtual Int32 PaddingRight { get; set; }

      /// <summary>
      /// Determines for which MaxNodeTypes this item will be visible.
      /// </summary>
      [XmlAttribute("visible_types")]
      [DefaultValue(MaxNodeType.All)]
      public virtual MaxNodeType VisibleTypes { get; set; }

      /// <summary>
      /// Indicates whether the item should be shown for treenodes which don't
      /// have an IMaxNode attached.
      /// </summary>
      [XmlIgnore]
      [DefaultValue(true)]
      public virtual Boolean ShowForNonMaxNodes 
      {
         get { return true; }
      }
      
      /// <summary>
      /// The item will only be shown if this method returns true.
      /// </summary>
      public virtual Boolean IsVisible(TreeNode tn)
      {
         IMaxNode node = TreeMode.GetMaxNode(tn);
         if (node == null)
            return this.ShowForNonMaxNodes;
         else
            return node.IsNodeType(this.VisibleTypes);
      }

      /// <summary>
      /// If true, the GetBounds method will return an item bounds rectangle
      /// which is centered relative to the tree.ItemHeight.
      /// </summary>
      [Browsable(false)]
      public virtual Boolean CenterVertically { get { return true; } }

      /// <summary>
      /// Returns the position of the item.
      /// </summary>
      public virtual Point GetPos(TreeNode tn)
      {
         if (tn == null)
            return Point.Empty;
         if (this.Layout == null || this.Layout.TreeView == null)
            return Point.Empty;

         Point pt = new Point(0, tn.Bounds.Y);
         pt.X = this.Layout.PaddingLeft - this.Layout.TreeView.HorizontalScroll.Value;
         
         foreach (TreeNodeLayoutItem item in this.Layout.LayoutItems)
         {
            if (item.IsVisible(tn))
            {
               pt.X += item.PaddingLeft;
               if (item == this)
                  break;
               pt.X += item.GetWidth(tn) + item.PaddingRight;
            }
         }

         return pt;
      }

      /// <summary>
      /// A value which indicates that the width should be calculated by the layoutitem.
      /// </summary>
      public const Int32 AutoWidth = -1;

      /// <summary>
      /// Gets or sets a fixed width for the TreeNodeLayoutItem.
      /// </summary>
      [XmlAttribute("width")]
      [DefaultValue(AutoWidth)]
      public Int32 Width { get; set; }

      /// <summary>
      /// Returns the width of the item.
      /// </summary>
      public Int32 GetWidth(TreeNode tn)
      {
         if (this.Width == AutoWidth)
            return GetAutoWidth(tn);
         else
            return this.Width;
      }

      /// <summary>
      /// Calculates a dynamic width value for the TreeNodeLayoutItem.
      /// </summary>
      protected abstract Int32 GetAutoWidth(TreeNode tn);

      /// <summary>
      /// Returns the height of the item.
      /// </summary>
      public abstract Int32 GetHeight(TreeNode tn);

      /// <summary>
      /// Returns the size of the item.
      /// </summary>
      public virtual Size GetSize(TreeNode tn)
      {
         return new Size(this.GetWidth(tn), this.GetHeight(tn));
      }

      /// <summary>
      /// The bounds of the item.
      /// </summary>
      public virtual Rectangle GetBounds(TreeNode tn)
      {
         if (this.Layout == null || tn == null)
            return Rectangle.Empty;

         Rectangle b = new Rectangle(this.GetPos(tn), this.GetSize(tn));
         if (this.CenterVertically && this.Layout != null)
            b.Y += (this.Layout.ItemHeight - b.Height) / 2;

         return b;
      }

      /// <summary>
      /// Draws the item for the given TreeNode at the given position.
      /// </summary>
      public abstract void Draw(Graphics graphics, TreeNode tn);

      /// <summary>
      /// This method is called when the mouse pointer is moved over the item for the first time.
      /// </summary>
      public virtual void HandleMouseEnter(MouseEventArgs e, TreeNode tn) { }

      /// <summary>
      /// This method is called when the mouse pointer is moved from within to outside the bounds of the item.
      /// </summary>
      public virtual void HandleMouseLeave(MouseEventArgs e, TreeNode tn) { }

      /// <summary>
      /// This method is called when a mouse button is held down over a TreeNode.
      /// </summary>
      public virtual void HandleMouseDown(MouseEventArgs e, TreeNode tn) { }

      /// <summary>
      /// This method is called when a mouse button is released over a TreeNode.
      /// </summary>
      public virtual void HandleMouseUp(MouseEventArgs e, TreeNode tn) { }

      /// <summary>
      /// This method is called when a TreeNode is double-clicked.
      /// </summary>
      public virtual void HandleDoubleClick(MouseEventArgs e, TreeNode tn) { }
   }

}
