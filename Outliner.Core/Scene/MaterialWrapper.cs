﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Max;
using Outliner.MaxUtils;

namespace Outliner.Scene
{
   public class MaterialWrapper : MaxNodeWrapper
   {
      public IMtl Material { get; private set; }

      public MaterialWrapper(IMtl material)
      {
         Throw.IfArgumentIsNull(material, "material");

         this.Material = material;
      }

      public override object BaseObject
      {
         get { return this.Material; }
      }


      #region Equality

      public override Boolean Equals(Object obj)
      {
         MaterialWrapper wrapper = obj as MaterialWrapper;
         return wrapper != null && this.Material.Handle == wrapper.Material.Handle;
      }

      public override int GetHashCode()
      {
         return this.Material.GetHashCode();
      }

      #endregion


      #region Delete

      public virtual Boolean CanDelete
      {
         get { return true; }
      }

      public virtual void Delete() 
      {
         throw new NotImplementedException();
      }

      #endregion


      #region Childnodes

      public override int ChildNodeCount
      {
         get
         {
            return this.ChildBaseObjects.Count();
         }
      }

      public override IEnumerable<object> ChildBaseObjects
      {
         get
         {
            //Assigned nodes.
            IDependentIterator di = MaxInterfaces.Global.DependentIterator.Create(this.Material);
            IReferenceMaker refMaker = null;
            while ((refMaker = di.Next) != null)
            {
               if (refMaker is IINode)
                  yield return refMaker as IINode;
            }

            //Submaterials
            for (int i = 0; i < this.Material.NumSubMtls; i++)
            {
               yield return this.Material.GetSubMtl(i);
            }
         }
      }

      #endregion


      #region Node Type

      public virtual SClass_ID SuperClassID
      {
         get { return this.Material.SuperClassID; }
      }

      public virtual IClass_ID ClassID
      {
         get { return this.Material.ClassID; }
      }

      protected override MaxNodeType MaxNodeType
      {
         get { return MaxNodeType.Material; }
      }

      #endregion


      #region Name

      public virtual String Name
      {
         get { return this.Material.Name; }
         set { this.Material.Name = value; }
      }

      public virtual Boolean CanEditName
      {
         get { return true; }
      }

      #endregion


      #region Node Properties

      #endregion


      #region ImageKey

      public virtual String ImageKey
      {
         get { return "material"; }
      }

      #endregion


      public override String ToString()
      {
         return "MaterialWrapper (" + this.Name + ")";
      }
   }
}
