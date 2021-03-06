﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PJanssen.Outliner.NodeSorters;

namespace PJanssen.Outliner.Configuration
{
   /// <summary>
   /// Defines a NodeSorter property on an object.
   /// </summary>
   public interface ISorterConfiguration
   {
      /// <summary>
      /// Gets or sets the NodeSorter object.
      /// </summary>
      NodeSorter Sorter { get; set; }
   }
}
