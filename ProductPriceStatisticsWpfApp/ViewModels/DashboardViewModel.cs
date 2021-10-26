using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using ProductPriceStatistics.Core.DTOs;
using ProductPriceStatisticsWpfApp.Models;
using ProductPriceStatisticsWpfApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace ProductPriceStatisticsWpfApp.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly IProductPriceStatisticsApiService _apiService;
        private IEnumerable<SelectableProduct> _products;
        private Dictionary<Guid, LineSeries> _lineSeriesDictionary;

        public IEnumerable<SelectableProduct> Products
        {
            get
            {
                return _products;
            }
            set
            {
                _products = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Products)));
            }
        }
        public SeriesCollection Series { get; set; }
        public Func<double, string> Formatter { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public DashboardViewModel()
        {
            _apiService = new ProductPriceStatisticsApiService("https://localhost:5001");

            _apiService.GetProducts().ContinueWith((result) =>
            {
                List<SelectableProduct> selectableProducts = new List<SelectableProduct>();

                foreach (var productDto in result.Result)
                {
                    SelectableProduct selectableProduct = new SelectableProduct(productDto);
                    selectableProduct.PropertyChanged += SelectableProduct_PropertyChanged;
                    selectableProducts.Add(selectableProduct);
                }

                Products = new ObservableCollection<SelectableProduct>(selectableProducts);
            });

            _lineSeriesDictionary = new Dictionary<Guid, LineSeries>();

            var dayConfig = Mappers.Xy<PriceDto>()
                .X(price => (double)price.DateTimeStamp.Ticks / TimeSpan.FromHours(1).Ticks)
                .Y(price => (double)price.Value);

            Series = new SeriesCollection(dayConfig);         
            Formatter = value => value >= 0 ? new System.DateTime((long)(value * TimeSpan.FromHours(1).Ticks)).ToString("dd.MM hh:mm") : "0";
        }

        private async void SelectableProduct_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SelectableProduct product = sender as SelectableProduct;

            if (product.IsSelected)
            {
                if (!_lineSeriesDictionary.ContainsKey(product.ProductId))
                {
                    var result = await _apiService.GetPricesOfProduct(product.ProductId, null, null);          

                    _lineSeriesDictionary.Add(product.ProductId, CreateLineSeries(product.Name, result));
                }        
                               
                Series.Add(_lineSeriesDictionary[product.ProductId]);
            }
            else 
            {
                Series.Remove(_lineSeriesDictionary[product.ProductId]);
            }
        }

        private LineSeries CreateLineSeries(string title, IEnumerable<PriceDto> priceDtos)
        {
            return new LineSeries()
            {
                Values = new ChartValues<PriceDto>(priceDtos),
                Title = title,
                Fill = Brushes.Transparent,
            };
        }
    }
}
