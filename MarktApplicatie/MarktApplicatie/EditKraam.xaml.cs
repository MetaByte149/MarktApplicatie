﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace MarktApplicatie
{
    
    public partial class EditKraam : Window
    {


        bool mouseDown = false;
        bool plankEditMode = true;

        List<string> fruitNames = new List<string>();

        
        

        int GRID_SIZE = SoufTools.GRID_SIZE;

        Plank selectedPlank = new Plank();

        List<Plank> planks = new List<Plank>();

        SolidColorBrush currentFill = new SolidColorBrush(Colors.Red);
        int selectedFruit = -1;

        

        

        

        private void SetColor(string code)
        {
            currentFill = SoufTools.GetColor(code);
        }

        private Rectangle Rect(double x, double y, double w, double h)
        {


            Rectangle r = new Rectangle
            {
                Width = w,
                Height = h,
                Fill = currentFill
            };
            
            Canvas.SetLeft(r, x);
            Canvas.SetTop(r, y);

            canvas.Children.Add(r);
            return r;
        }


        public void Add_plank(double x, double y, double width, double height)
        {
            SetColor("#654321");

            Plank p = new Plank(
                planks.Count,
                Rect(x, y, width * GRID_SIZE, height * GRID_SIZE)
                );

            planks.Add(p);
            selectedPlank = p;
            Plank.selectedPlank = planks.Count() - 1;

            CheckPlanks();
        }

        // window initializer
        public EditKraam()
        {
            // initialize window
            InitializeComponent();
            
            if (Settings1.Default.Darkmode)
            {
                listView.Background = new SolidColorBrush(Colors.Black);
                canvas.Background = new SolidColorBrush(Colors.Black);
                
            }

            else 
            {
                listView.Background = new SolidColorBrush(Colors.White);
                canvas.Background = new SolidColorBrush(Colors.White);


            }

            Fruit.c = canvas;
            Plank.c = canvas;

            ReloadFruits();

            if (Settings1.Default.Font8 == true)
            {
                FontSize = 8;
            }

            if (Settings1.Default.Font10 == true)
            {
                FontSize = 10;
            }

            if (Settings1.Default.Font12 == true)
            {
                FontSize = 12;
            }

            if (Settings1.Default.Font14 == true)
            {
                FontSize = 14;
            }

            if (Settings1.Default.Font16 == true)
            {
                FontSize = 16;
            }




        }

        public void CreateFruit(String name, String hex_color)
        {
            TextBlock txt = new TextBlock
            {
                Text = name,
                Background = SoufTools.GetColor(hex_color)
            };

            listView.Items.Add(txt);
            fruitNames.Add(name);
        }

        private void go_home(object sender, MouseButtonEventArgs e)
        {
            MainWindow mainwindow = new MainWindow();
            mainwindow.Show();
            this.Close();
        }

        

        private void ListView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            selectedFruit = listView.SelectedIndex;
            debugText.Text = selectedFruit.ToString();
        }

        private void Create_plank(object sender, MouseButtonEventArgs e)
        {
            plank_popup dialog = new plank_popup();
            int width, height;
            if(dialog.ShowDialog() == true)
            {
                debugText.Text = planks.Count.ToString();
                width = Convert.ToInt16(dialog.Inp_width);
                height = Convert.ToInt16(dialog.Inp_height);



                SetColor("#654321");

                Plank p = new Plank(
                    planks.Count,
                    Rect(0, 0, width * GRID_SIZE, height * GRID_SIZE)
                    );

                planks.Add(p);
                selectedPlank = p;
                Plank.selectedPlank = planks.Count() - 1;

                CheckPlanks();

            }
            
        }

        public void save_Composition(object sender, MouseButtonEventArgs e)
        {
            save_popup popup = new save_popup();
            if (popup.ShowDialog() == true) {

                string path_filename = Path.Combine(SoufTools.compositions_path, popup.FileName);


                // Creating the JSON file
                PlankInfo[] plankinfos = new PlankInfo[planks.Count];
                for (int i = 0; i < planks.Count; i++)
                {
                    Rectangle r = planks[i].r;
                    var width = r.Width / SoufTools.GRID_SIZE;
                    var height = r.Height / SoufTools.GRID_SIZE;
                    double plank_x = Canvas.GetLeft(r);
                    double plank_y = Canvas.GetTop(r);
                    plankinfos[i] = new PlankInfo()
                    {
                        Width = width,
                        Height = height,
                        X = plank_x,
                        Y = plank_y
                    };
                }
                string strResultJson = JsonConvert.SerializeObject(plankinfos);
                SoufTools.CreateFile(path_filename + ".json", strResultJson);


                // Creating an Image
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(canvas);
                using (FileStream stream = new FileStream(path_filename + ".png", FileMode.Create))
                {
                    PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                    encoder5.Frames.Add(BitmapFrame.Create(rtb));
                    encoder5.Save(stream);
                }

            }


            

            
        }

        private void Canvas_onclick(object sender, MouseButtonEventArgs e) {

            Point p = e.GetPosition(this);
            mouseDown = true;
            positionText.Text = $"X: {p.X.ToString()}, \nY: {p.Y.ToString()}";



            if (plankEditMode) {
                for (int i = 0; i < planks.Count; i++) {
                    Rectangle r = planks[i].r;
                    double x = Canvas.GetLeft(r);
                    double y = Canvas.GetTop(r);


                    if (p.X > x && p.X < x + r.Width &&
                        p.Y > y && p.Y < y + r.Height) {
                        Plank.selectedPlank = i;
                        selectedPlank = planks[i];
                        selectedPlank.OnClick(p.X, p.Y);
                        CheckPlanks();
                        return;
                    }
                }

                // als niks gedrukt is dan moet hij deselecteren
                selectedPlank = new Plank();
                Plank.selectedPlank = -1;
                CheckPlanks();
            }
            else {

                foreach(Plank plank in planks) {
                    Rectangle r = plank.r;
                    double plank_x = Canvas.GetLeft(r);
                    double plank_y = Canvas.GetTop(r);

                    if (p.X > plank_x && p.X < plank_x + r.Width &&
                        p.Y > plank_y && p.Y < plank_y + r.Height) {

                        double x = p.X - Canvas.GetLeft(plank.r);
                        double y = p.Y - Canvas.GetTop(plank.r);
                        plank.OnClick(selectedFruit, x, y);
                    }
                    
                }
            }
            

        }

        void CheckPlanks() {
            // Make it get green stroke, if its currently selected.
            for(int i = 0; i < planks.Count; i++) {
                planks[i].Check();
             }
            
            
            

        }

        private void Canvas_onrelease(object sender, MouseButtonEventArgs e) {
            mouseDown = false;


        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e) {
            Point p = e.GetPosition(this);
            // if the mouse is held down while moving
            if (mouseDown) {
                positionText.Text = $"X: {p.X.ToString()}, \nY: {p.Y.ToString()}";

                // if you want to move the current selected plank
                if (plankEditMode) {
                    if (Plank.selectedPlank == -1)
                        return;

                    selectedPlank.Move(p.X, p.Y);
                }


             }
        }

        private void Switch_editmode_onclick(object sender, MouseButtonEventArgs e) {

            if (plankEditMode) {
                plankEditMode = false;
                btn_switch_editmode.Content = "Switch to\nplank edit";
                
            }
            else {
                plankEditMode = true;
                btn_switch_editmode.Content = "Switch to\nadd fruit";
                
            }
        }

        private void Verander_grootte(object sender, MouseButtonEventArgs e) {
            if (Plank.selectedPlank == -1) { 
                MessageBox.Show("Selecteer een plank");
                return;
            }

            plank_popup dialog = new plank_popup();
            int width, height;
            if (dialog.ShowDialog() == true) {
                debugText.Text = planks.Count.ToString();
                width = Convert.ToInt16(dialog.Inp_width);
                height = Convert.ToInt16(dialog.Inp_height);

                selectedPlank.Resize(width * GRID_SIZE, height * GRID_SIZE);
            }

        }

        public void ReloadFruits()
        {
            string[][] fruit_info = SoufTools.GetAllFruits();

            ReloadFruitsMainScreen(fruit_info);

            Fruit.fruit_info = fruit_info;


        }

        public void ReloadFruitsMainScreen(string[][] fruit_info)
        {
            List<TextBlock> selected_blocks = new List<TextBlock>();

            foreach(TextBlock block in listView.Items)
            {
                selected_blocks.Add(block);
            }

            // Delete all previous fruits
            foreach (TextBlock block in selected_blocks)
            {
                listView.Items.Remove(block);
            }

            foreach(string[] d in fruit_info)
            {
                CreateFruit(d[0], d[1]);
            }
            


        }
        private void Go_to_custom_fruits(object sender, MouseButtonEventArgs e)
        {
            custom_fruits dialog = new custom_fruits();
            if (dialog.ShowDialog() == true)
            {
                ReloadFruits();
            }


        }

        private void Canvas_right_click(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this);
            mouseDown = true;

            if (plankEditMode)
            {

            }
            else
            {
                foreach (Plank plank in planks)
                {
                    Rectangle r = plank.r;
                    double plank_x = Canvas.GetLeft(r);
                    double plank_y = Canvas.GetTop(r);

                    if (p.X > plank_x && p.X < plank_x + r.Width &&
                        p.Y > plank_y && p.Y < plank_y + r.Height)
                    {
                        double x = p.X - Canvas.GetLeft(plank.r);
                        double y = p.Y - Canvas.GetTop(plank.r);
                        plank.OnClick(-1, x, y);
                    }
                }
            }

            

        }

        private void reset(object sender, MouseButtonEventArgs e)
        {
            EditKraam editkraam = new EditKraam();
            editkraam.Show();
            this.Close();
        }
    }
}
