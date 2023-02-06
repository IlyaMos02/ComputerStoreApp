using NetTechLab3.Client;
using NetTechLab3.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace NetTechLab3.View
{
    public partial class MainView : Form
    {
        private BindingSource ProductBindingSource;
        private List<Product> ProductList;

        public MainView()
        {
            InitializeComponent();
            tabControl1.TabPages.Remove(tabEdit);           

            ProductBindingSource = new BindingSource();           

            dataGridView1.DataSource = ProductBindingSource;                       
        }       

        private void LoadAllProducts()
        {
            string response = SocketClient.Send("getProd");
            ProductList = JsonConvert.DeserializeObject<List<Product>>(response);
            ProductBindingSource.DataSource = ProductList;
        }

        public string IdProduct
        {
            get { return txtId.Text; }
            set { txtId.Text = value; }
        }
        public string Name
        {
            get { return txtName.Text; }
            set { txtName.Text = value; }
        }
        public string Description
        {
            get { return txtDesc.Text; }
            set { txtDesc.Text = value; }
        }
        public string Price
        {
            get { return txtPrice.Text; }
            set { txtPrice.Text = value; }
        }
        public string Status
        {
            get { return comboStatus.Text; }
            set { comboStatus.Text = value; }           
        }
        public string isSuccessful { get; set; }       
        public bool isEdit { get; set; }

        private void CleanViewFields()
        {
            IdProduct = "0";
            Name = "";
            Description = "";
            Price = "0";
            Status = "";
        }                                      

        private bool IsLocked(string response)
        {
            if (!string.Equals(response, "locked"))
                return false;
            else
                return true;
        }

        private void MainView_Load(object sender, EventArgs e)
        {
            try
            {
                LoadAllProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string ProductList = string.Empty;
            bool emptyValue = string.IsNullOrWhiteSpace(txtSearch.Text);
            if (emptyValue == false)               
                ProductList = SocketClient.Send("search:" + txtSearch.Text);
            else
                ProductList = SocketClient.Send("getProd");

            if (!IsLocked(ProductList))
                ProductBindingSource.DataSource = JsonConvert.DeserializeObject<List<Product>>(ProductList);
            else
                MessageBox.Show("Locked");
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            isEdit = false;
            tabControl1.TabPages.Remove(tabProducts);
            tabControl1.TabPages.Add(tabEdit);
            tabEdit.Text = "Add new Product";
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var product = (Product)ProductBindingSource.Current;
            IdProduct = product.IdProduct.ToString();
            Name = product.Name;
            Description = product.Description;
            Price = product.Price.ToString();
            Status = product.Status;            
            isEdit = true;

            tabControl1.TabPages.Remove(tabProducts);
            tabControl1.TabPages.Add(tabEdit);
            tabEdit.Text = "Edit Product";
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if(result == DialogResult.Yes)
            {
                try
                {
                    var Product = (Product)ProductBindingSource.Current;
                    isSuccessful = SocketClient.Send("deleteProd:" + Product.IdProduct);
                    if (!IsLocked(isSuccessful))
                    {
                        MessageBox.Show("Operration successful");
                        LoadAllProducts();
                    }
                    else
                        MessageBox.Show("Locked");
                }
                catch (Exception ex)
                {
                    isSuccessful = false.ToString();
                    MessageBox.Show("An error ocurred");
                }
            }            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CleanViewFields();
            tabControl1.TabPages.Remove(tabEdit);
            tabControl1.TabPages.Add(tabProducts);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var model = new Product();
            model.IdProduct = Convert.ToInt32(IdProduct);
            model.Name = Name;
            model.Description = Description;
            model.Price = Convert.ToInt32(Price);
            model.Status = Status;           

            try
            {                
                if (isEdit)
                {
                    var result = SocketClient.Send("editProd|" + JsonConvert.SerializeObject(model));

                    if (!IsLocked(result))
                    {
                        if (result == true.ToString())
                        {
                            LoadAllProducts();

                            tabControl1.TabPages.Remove(tabEdit);
                            tabControl1.TabPages.Add(tabProducts);

                            CleanViewFields();
                            MessageBox.Show("Product edit success");
                        }
                        else
                        {
                            MessageBox.Show("Product edit fail");
                        }
                    }
                    else
                        MessageBox.Show("Locked");
                }
                else
                {
                    var result = SocketClient.Send("addProd|" + JsonConvert.SerializeObject(model));
                    
                    if (!IsLocked(result))
                    {
                        if (result == true.ToString())
                        {
                            LoadAllProducts();

                            tabControl1.TabPages.Remove(tabEdit);
                            tabControl1.TabPages.Add(tabProducts);

                            CleanViewFields();
                            MessageBox.Show("Product add success");
                        }
                        else
                        {
                            MessageBox.Show("Product add fail");
                        }
                    }
                    else
                        MessageBox.Show("Locked");
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLogs_Click(object sender, EventArgs e)
        {
            Process.Start(@"C:\Users\GameMax\source\repos\NetTechLab3\Server\Logs.txt");
        }
    }
}
