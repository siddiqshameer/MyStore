using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Windows.Controls.Primitives;

namespace MyStore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["storedataConnectionString"].ConnectionString);
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StoreWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ListboxRefresh();
            TableRefresh();
            SalesListBox.SelectedIndex = 0;
        }

        void ListboxRefresh()
        {
            DataClass dc = new DataClass();
            DataTable dt = dc.StockLoad();
            SalesListBox.ItemsSource = dt.AsDataView();
            SalesListBox.DisplayMemberPath = "item_name";
        }

        void TableRefresh()
        {
            DataClass dc = new DataClass();
            DataTable dt = dc.StockLoad();
            StockTable.ItemsSource = dt.AsDataView();
        }

        private void btnStockInsert_Click(object sender, RoutedEventArgs e)
        {
            cn.Open();
            SqlCommand cm = new SqlCommand("insert into stock values('" + txtStockNum.Text + "','" + txtStockName.Text + "','" + txtStockPrice.Text + "','" + txtStockQty.Text + "')", cn);
            cm.ExecuteNonQuery();
            DataClass dc = new DataClass();
            DataTable dt = dc.StockLoad();
            StockTable.ItemsSource = dt.AsDataView();
            cn.Close();

            txtStockName.Text = txtStockPrice.Text = txtStockQty.Text = "";
            txtStockNum.Text =(int.Parse(txtStockNum.Text) + 1).ToString();
        }

        private void SalesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataRowView dr = SalesListBox.SelectedItem as DataRowView;
            if (dr!=null)
            {
                lblSalesName.Content = dr["item_name"].ToString();
                txtSalesPrice.Text = dr["price"].ToString();
            }
            else
            {
               
            }
            
        }

        private void txtSalesQuantity_TextChanged(object sender, TextChangedEventArgs ef)
        {
            if (txtSalesQuantity.Text != "")
            {
                double total = double.Parse(txtSalesPrice.Text) * int.Parse(txtSalesQuantity.Text);
                txtSalesAmount.Text = total.ToString();
            }
            else
            {
                //Do nothing
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            salesDataGrid.Items.Add(new { Name = lblSalesName.Content, Price = txtSalesPrice.Text, Quantity = txtSalesQuantity.Text, Amount = txtSalesAmount.Text });

            cn.Open();
            SqlCommand cmd = new SqlCommand("update stock set quantity=quantity-'" + txtSalesQuantity.Text + "' where item_name='" + lblSalesName.Content.ToString() + "'", cn);
            cmd.ExecuteNonQuery();
            cn.Close();

            txtSalesQuantity.Text = string.Empty;
            txtSalesAmount.Text = string.Empty;
            int Ind = SalesListBox.SelectedIndex;
            ListboxRefresh();
            TableRefresh();
            SalesListBox.Focus();
            SalesListBox.SelectedIndex = ++Ind;


            //try
            //{
            //    SalesListBox.SelectedIndex++;
            //}
            //catch (Exception)
            //{

            //    SalesListBox.SelectedIndex--;
            //}

        }

        private void btnGetTotal_Click(object sender, RoutedEventArgs e)
        {
            decimal sum = 0;


            for (int i = 0; i < salesDataGrid.Items.Count; ++i)
            {
                //(decimal.Parse((tblData.SelectedCells[3].Column.GetCellContent(item) as TextBlock).Text))
                sum += (decimal.Parse((salesDataGrid.Columns[3].GetCellContent(salesDataGrid.Items[i]) as TextBlock).Text));
            }

            lblSalesTotal.Content = sum.ToString();
        }

        private void SalesListBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key==Key.Enter)
            {
                txtSalesQuantity.Focus();
            }
        }

        private void txtSalesQuantity_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key==Key.Enter)
            {
                btnSalesAdd.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }
    }
}
