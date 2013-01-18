﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outliner.Scene;
using Autodesk.Max;
using Outliner.MaxUtils;
using Autodesk.Max.Plugins;

namespace Outliner
{
internal static class GroupHelpers
{
   private static List<IINodeWrapper> openedGroupHeads;
   private static uint closeGroupHeadsCbKey = 0;
   private static GlobalDelegates.Delegate5 closeDelegate;

   private static void Start()
   {
      if (GroupHelpers.closeGroupHeadsCbKey == 0)
      {
         CloseGroupHeadsNodeEventCb cb = new CloseGroupHeadsNodeEventCb();
         IISceneEventManager sceneEventMgr = MaxInterfaces.Global.ISceneEventManager;
         GroupHelpers.closeGroupHeadsCbKey = sceneEventMgr.RegisterCallback(cb, false, 50, true);
      }

      if (GroupHelpers.closeDelegate == null)
      {
         GroupHelpers.closeDelegate = new GlobalDelegates.Delegate5((param, info) => MaxInterfaces.COREInterface.ClearNodeSelection(false));
         IGlobal global = MaxInterfaces.Global;
         global.RegisterNotification(closeDelegate, null, SystemNotificationCode.SystemPreNew);
         global.RegisterNotification(closeDelegate, null, SystemNotificationCode.SystemPreReset);
         global.RegisterNotification(closeDelegate, null, SystemNotificationCode.FilePreOpen);
         global.RegisterNotification(closeDelegate, null, SystemNotificationCode.FilePreSave);
      }
   }

   public static void Stop()
   {
      if (GroupHelpers.closeGroupHeadsCbKey != 0)
      {
         IISceneEventManager sceneEventMgr = MaxInterfaces.Global.ISceneEventManager;
         sceneEventMgr.UnRegisterCallback(GroupHelpers.closeGroupHeadsCbKey);
         GroupHelpers.closeGroupHeadsCbKey = 0;
      }

      if (GroupHelpers.closeDelegate != null)
      {
         IGlobal global = MaxInterfaces.Global;
         global.UnRegisterNotification(closeDelegate, null, SystemNotificationCode.SystemPreNew);
         global.UnRegisterNotification(closeDelegate, null, SystemNotificationCode.SystemPreReset);
         global.UnRegisterNotification(closeDelegate, null, SystemNotificationCode.FilePreOpen);
         global.UnRegisterNotification(closeDelegate, null, SystemNotificationCode.FilePreSave);
         GroupHelpers.closeDelegate = null;
      }

      if (GroupHelpers.openedGroupHeads != null)
         GroupHelpers.openedGroupHeads.Clear();
   }

   /// <summary>
   /// Opens any closed group heads in the provided list of nodewrappers.
   /// When the selection changes, the opened heads are closed automatically as required.
   /// </summary>
   public static void OpenSelectedGroupHeads(IEnumerable<MaxNodeWrapper> nodes)
   {
      Throw.IfArgumentIsNull(nodes, "nodes");

      if (GroupHelpers.openedGroupHeads == null)
         GroupHelpers.openedGroupHeads = new List<IINodeWrapper>();

      foreach (MaxNodeWrapper node in nodes)
      {
         IINodeWrapper inode = node as IINodeWrapper;
         if (inode == null)
            continue;

         if (inode.IINode.IsGroupMember && !inode.IINode.IsOpenGroupMember)
         {
            IINodeWrapper parent = inode.Parent as IINodeWrapper;
            while (parent != null && (parent.IINode.IsGroupMember || parent.IINode.IsGroupHead))
            {
               if (parent.IINode.IsGroupHead && !parent.IINode.IsOpenGroupHead)
               {
                  GroupHelpers.OpenCloseGroup(parent, true);
                  GroupHelpers.openedGroupHeads.Add(parent);
               }
               parent = parent.Parent as IINodeWrapper;
            }
            inode.IINode.SetGroupMemberOpen(true);
         }
      }

      GroupHelpers.Start();
   }

   /// <summary>
   /// Closes any group heads that were opened using OpenGroupHeads() and are no longer a parent of a selected node.
   /// </summary>
   public static Boolean CloseUnselectedGroupHeads()
   {
      if (GroupHelpers.openedGroupHeads == null)
         return false;

      Boolean groupsClosed = false;

      for (int i = GroupHelpers.openedGroupHeads.Count - 1; i >= 0; i--)
      {
         IINodeWrapper groupHead = GroupHelpers.openedGroupHeads[i];
         if (!HelperMethods.IsParentOfSelected(groupHead))
         {
            GroupHelpers.OpenCloseGroup(groupHead, false);
            GroupHelpers.openedGroupHeads.RemoveAt(i);
            groupsClosed = true;
         }
      }

      if (GroupHelpers.openedGroupHeads.Count == 0)
         GroupHelpers.Stop();

      return groupsClosed;
   }

   /// <summary>
   /// Opens or closes a group head.
   /// </summary>
   /// <param name="groupHead">The group head node to open or close.</param>
   public static void OpenCloseGroup(IINodeWrapper groupHead, Boolean open)
   {
      if (groupHead == null)
         return;

      if (groupHead.IINode.IsGroupHead)
         groupHead.IINode.SetGroupHeadOpen(open);

      foreach (MaxNodeWrapper child in groupHead.WrappedChildNodes)
      {
         IINodeWrapper inodeChild = child as IINodeWrapper;
         if (inodeChild != null && inodeChild.IINode.IsGroupMember)
         {
            inodeChild.IINode.SetGroupMemberOpen(open);
            OpenCloseGroup(inodeChild, open);
         }
      }
   }

   public static IINodeWrapper CreateGroupHead()
   {
      IInterface ip = MaxInterfaces.COREInterface;
      IGlobal global = MaxInterfaces.Global;
      IClass_ID classID = global.Class_ID.Create((uint)BuiltInClassIDA.DUMMY_CLASS_ID, 0);
      IDummyObject dummy = ip.CreateInstance(SClass_ID.Helper, classID) as IDummyObject;
      dummy.Box = global.Box3.Create( global.Point3.Create(0, 0, 0)
                                    , global.Point3.Create(0, 0, 0));
      IINode groupHead = ip.CreateObjectNode(dummy);
      String newName = "group";
      MaxInterfaces.COREInterface.MakeNameUnique(ref newName);
      groupHead.Name = newName;
      groupHead.SetGroupHead(true);

      return new IINodeWrapper(groupHead);
   }

   public static void AddNodesToGroup(IEnumerable<MaxNodeWrapper> nodes, IINodeWrapper groupHead)
   {
      foreach (MaxNodeWrapper node in nodes.Where(n => n is IINodeWrapper))
      {
         node.Parent = groupHead;
         ((IINode)node.WrappedNode).SetGroupMember(true);
      }
   }

   private class CloseGroupHeadsNodeEventCb : INodeEventCallback
   {
      public CloseGroupHeadsNodeEventCb() { }

      public override void SelectionChanged(ITab<UIntPtr> nodes)
      {
         if (GroupHelpers.CloseUnselectedGroupHeads())
         {
            IInterface core = MaxInterfaces.COREInterface;
            core.RedrawViews(core.Time, RedrawFlags.Normal, null);
         }
      }
   }
}
}
