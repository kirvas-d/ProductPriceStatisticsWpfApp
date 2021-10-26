using ProductPriceStatistics.Core.DTOs;
using System;
using System.ComponentModel;

namespace ProductPriceStatisticsWpfApp.Models
{
    public class SelectableProduct : INotifyPropertyChanged
    {
        private readonly ProductDto _productDto;
        private bool _isSelected;

        public bool IsSelected 
        {
            get 
            {
                return _isSelected;
            }
            set 
            {
                _isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
            } 
        }
        public string Name => _productDto.Name;
        public Guid ProductId => _productDto.ProductId;

        public event PropertyChangedEventHandler PropertyChanged;

        public SelectableProduct(ProductDto productDto) 
        {
            _productDto = productDto;
        }
    }
}
