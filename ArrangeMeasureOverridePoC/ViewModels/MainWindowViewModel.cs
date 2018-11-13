using ArrangeMeasureOverridePoC.Models;
using System.Collections.ObjectModel;

namespace ArrangeMeasureOverridePoC.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Properties
        private ObservableCollection<Customer> _customers = new ObservableCollection<Customer>();
        public ObservableCollection<Customer> Customers
        {
            get
            {
                return _customers;
            }
            set
            {
                _customers = value;
                OnPropertyChanged(nameof(Customers));
            }
        }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            SetupCustomers();
        }
        #endregion

        #region Private Methods
        private void SetupCustomers()
        {
            Customers.Add(new Customer() { Name = "Paul", Surname = "Kowalsky", Age = "27", IsShown = false });
            Customers.Add(new Customer() { Name = "Jack", Surname = "Muller", Age = "67", IsShown = false });
            Customers.Add(new Customer() { Name = "Margaret", Surname = "Muller", Age = "60", IsShown = false });
            Customers.Add(new Customer() { Name = "Luna", Surname = "Braveheart", Age = "6", IsShown = false });
            Customers.Add(new Customer() { Name = "Milka", Surname = "Rosevelda", Age = "8", IsShown = false });
        }
        #endregion
    }
}
