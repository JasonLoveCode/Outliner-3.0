﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outliner.Scene;
using Autodesk.Max;
using Outliner.Plugins;

namespace Outliner.Filters
{
   [OutlinerPlugin(OutlinerPluginType.Filter)]
   [LocalizedDisplayName(typeof(Resources), "Filter_Nurbs")]
   [LocalizedDisplayImage(typeof(Resources), "nurbs")]
   [FilterCategory(FilterCategory.Classes)]
   public class NurbsFilter : Filter<IMaxNodeWrapper>
   {
      protected override Boolean ShowNodeInternal(IMaxNodeWrapper data)
      {
         IINodeWrapper iinodeWrapper = data as IINodeWrapper;
         if (iinodeWrapper == null)
            return false;

         return iinodeWrapper.SuperClassID == SClass_ID.Geomobject && iinodeWrapper.IINode.ObjectRef.IsShapeObject;
      }
   }
}
