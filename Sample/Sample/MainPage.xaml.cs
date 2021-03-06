﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sample
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            BindingContext = this;
            InitializeComponent();
        }

        public int SelectedTabIndex { get; set; } = 1;

        public string TabTitle1 { get; set; } = "Photos";
        public string TabTitle2 { get; set; } = "My team";
        public string TabTitle3 { get; set; } = "Calendar";

        public bool IsShowTab1 { get; set; } = true;
        public bool IsShowTab2 { get; set; } = true;
        public bool IsShowTab3 { get; set; } = true;


        private void Button_Clicked(object sender, EventArgs e)
        {
            IsShowTab1 = !IsShowTab1;
        }

        private void Button_Clicked1(object sender, EventArgs e)
        {
            IsShowTab2 = !IsShowTab2;
        }

        private void Button_Clicked2(object sender, EventArgs e)
        {
            IsShowTab3 = !IsShowTab3;
        }
    }
}
