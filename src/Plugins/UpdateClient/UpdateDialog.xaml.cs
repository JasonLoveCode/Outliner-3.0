﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Outliner.UpdateClient
{
   /// <summary>
   /// Interaction logic for UpdateDialog.xaml
   /// </summary>
   public partial class UpdateDialog : Window
   {
      public UpdateDialog()
      {
         InitializeComponent();
      }

      private void RemindBtn_Click(object sender, RoutedEventArgs e)
      {
         this.Close();
      }
   }
}
