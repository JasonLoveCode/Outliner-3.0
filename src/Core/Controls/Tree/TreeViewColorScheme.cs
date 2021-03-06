﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Autodesk.Max;
using System.Reflection;
using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;

namespace PJanssen.Outliner.Controls.Tree
{
public class TreeViewColorScheme
{
   /// <summary>
   /// Indicates whether the ColorScheme can be edited by the user.
   /// </summary>
   [XmlIgnore]
   public Boolean Editable { get; set; }

   public SerializableColor ForegroundLight { get; set; }
   public SerializableColor ForegroundDark { get; set; }

   public SerializableColor Background { get; set; }
   public SerializableColor AltBackground { get; set; }

   public SerializableColor SelectionForeground { get; set; }
   public SerializableColor SelectionBackground { get; set; }

   public SerializableColor DropTargetForeground { get; set; }
   public SerializableColor DropTargetBackground { get; set; }

   public SerializableColor ParentForeground { get; set; }
   public SerializableColor ParentBackground { get; set; }

   public SerializableColor ContextMenuBackground { get; set; }
   public SerializableColor ContextMenuForeground { get; set; }

   [DefaultValue(false)]
   public Boolean AlternateBackground { get; set; }

   public TreeViewColorScheme()
   {
      this.Editable              = true;
      this.ForegroundLight       = new SerializableColor(Color.White);
      this.ForegroundDark        = new SerializableColor(Color.Black);
      this.Background            = new SerializableColor(Color.White);
      this.AltBackground         = new SerializableColor(Color.LightGray);
      this.SelectionForeground   = new SerializableColor(SystemColors.HighlightText);
      this.SelectionBackground   = new SerializableColor(SystemColors.Highlight);
      this.DropTargetForeground  = new SerializableColor(SystemColors.WindowText);
      this.DropTargetBackground  = new SerializableColor(255, 177, 177);
      this.ParentForeground      = new SerializableColor(SystemColors.WindowText);
      this.ParentBackground      = new SerializableColor(177, 255, 177);
      this.ContextMenuBackground = new SerializableColor(93, 93, 93);
      this.ContextMenuForeground = new SerializableColor(230, 230, 230);
   }

   public void UpdateColors()
   {
      if (this.ForegroundLight.IsGuiColor)
         this.ForegroundLight = new SerializableColor(this.ForegroundLight.GuiColor);
      if (this.ForegroundDark.IsGuiColor)
         this.ForegroundDark = new SerializableColor(this.ForegroundDark.GuiColor);
      if (this.Background.IsGuiColor)
         this.Background = new SerializableColor(this.Background.GuiColor);
      if (this.AltBackground.IsGuiColor)
         this.AltBackground = new SerializableColor(this.AltBackground.GuiColor);
      if (this.SelectionForeground.IsGuiColor)
         this.SelectionForeground = new SerializableColor(this.SelectionForeground.GuiColor);
      if (this.SelectionBackground.IsGuiColor)
         this.SelectionBackground = new SerializableColor(this.SelectionBackground.GuiColor);
      if (this.DropTargetForeground.IsGuiColor)
         this.DropTargetForeground = new SerializableColor(this.DropTargetForeground.GuiColor);
      if (this.DropTargetBackground.IsGuiColor)
         this.DropTargetBackground = new SerializableColor(this.DropTargetBackground.GuiColor);
      if (this.ParentForeground.IsGuiColor)
         this.ParentForeground = new SerializableColor(this.ParentForeground.GuiColor);
      if (this.ParentBackground.IsGuiColor)
         this.ParentBackground = new SerializableColor(this.ParentBackground.GuiColor);
   }

   public static TreeViewColorScheme FromXml(String path)
   {
      using (FileStream stream = new FileStream(path, FileMode.Open))
      {
         return TreeViewColorScheme.FromXml(stream);
      }
   }

   public static TreeViewColorScheme FromXml(Stream stream)
   {
      XmlSerializer xs = new XmlSerializer(typeof(TreeViewColorScheme));
      return xs.Deserialize(stream) as TreeViewColorScheme;
   }

   public void ToXml(String path)
   {
      using (FileStream stream = new FileStream(path, FileMode.Create))
      {
         this.ToXml(stream);
      }
   }

   public void ToXml(Stream stream)
   {
      XmlSerializer xs = new XmlSerializer(typeof(TreeViewColorScheme));
      xs.Serialize(stream, this);
   }

   public static TreeViewColorScheme MaxColors
   {
      get
      {
         TreeViewColorScheme c = new TreeViewColorScheme();
         
         c.Editable             = false;
         c.ForegroundLight      = new SerializableColor(200, 200, 200);
         c.ForegroundDark       = new SerializableColor(42, 42, 42);
         c.Background           = new SerializableColor(GuiColors.Window);
         c.AltBackground        = (c.Background.Color.GetBrightness() < 0.5f)
            ? new SerializableColor(c.Background.Color.R + 10, c.Background.Color.G + 10, c.Background.Color.B + 10)
            : new SerializableColor(c.Background.Color.R - 10, c.Background.Color.G - 10, c.Background.Color.B - 10);
         c.SelectionForeground  = new SerializableColor(GuiColors.HilightText);
         c.SelectionBackground  = new SerializableColor(GuiColors.Hilight);
         c.DropTargetForeground = c.ForegroundDark;
         c.DropTargetBackground = new SerializableColor(255, 177, 177);
         c.ParentForeground     = c.ForegroundDark;
         c.ParentBackground     = new SerializableColor(177, 255, 177);

         return c;
      }
   }

   public static TreeViewColorScheme MayaColors
   {
      get
      {
         TreeViewColorScheme c = new TreeViewColorScheme();

         c.AlternateBackground  = true;
         c.Editable             = false;
         c.ForegroundLight      = new SerializableColor(220, 220, 220);
         c.ForegroundDark       = new SerializableColor(32, 32, 32);
         c.Background           = new SerializableColor(42, 42, 42);
         c.AltBackground        = new SerializableColor(48, 48, 48);
         c.SelectionForeground  = new SerializableColor(255, 255, 255);
         c.SelectionBackground  = new SerializableColor(103, 141, 178);
         c.DropTargetForeground = new SerializableColor(0, 0, 0);
         c.DropTargetBackground = new SerializableColor(255, 177, 177);
         c.ParentForeground     = new SerializableColor(220, 220, 220);
         c.ParentBackground     = new SerializableColor(65, 77, 90);

         return c;
      }
   }
}
}
