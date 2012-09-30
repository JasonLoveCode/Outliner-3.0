﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;
using System.Security.Permissions;

namespace Outliner.Controls
{
   public static class BitmapProcessing
   {
      /// <summary>
      /// Inverts the pixel data of the bitmap.
      /// </summary>
      [SecurityPermission(SecurityAction.Demand)]
      public static void Invert(Bitmap b)
      {
         BitmapData bmData = b.LockBits( new Rectangle(0, 0, b.Width, b.Height)
                                       , ImageLockMode.ReadWrite
                                       , PixelFormat.Format32bppArgb);
         int stride = bmData.Stride;
         System.IntPtr Scan0 = bmData.Scan0;
         unsafe
         {
            byte* p = (byte*)(void*)Scan0;

            int nOffset = stride - b.Width * 4;

            for (int y = 0; y < b.Height; ++y)
            {
               for (int x = 0; x < b.Width; ++x)
               {
                  p[0] = (byte)(255 - p[0]);
                  p[1] = (byte)(255 - p[1]);
                  p[2] = (byte)(255 - p[2]);

                  p += 4;
               }
               p += nOffset;
            }
         }

         b.UnlockBits(bmData);
      }


      /// <summary>
      /// Desaturates the pixel data of the bitmap.
      /// </summary>
      [SecurityPermission(SecurityAction.Demand)]
      public static void Desaturate(Bitmap b)
      {
         BitmapData bmData = b.LockBits( new Rectangle(0, 0, b.Width, b.Height)
                                       , ImageLockMode.ReadWrite
                                       , PixelFormat.Format32bppArgb);
         int stride = bmData.Stride;
         System.IntPtr Scan0 = bmData.Scan0;
         unsafe
         {
            byte* p = (byte*)(void*)Scan0;

            int nOffset = stride - b.Width * 4;

            double px;

            for (int y = 0; y < b.Height; ++y)
            {
               for (int x = 0; x < b.Width; ++x)
               {
                  px = .299 * p[2] + .587 * p[1] + .114 * p[0];

                  p[0] = p[1] = p[2] = (byte)px;

                  p += 4;
               }
               p += nOffset;
            }
         }

         b.UnlockBits(bmData);
      }


      /// <summary>
      /// Sets the opacity on the pixel data of the bitmap.
      /// </summary>
      /// <param name="b">The bitmap to process.</param>
      /// <param name="opacity">The opacity to set, range between 0 and 255.</param>
      [SecurityPermission(SecurityAction.Demand)]
      public static void Opacity(Bitmap b, Int32 opacity)
      {
         BitmapData bmData = b.LockBits( new Rectangle(0, 0, b.Width, b.Height)
                                       , ImageLockMode.ReadWrite
                                       , PixelFormat.Format32bppArgb);
         int stride = bmData.Stride;
         System.IntPtr Scan0 = bmData.Scan0;
         unsafe
         {
            byte* p = (byte*)(void*)Scan0;

            int nOffset = stride - b.Width * 4;
            float opacity_factor = opacity / 255f;

            for (int y = 0; y < b.Height; ++y)
            {
               for (int x = 0; x < b.Width; ++x)
               {
                  p[3] = (byte)(opacity_factor * p[3]);
                  p += 4;
               }
               p += nOffset;
            }
         }

         b.UnlockBits(bmData);
      }

      /// <summary>
      /// Increases the brightness of the pixel data in the bitmap.
      /// </summary>
      [SecurityPermission(SecurityAction.Demand)]
      public static void Brightness(Bitmap b, Int32 brightness)
      {
         BitmapData bmData = b.LockBits( new Rectangle(0, 0, b.Width, b.Height)
                                       , ImageLockMode.ReadWrite
                                       , PixelFormat.Format32bppArgb);
         int stride = bmData.Stride;
         System.IntPtr Scan0 = bmData.Scan0;
         unsafe
         {
            byte* p = (byte*)(void*)Scan0;

            int nOffset = stride - b.Width * 4;
            int px;

            for (int y = 0; y < b.Height; ++y)
            {
               for (int x = 0; x < b.Width; ++x)
               {
                  px = brightness + p[0];
                  p[0] = (px > 255) ? (byte)255 : (byte)px;

                  px = brightness + p[1];
                  p[1] = (px > 255) ? (byte)255 : (byte)px;

                  px = brightness + p[2];
                  p[2] = (px > 255) ? (byte)255 : (byte)px;

                  p += 4;
               }
               p += nOffset;
            }
         }

         b.UnlockBits(bmData);
      }
   }
}
