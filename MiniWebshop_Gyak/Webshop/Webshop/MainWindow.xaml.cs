using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using NetworkHelper;

namespace Webshop
{
    public partial class MainWindow : Window
    {
        List<Kategoria> kategoria = new List<Kategoria>();

        Kategoria kivalasztott;

        ComboBox combo; // ComboBox referencia

        public MainWindow()
        {
            InitializeComponent();

            

            DataGridFeltolt();

            KategoriakComboBetolt();
        }

        private void DataGridFeltolt()
        {
            string url = "http://localhost:3000/products";

            kategoria = Backend.GET(url)
                .Send()
                .As<List<Kategoria>>();

            Webshop.ItemsSource = kategoria;
        }

        private void KategoriakComboBetolt()
        {
            string url = "http://localhost:3000/categories";

            var kategoriakCombo = Backend.GET(url)
                .Send()
                .As<List<Kategoria>>();

            
            cmbKategoria.ItemsSource = kategoriakCombo;
            cmbKategoria.DisplayMemberPath = "Category";
            cmbKategoria.SelectedValuePath = "Category";

            
            cmbKategoria2.ItemsSource = kategoriakCombo;
            cmbKategoria2.DisplayMemberPath = "Category";
            cmbKategoria2.SelectedValuePath = "Category";
        }


        private void Frissites_Click(object sender, RoutedEventArgs e)
        {
            txtNev.Text = "";
            txtAr.Text = "";

            DataGridFeltolt();
        }

        private void Szures_Click(object sender, RoutedEventArgs e)
        {
            string url = "http://localhost:3000/products/filter?";

            if (cmbKategoria.SelectedValue != null)
            {
                url += "category=" + cmbKategoria.SelectedValue.ToString();
            }

            if (Raktaron.IsChecked == true)
            {
                url += "&inStock=true";
            }

            kategoria = Backend.GET(url)
                .Send()
                .As<List<Kategoria>>();

            Webshop.ItemsSource = kategoria;
        }






        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (cmbKategoria2.SelectedValue == null) return;

            var uj = new
            {
                Name = txtNev.Text,
                Price = double.Parse(txtAr.Text),
                Category = cmbKategoria2.SelectedValue.ToString(),
                inStock = Raktaron_2.IsChecked == true ? 1 : 0
            };

            Backend.POST("http://localhost:3000/products")
                .Body(uj)
                .Send();

            DataGridFeltolt();
        }


        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (kivalasztott == null) return;

            var modosit = new
            {
                Name = txtNev.Text,
                Price = double.Parse(txtAr.Text),
                Category = cmbKategoria2.SelectedValue.ToString(),
                inStock = Raktaron_2.IsChecked == true ? 1 : 0
            };

            Backend.PUT("http://localhost:3000/products/" + kivalasztott.Id)
                .Body(modosit)
                .Send();

            DataGridFeltolt();
        }


        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (kivalasztott == null) return;

            Backend.DELETE("http://localhost:3000/products/" + kivalasztott.Id)
                .Send();

            DataGridFeltolt();
        }

        private void Webshop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            kivalasztott = Webshop.SelectedItem as Kategoria;

            if (kivalasztott == null) return;

            txtNev.Text = kivalasztott.Name;

            txtAr.Text = kivalasztott.Price.ToString();

            combo.SelectedValue = kivalasztott.Category;

            Raktaron.IsChecked = kivalasztott.inStock == 1;

            
            if (kivalasztott.inStock == 1)
               Raktaron_2.IsChecked = true;
            else
                Raktaron_2.IsChecked = false;
            

        }
    }
}
