﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PJanssen.Outliner.WebServices
{
   public class UpdateData
   {
      public bool IsUpdateAvailable
      {
         get;
         set;
      }

      public OutlinerVersion NewVersion
      {
         get;
         set;
      }

      public string DownloadUrl
      {
         get;
         set;
      }

      public string Signature
      {
         get;
         set;
      }

      public string ReleaseNotesUrl
      {
         get;
         set;
      }
   }
}
