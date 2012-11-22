﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Max.Plugins;
using Autodesk.Max;
using Outliner.MaxUtils;
using Outliner.Scene;

namespace Outliner.Commands
{
   public abstract class Command : RestoreObj
   {
      protected abstract void Do();
      protected abstract void Undo();

      public override void Redo()
      {
         this.Do();
      }

      public override void Restore(bool isUndo)
      {
         this.Undo();
      }

      /// <summary>
      /// Registers a command in the hold and executes it by running its Redo method.
      /// </summary>
      public virtual void Execute(Boolean redrawViews)
      {
         IHold theHold = MaxInterfaces.Global.TheHold;
         if (!theHold.Holding)
            theHold.Begin();

         this.Do();

         theHold.Put(this);
         theHold.Accept(this.Description);

         if (redrawViews)
         {
            IInterface ip = MaxInterfaces.Global.COREInterface;
            ip.RedrawViews(ip.Time, RedrawFlags.Normal, null);
         }
      }
   }
}