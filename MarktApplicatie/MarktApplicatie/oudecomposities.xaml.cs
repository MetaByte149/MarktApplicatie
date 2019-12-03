﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace MarktApplicatie
{
    public partial class OudeComposities : Window
    {

        string[] composition_files = SoufTools.GetAllCompositions();

        public OudeComposities()
        {
            InitializeComponent();

            if(composition_files.Length < 1) {
                MessageBox.Show("Je moet eerst een compositie opslaan!");
            }

            // for every file get the filename and add it to 
            foreach (string composition_filepath in composition_files) {
                string[] path_parts = composition_filepath.Split('\\');
                string filename = path_parts[path_parts.Length - 1];
                AddListViewItem(filename);
            }



        }

        private void AddListViewItem(string filename) {
            TextBlock b = new TextBlock() {
                Text = filename.Split('.')[0],
                FontSize = 24
            };

            

            listView.Items.Add(b);

        }

        

        private void ListView_onclick(object sender, MouseButtonEventArgs e) {
            string selected_file_name = composition_files[listView.SelectedIndex];

            string plankinfo = File.ReadAllText(Path.Combine(@"..\..\json\", selected_file_name));
            PlankInfo[] result = JsonConvert.DeserializeObject<PlankInfo[]>(plankinfo);

            EditKraam editkraam = new EditKraam();

            if (result != null) {
                foreach (PlankInfo p in result) {
                    editkraam.Add_plank(p.X, p.Y, p.Width, p.Height);
                }
            }

            editkraam.Show();
            Close();






        }

        private void onClick_homepage(object sender, MouseButtonEventArgs e) {
            MainWindow home = new MainWindow();
            home.Show();
            this.Close();
        }



        public void btnLoadPlanks_Click(int valuetab)
        {
            if (valuetab == 1)
            {
                string plankinfo = File.ReadAllText(@"..\..\json\plankinfo.json");
                PlankInfo[] result = JsonConvert.DeserializeObject<PlankInfo[]>(plankinfo);
                Console.WriteLine(result);

                EditKraam ek = new EditKraam();

                if (result != null)
                {
                    foreach (PlankInfo p in result)
                    {
                        ek.Add_plank(p.X, p.Y, p.Width, p.Height);
                    }
                }

                ek.Show();
                Close();
            }
            else
            {
                string plankinfo = File.ReadAllText(@"..\..\json\plankinfo" + valuetab + ".json");
                PlankInfo[] result = JsonConvert.DeserializeObject<PlankInfo[]>(plankinfo);
                Console.WriteLine(result);

                EditKraam ek = new EditKraam();

                if (result != null)
                {
                    foreach (PlankInfo p in result)
                    {
                        ek.Add_plank(p.X, p.Y, p.Width, p.Height);
                    }
                }
                ek.Show();
                Close();
            }
        }


        public void btnEditTitle_Click(object sender, RoutedEventArgs e) {

        }
    }
}