﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MarktApplicatie {
    public class Plank {

        public int GRID_SIZE = SoufTools.GRID_SIZE;
        public static Canvas c;

        // location where the plank has been clicked lastly on the plank
        public double onClick_X;
        public double onClick_Y;

        public int id;
        public Rectangle r;
        public static int selectedPlank = -1;
        public List<Fruit> fruits = new List<Fruit>();

        public int cols, rows;

        public void StandardConstructer() {
            id = -1;
            r = new Rectangle();
            cols = 0;
            rows = 0;
        }

        public Plank(int id_, Rectangle r_) {
            StandardConstructer();
            id = id_;
            r = r_;

            cols = (int)r.Width / GRID_SIZE;
            rows = (int)r.Height / GRID_SIZE;

            // initialize with null fruits
            for (int i = 0; i < cols * rows; i++) {
                fruits.Add(new Fruit(i, -1, this));
            }


        }

        public Plank() {
            StandardConstructer();
        }

        public void Check() {
            if (selectedPlank == id) {
                r.Stroke = SoufTools.GetColor("#00FF00");
            }
            else {
                r.Stroke = SoufTools.GetColor("#000000");
            }
        }

        public void Resize(int width, int height) {

            r.Width = width;
            r.Height = height;

            cols = (int)r.Width / GRID_SIZE;
            rows = (int)r.Height / GRID_SIZE;

            List<Fruit> new_fruits = new List<Fruit>();

            // remove all fruit rectangles from canvas
            foreach(Fruit f in fruits) {
                c.Children.Remove(f.r);
            }


            // initialize with null fruits
            for (int i = 0; i < cols * rows; i++) {
                new_fruits.Add(new Fruit(i, -1, this));
            }

            fruits = new_fruits;
        }

        // onclick om fruit te veranderen
        public void OnClick(int selectedFruit, double x, double y) {

            int col = Math.Abs((int)Math.Floor(x / GRID_SIZE));
            int row = Math.Abs((int)Math.Floor(y / GRID_SIZE));

            

            // index of pressed fruit in list
            int i = (row * cols) + col;

            fruits[i].Change(selectedFruit);
            
        }

        // general onclick (to remember where it has been clicked to it can move it smoothly)
        public void OnClick(double x, double y) {

            onClick_X = x - Canvas.GetLeft(r);
            onClick_Y = y - Canvas.GetTop(r);

        }



        public Rectangle[] GetAllFruitRect() {

            Rectangle[] rects = new Rectangle[fruits.Count];

            for(int i = 0; i < fruits.Count; i++) {
                rects[i] = fruits[i].r;
            }

            return rects;
        }

        public void Move(double x, double y) {



            x -= onClick_X;
            y -= onClick_Y;


            Canvas.SetLeft(r, x);
            Canvas.SetTop(r, y);

            foreach(Fruit f in fruits) {
                f.Move(x, y);
            }
        }

        public void ReloadFruits()
        {
            Fruit.fruit_info = SoufTools.GetAllFruits();
        }

    }

    public class Fruit{
        public Rectangle r;
        public int id;
        public int GRID_SIZE = SoufTools.GRID_SIZE;

        public Plank plank;

        // list of every food that holds list with 2 values, name and color
        public static string[][] fruit_info = SoufTools.GetAllFruits();


        public static Canvas c;
        public static SolidColorBrush standard_color = SoufTools.GetColor("#000000");

        public int x_on_plank;
        public int y_on_plank;
        public int currentFruit;

        public Fruit(int id_, int fruitId, Plank plank_) {
            id = id_;
            

            // parent
            plank = plank_;
            

            // creating the rectangle
            r = new Rectangle {
                Width = GRID_SIZE - GRID_SIZE / 5,
                Height = GRID_SIZE - GRID_SIZE / 5
            };

            //TEMP
            r.Stroke = SoufTools.GetColor("#FF0000");

            double plank_x = Canvas.GetLeft(plank.r);
            double plank_y = Canvas.GetTop( plank.r);

            // get coordinates inside plank
            x_on_plank = id % plank.cols * GRID_SIZE;
            y_on_plank = id / plank.cols * GRID_SIZE;

            Move(plank_x, plank_y);



            // als hij geen null fruit is dan moet je kleur instellen
            if (fruitId != -1) {
                Change(fruitId);
            }
            else {
                r.Fill = standard_color;
            }

            c.Children.Add(r);
            
            
        }

        public void ReloadFoods()
        {
            fruit_info = SoufTools.GetAllFruits();
        }

        public void Change(int selectedFruit) {
            currentFruit = selectedFruit;
            if (selectedFruit < 0) {
                r.Fill = standard_color;
                return;
            }

            string hex = fruit_info[selectedFruit][1];
            r.Fill = SoufTools.GetColor(hex);

        }

        internal void Move(double x, double y) {
            Canvas.SetLeft(r, x_on_plank + x + GRID_SIZE / 10);
            Canvas.SetTop( r, y_on_plank + y + GRID_SIZE / 10);
        }
    }
}
