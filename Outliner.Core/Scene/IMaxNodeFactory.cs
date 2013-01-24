﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Outliner.Scene
{
   public interface IMaxNodeFactory
   {
      IMaxNode CreateMaxNode(Object baseNode);
   }
}
