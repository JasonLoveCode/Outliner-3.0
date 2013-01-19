﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outliner.Scene;
using System.Xml.Serialization;
using System.ComponentModel;
using Outliner.Plugins;
using System.Windows.Forms;
using System.Globalization;

namespace Outliner.Controls.ContextMenu
{
[OutlinerPlugin(OutlinerPluginType.ContextMenuItemModel)]
[LocalizedDisplayName(typeof(ContextMenuResources), "Str_ActionMenuItemModel")]
public class ActionMenuItemModel : MenuItemModel
{
   public ActionMenuItemModel() : this(String.Empty, String.Empty, null) { }

   public ActionMenuItemModel(String text, String image, Type resType) 
      : base(text, image, resType)
   {
      this.EnabledPredicate = "";
      this.OnClickAction = "";
      this.CheckedPredicate = String.Empty;
   }

   [XmlElement("enabled")]
   [DefaultValue("None")]
   [DisplayName("Enabled Predicate")]
   [Category("2. Action Properties")]
   [TypeConverter(typeof(OutlinerPredicateConverter))]
   public String EnabledPredicate { get; set; }

   [XmlElement("checked")]
   [DefaultValue("None")]
   [DisplayName("Checked Predicate")]
   [Category("2. Action Properties")]
   [TypeConverter(typeof(OutlinerPredicateConverter))]
   public String CheckedPredicate { get; set; }

   [XmlElement("onclick")]
   [DefaultValue("None")]
   [DisplayName("OnClick Action")]
   [Category("2. Action Properties")]
   [TypeConverter(typeof(OutlinerActionConverter))]
   public String OnClickAction { get; set; }


   protected override Boolean Enabled( Outliner.Controls.Tree.TreeView treeView
                                     , Outliner.Controls.Tree.TreeNode clickedTn)
   {
      Throw.IfArgumentIsNull(treeView, "treeView");

      IEnumerable<IMaxNode> context = HelperMethods.GetMaxNodes(treeView.SelectedNodes);
      OutlinerPredicate predicate = OutlinerActions.GetPredicate(this.EnabledPredicate);

      if (predicate != null)
         return predicate(clickedTn, context);
      else
         return base.Enabled(treeView, clickedTn);
   }


   protected override Boolean Checked( Outliner.Controls.Tree.TreeView treeView
                                     , Outliner.Controls.Tree.TreeNode clickedTn)
   {
      Throw.IfArgumentIsNull(treeView, "treeView");

      IEnumerable<IMaxNode> context = HelperMethods.GetMaxNodes(treeView.SelectedNodes);
      OutlinerPredicate predicate = OutlinerActions.GetPredicate(this.CheckedPredicate);

      if (predicate != null)
         return predicate(clickedTn, context);
      else
         return base.Checked(treeView, clickedTn);
   }


   protected override void OnClick(Outliner.Controls.Tree.TreeView treeView
                                  , Outliner.Controls.Tree.TreeNode clickedTn)
   {
      Throw.IfArgumentIsNull(treeView, "treeView");

      IEnumerable<IMaxNode> context = HelperMethods.GetMaxNodes(treeView.SelectedNodes);
      OutlinerAction action = OutlinerActions.GetAction(this.OnClickAction);

      if (action != null)
         action(clickedTn, context);
      else
         MessageBox.Show(String.Format(CultureInfo.CurrentCulture
                                       , ContextMenuResources.Str_ActionNotFound
                                       , this.OnClickAction)
                        , ContextMenuResources.Str_ContextMenuWarningTitle
                        , MessageBoxButtons.OK
                        , MessageBoxIcon.Warning
                        , MessageBoxDefaultButton.Button1
                        , ControlHelpers.GetLocalizedMessageBoxOptions());
   }
}
}
