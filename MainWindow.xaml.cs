using System.Windows;
using System.Windows.Input;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CurrencyConverter_API;
using System.Net.Http;
using System;
using Newtonsoft.Json;

namespace CurrencyConverter_Static
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Root val = new Root();

        public MainWindow()
        {
            InitializeComponent();

            //ClearControls method is used to clear all control values
            ClearControls();

            GetValue();

        }

        private async void GetValue()
        {
            val = await GetData<Root>("https://openexchangerates.org/api/latest.json?app_id=4ec321c31c284dcd8641b0fd34a0e165");
            BindCurrency();
        }

        public static async Task<Root> GetData<T>(string url)
        {
            var myRoot = new Root();
            try
            {
                using (var client = new HttpClient()) // HttpClient class provides a base class for sending/receiving the Http request
                {
                    client.Timeout = TimeSpan.FromMinutes(1); // The timespan to wait before request times out
                    HttpResponseMessage response = await client.GetAsync(url); // HttpResponseMessage is a way of returning a message
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) // Check API response status code ok
                    {
                        var ResponseString = await response.Content.ReadAsStringAsync(); //Serialize the HTTP content to a string
                        var ResponseObject = JsonConvert.DeserializeObject<Root>(ResponseString);

                        //MessageBox.Show("License: " + ResponseObject.license, "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                        return ResponseObject; // Return API response
                    }
                    return myRoot;
                }
            }
            catch
            {
                return myRoot;
            }
        }


        #region Bind Currency From and To Combobox
        private void BindCurrency()
        {
            DataTable dt = new DataTable();

            //Add display column in DataTable
            dt.Columns.Add("Text");

            //Add value column in DataTable
            dt.Columns.Add("Value");

            //Add rows in Datatable with text and value
            dt.Rows.Add("--SELECT--", 0);
            dt.Rows.Add("INR", val.rates.INR);
            dt.Rows.Add("USD", val.rates.USD);
            dt.Rows.Add("NZD", val.rates.NZD);
            dt.Rows.Add("JPY", val.rates.JPY);
            dt.Rows.Add("EUR", val.rates.EUR);
            dt.Rows.Add("CAD", val.rates.CAD);
            dt.Rows.Add("ISK", val.rates.ISK);
            dt.Rows.Add("PHP", val.rates.PHP);
            dt.Rows.Add("BAM", val.rates.BAM);
            dt.Rows.Add("HRK", val.rates.HRK);

            //The data to currency Combobox is assigned from datatable
            cmbFromCurrency.ItemsSource = dt.DefaultView;

            //DisplayMemberPath Property is used to display data in Combobox
            cmbFromCurrency.DisplayMemberPath = "Text";

            //SelectedValuePath property is used to set the value in Combobox
            cmbFromCurrency.SelectedValuePath = "Value";

            //SelectedIndex property is used to bind hint in the Combobox. The default value is Select.
            cmbFromCurrency.SelectedIndex = 0;

            //All properties are set for 'To Currency' Combobox as 'From Currency' Combobox
            cmbToCurrency.ItemsSource = dt.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Value";
            cmbToCurrency.SelectedIndex = 0;
        }
        #endregion

        #region Button Click Event

        //Convert button - click event
        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            //Create a variable as ConvertedValue with double data type to store currency converted value
            double ConvertedValue;

            //Check amount textbox is Null or Blank
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                //If amount textbox is Null or Blank it will show the below message box   
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //After clicking on message box OK sets the Focus on amount textbox
                txtCurrency.Focus();
                return;
            }
            //Else if the currency from is not selected or it is default text --SELECT--
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                //It will show the message
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on From Combobox
                cmbFromCurrency.Focus();
                return;
            }
            //Else if Currency To is not Selected or Select Default Text --SELECT--
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                //It will show the message
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                //Set focus on To Combobox
                cmbToCurrency.Focus();
                return;
            }

            //If From and To Combobox selected values are same
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                //The amount textbox value set in ConvertedValue.
                //double.parse is used to convert datatype String To Double.
                //Textbox text have string and ConvertedValue is double datatype
                ConvertedValue = double.Parse(txtCurrency.Text);

                //Show in label converted currency and converted currency name.
                // and ToString("N3") is used to place 000 after after the(.)
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {
                //Calculation for currency converter is From Currency value multiply(*) 
                // with amount textbox value and then the total is divided(/) with To Currency value
                ConvertedValue = (double.Parse(cmbToCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)) 
                    / double.Parse(cmbFromCurrency.SelectedValue.ToString());

                //Show in label converted currency and converted currency name.
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
        }

        //Clear button click event
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            //ClearControls method  is used to clear all control value
            ClearControls();
        }
        #endregion

        #region Extra Events

        //ClearControls used for clear all controls value
        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }

        //Allow only the integer value in TextBox
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //Regular Expression to add regex add library using System.Text.RegularExpressions;
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion
    }
}
