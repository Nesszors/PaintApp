﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace PaintApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Point currentPoint;
        private Point oldPoint;
        private Line line;
        private bool fill;
        private bool[,] seen;
        private Color fillColor;
        private WriteableBitmap bm;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.canvas1.MouseMove += new MouseEventHandler(canvas1_MouseMove);
            this.canvas1.MouseLeftButtonDown += new MouseButtonEventHandler(canvas1_MouseLeftButtonDown);
            Globals.scb = new SolidColorBrush(Colors.Black);
       
            fill = false;
        }

        void canvas1_MouseMove(object sender, MouseEventArgs e)
        {
            currentPoint = e.GetPosition(this.canvas1);
            line = new Line() { X1 = currentPoint.X, Y1 = currentPoint.Y, X2 = oldPoint.X, Y2 = oldPoint.Y };
            line.Stroke = Globals.scb;
            line.StrokeThickness = 8;
            line.StrokeStartLineCap = PenLineCap.Round;

            this.canvas1.Children.Add(line);
            oldPoint = currentPoint;
            fill = false;
        }

        void canvas1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            currentPoint = e.GetPosition(canvas1);
            oldPoint = currentPoint;         
        }
        void canvas1_Tap(object sender, GestureEventArgs e)
        {
            currentPoint = e.GetPosition(canvas1);

            bm = new WriteableBitmap(canvas1, null);
            Image image = new Image();
            Uri uri = new Uri("images/temp1.png", UriKind.Relative);
            ImageSource img = bm;
            image.SetValue(Image.SourceProperty, img);

            if (fill)
            {
                fillColor = bm.GetPixel((int)currentPoint.X, (int)currentPoint.Y);
                seen = new bool[bm.PixelWidth, bm.PixelHeight];
 //               flood(currentPoint);
            }

            bm.SetPixel((int)currentPoint.X, (int)currentPoint.Y, Globals.scb.Color); //remove this, was just testing if bm functions were working

            bm.Invalidate();
            this.canvas1.Children.Add(image);
        }
        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            this.canvas1.Children.Clear();
        }

        private void button2_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ColorPicker.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            button2.Background = Globals.scb;
        }
        private void fillButton_Click(object sender, RoutedEventArgs e)
        {
            fill = true;
        }
        private void flood(Point p)
        {
            int x;
            int y;
            bm.SetPixel((int)p.X, (int)p.Y, Globals.scb.Color);
            
            for (int i = 0; i < 8; i++)
            {
                x = (int)p.X + Globals.di[i];
                y = (int)p.Y + Globals.dj[i];
                if (x > 0 && x < bm.PixelWidth && y > 0 && y < bm.PixelHeight)
                    if (!seen[x, y])
                        if (fillColor == bm.GetPixel(x, y))
                        {
                            seen[x, y] = true;
                            flood(new Point(p.X + Globals.di[i], p.Y + Globals.dj[i]));
                        }
            }
        }
    }
}
