﻿using Outliner.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Outliner.Scene;
using System;
using Outliner.Controls.FiltersBase;

namespace Outliner_Unit_Tests
{
/// <summary>
///This is a test class for NameFilterTest and is intended
///to contain all NameFilterTest Unit Tests
///</summary>
[TestClass()]
public class NameFilterTest
{
   public class MockWrapper : IMaxNodeWrapper
   {
      public MockWrapper(String name)
      {
         this.Name = name;
      }

      public override object WrappedNode { get { return null; } }

      public override System.Collections.Generic.IEnumerable<IMaxNodeWrapper> ChildNodes
      {
         get { return null; }
      }

      public override string Name { get; set; }

      public override Autodesk.Max.SClass_ID SuperClassID { get { return Autodesk.Max.SClass_ID.Utility; } }
      public override Autodesk.Max.IClass_ID ClassID { get { return null; } }
   }

   /// <summary>
   ///A test for NameFilter Constructor
   ///</summary>
   [TestMethod()]
   public void NameFilterConstructorTest() 
   {
      NameFilter target = new NameFilter();
      Assert.AreEqual(String.Empty, target.SearchString);
      Assert.AreEqual(false, target.CaseSensitive);
   }

   /// <summary>
   ///A test for SearchString
   ///</summary>
   [TestMethod()]
   public void SearchStringTest() 
   {
      NameFilter target = new NameFilter();
      string expected = string.Empty;
      target.SearchString = expected;
      Assert.AreEqual(String.Empty, target.SearchString);

      target.SearchString = "t";
      Assert.AreEqual("t", target.SearchString);

      target.SearchString = "*test";
      Assert.AreEqual("*test", target.SearchString);

      target.SearchString = "";
      Assert.AreEqual(String.Empty, target.SearchString);
   }

   /// <summary>
   ///A test for CaseSensitive
   ///</summary>
   [TestMethod()]
   public void CaseSensitiveTest()
   {
      NameFilter target = new NameFilter();
      target.CaseSensitive = true;
      Assert.IsTrue(target.CaseSensitive);

      MockWrapper w = new MockWrapper("Test_sphere");
      target.SearchString = "Test_sphere";
      Assert.AreEqual(FilterResults.Show, target.ShowNode(w));
      target.SearchString = "test_sphere";
      Assert.AreEqual(FilterResults.Hide, target.ShowNode(w));

      target.CaseSensitive = false;
      Assert.IsFalse(target.CaseSensitive);

      target.SearchString = "Test_sphere";
      Assert.AreEqual(FilterResults.Show, target.ShowNode(w));
      target.SearchString = "test_sphere";
      Assert.AreEqual(FilterResults.Show, target.ShowNode(w));
   }
         
   /// <summary>
   ///A test for ShowNode
   ///</summary>
   [TestMethod()]
   public void ShowNodeTest()
   {
      NameFilter target = new NameFilter();
      MockWrapper w = new MockWrapper("test_sphere");
      Assert.AreEqual(FilterResults.Show, target.ShowNode(w));

      target.SearchString = "t";
      Assert.AreEqual(FilterResults.Show, target.ShowNode(w));

      target.SearchString = "*t";
      Assert.AreEqual(FilterResults.Show, target.ShowNode(w));

      target.SearchString = "*sphere";
      Assert.AreEqual(FilterResults.Show, target.ShowNode(w));

      target.SearchString = "a";
      Assert.AreEqual(FilterResults.Hide, target.ShowNode(w));

      target.SearchString = "*a";
      Assert.AreEqual(FilterResults.Hide, target.ShowNode(w));

      target.SearchString = "test_sphere_a";
      Assert.AreEqual(FilterResults.Hide, target.ShowNode(w));
   }

   [TestMethod()]
   public void UseWildcardTest()
   {
      NameFilter f = new NameFilter();
      MockWrapper w1 = new MockWrapper("test_a");
      MockWrapper w2 = new MockWrapper("a_test");

      f.SearchString = "test";
      f.UseWildcard = false;
      Assert.AreEqual(FilterResults.Show, f.ShowNode(w1));
      Assert.AreEqual(FilterResults.Hide, f.ShowNode(w2));

      f.UseWildcard = true;
      Assert.AreEqual(FilterResults.Show, f.ShowNode(w1));
      Assert.AreEqual(FilterResults.Show, f.ShowNode(w2));
   }
}
}