﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Outliner.Filters;
using Outliner.Scene;
using Outliner.Plugins;
using Outliner.MaxUtils;
using Autodesk.Max;
using Outliner.Controls.Tree.Layout;
using Outliner.Configuration;

namespace Outliner.Controls.Options
{
public partial class FilterCollectionEditor : OutlinerUserControl
{
   private FilterConfiguration filterConfiguration;
   private Filter<MaxNodeWrapper> rootFilter;
   private Dictionary<Filter<MaxNodeWrapper>, Tree.TreeNode> treeNodes;

   public FilterCollectionEditor()
   {
      InitializeComponent();

      this.treeNodes = new Dictionary<Filter<MaxNodeWrapper>, Tree.TreeNode>();

      this.filtersTree.TreeNodeLayout = new TreeNodeLayout();
      this.filtersTree.TreeNodeLayout.LayoutItems.Add(new TreeNodeIndent() { UseVisualStyles = false });
      this.filtersTree.TreeNodeLayout.LayoutItems.Add(new TreeNodeText());
      this.filtersTree.TreeNodeLayout.LayoutItems.Add(new EmptySpace());
      this.filtersTree.TreeNodeLayout.FullRowSelect = true;
   }

   public FilterCollectionEditor(FilterConfiguration config) : this()
   {
      Throw.IfArgumentIsNull(config, "config");

      this.RootFilter = config.Filter;
      this.filterConfiguration = config;
   }

   public FilterCollectionEditor(Filter<MaxNodeWrapper> rootFilter) : this()
   {
      Throw.IfArgumentIsNull(rootFilter, "rootFilter");

      this.RootFilter = rootFilter;
   }

   private void FillFilterComboBox()
   {
      IEnumerable<OutlinerPluginData> filters = OutlinerPlugins.GetPlugins(OutlinerPluginType.Filter);
      foreach (OutlinerPluginData filter in filters)
      {
         this.filtersComboBox.Items.Add(filter);
      }
      this.filtersComboBox.DisplayMember = "DisplayName";
      this.filtersComboBox.SelectedIndex = 0;
   }

   [Browsable(false)]
   [DefaultValue(null)]
   public Filter<MaxNodeWrapper> RootFilter
   {
      get { return this.rootFilter; }
      set
      {
         this.rootFilter = value;
         this.FillFilterComboBox();
         this.FillTree();
      }
   }

   public Action UpdateAction { get; set; }

   private void FillTree()
   {
      this.treeNodes.Clear();
      this.filtersTree.Nodes.Clear();
      if (this.rootFilter != null)
         this.AddFilterToTree(this.rootFilter, this.filtersTree.Nodes);
      this.filtersTree.Root.ExpandAll();
   }

   private void AddFilterToTree(Filter<MaxNodeWrapper> filter, Tree.TreeNodeCollection parentCollection)
   {
      filter.FilterChanged += filterChanged;

      Tree.TreeNode tn = new Tree.TreeNode();
      tn.Text = GetTreeNodeText(filter, tn);
      tn.Tag = filter;

      this.treeNodes.Add(filter, tn);

      FilterCombinator<MaxNodeWrapper> combinator = filter as FilterCombinator<MaxNodeWrapper>;
      if (combinator != null)
      {
         foreach (Filter<MaxNodeWrapper> child in combinator.Filters)
         {
            this.AddFilterToTree(child, tn.Nodes);
         }
         combinator.Filters.FilterAdded += this.filterAdded;
         combinator.Filters.FilterRemoved += this.filterRemoved;
         tn.DragDropHandler = new FilterCombinatorDragDropHandler(combinator);
      }
      else
         tn.DragDropHandler = new FilterDragDropHandler(filter);

      parentCollection.Add(tn);
   }

   private String GetTreeNodeText(Filter<MaxNodeWrapper> filter, Tree.TreeNode tn)
   {
      FilterCombinator<MaxNodeWrapper> combinator = filter as FilterCombinator<MaxNodeWrapper>;
      if (combinator != null)
      {
         return String.Format( "Combinator: {0}"
                             , Functor.PredicateToString<Boolean>(combinator.Predicate).ToUpper());
      }
      else if (filter is NodePropertyFilter)
      {
         return String.Format( "Property: {0}{1}"
                             , (filter.Invert ? "not " : "")
                             , (filter as NodePropertyFilter).Property.ToString());
      }
      else
      {
         OutlinerPluginData filterPluginData = OutlinerPlugins.GetPlugin(filter.GetType());
         if (filterPluginData != null)
            return filterPluginData.DisplayName;
         else
            return filter.GetType().Name;
      }
   }


   private void addFilterButton_Click(object sender, EventArgs e)
   {
      OutlinerPluginData selPlugin = this.filtersComboBox.SelectedItem as OutlinerPluginData;

      FilterCombinator<MaxNodeWrapper> combinator = null;
      Tree.TreeNode selCombinatorTn = this.filtersTree.SelectedNodes.FirstOrDefault();
      while (selCombinatorTn != null && combinator == null)
      {
         combinator = selCombinatorTn.Tag as FilterCombinator<MaxNodeWrapper>;
         selCombinatorTn = selCombinatorTn.Parent;
      }

      if (selPlugin != null)
      {
         Filter<MaxNodeWrapper> filter = Activator.CreateInstance(selPlugin.Type, null) as Filter<MaxNodeWrapper>;
         filter.FilterChanged += filterChanged;
         if (combinator != null)
            combinator.Filters.Add(filter);
         else if (this.rootFilter == null)
         {
            this.RootFilter = filter;
            if (this.filterConfiguration != null)
               this.filterConfiguration.Filter = filter;
         }
         else
         {
            Filter<MaxNodeWrapper> oldRoot = this.RootFilter;
            combinator = new FilterCombinator<MaxNodeWrapper>();
            combinator.Filters.Add(oldRoot);
            combinator.Filters.Add(filter);
            this.RootFilter = combinator;
            if (this.filterConfiguration != null)
               this.filterConfiguration.Filter = combinator;
         }
      }
   }

   private void deleteFilterButton_Click(object sender, EventArgs e)
   {
      this.filtersTree.BeginUpdate();

      IEnumerable<Tree.TreeNode> selNodes = this.filtersTree.SelectedNodes.ToList();
      foreach (Tree.TreeNode tn in selNodes)
      {
         Filter<MaxNodeWrapper> filter = tn.Tag as Filter<MaxNodeWrapper>;
         if (filter != null && tn.Parent != null)
         {
            FilterCombinator<MaxNodeWrapper> combinator = tn.Parent.Tag as FilterCombinator<MaxNodeWrapper>;
            if (combinator != null)
               combinator.Filters.Remove(filter);
         }
      }

      this.filtersTree.EndUpdate();
   }

   private void filtersTree_SelectionChanged(object sender, Tree.SelectionChangedEventArgs e)
   {
      this.filterPropertyGrid.SelectedObjects = e.Nodes.Select(tn => tn.Tag).ToArray();
   }

   private void filterChanged(object sender, EventArgs args)
   {
      if (this.UpdateAction != null)
         this.UpdateAction();

      Filter<MaxNodeWrapper> filter = sender as Filter<MaxNodeWrapper>;
      Tree.TreeNode tn;
      if (this.treeNodes.TryGetValue(filter, out tn))
         tn.Text = GetTreeNodeText(filter, tn);
   }

   private void filterAdded(object sender, FilterChangedEventArgs<MaxNodeWrapper> args)
   {
      FilterCollection<MaxNodeWrapper> collection = sender as FilterCollection<MaxNodeWrapper>;
      if (collection != null)
      {
         Tree.TreeNode parentTn;
         if (this.treeNodes.TryGetValue(collection.Owner, out parentTn))
         {
            this.AddFilterToTree(args.Filter, parentTn.Nodes);
            if (this.UpdateAction != null)
               this.UpdateAction();
         }
      }
   }

   private void filterRemoved(object sender, FilterChangedEventArgs<MaxNodeWrapper> args)
   {
      Tree.TreeNode tn;
      if (this.treeNodes.TryGetValue(args.Filter, out tn))
      {
         tn.Remove();
         this.treeNodes.Remove(args.Filter);
         if (this.UpdateAction != null)
            this.UpdateAction();
      }
   }
}
}
