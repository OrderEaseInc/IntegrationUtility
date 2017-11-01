using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataTransfer.AccessDatabase;
using LinkGreenODBCUtility.Properties;

namespace LinkGreenODBCUtility
{
    public partial class UtilityMappings : Form
    {
        public static Mapping Mapping = new Mapping();
        public string DsnName = Settings.DsnName;

        public UtilityMappings()
        {
            InitializeComponent();
        }

        private void UtilityLoad(object sender, EventArgs e)
        {
            // Add the dsn data source names to the list box
            List<string> odbcDataSourceNames = GetOdbcDataSources();
            foreach (var sourceName in odbcDataSourceNames)
            {
                if (sourceName != Settings.DsnName)
                {
                    categoriesDataSource.Items.Add(sourceName);
                    customersDataSource.Items.Add(sourceName);
                    productsDataSource.Items.Add(sourceName);
                    priceLevelsDataSource.Items.Add(sourceName);
                    pricingDataSource.Items.Add(sourceName);
                    suppliersDataSource.Items.Add(sourceName);
                    supplierInventoryDataSource.Items.Add(sourceName);
                    buyerInventoryDataSource.Items.Add(sourceName);
                }
            }
            
            var mapping = new Mapping(DsnName);

            // Categories
            string mappedDsnName = mapping.GetDsnName("Categories");
            int idx = categoriesDataSource.FindString(mappedDsnName);
            if (idx != -1)
            {
                categoriesDataSource.SetSelected(idx, true);
            }
            DisplayActiveTableMapping();
        }

        private void mappingTabChanged(object sender, EventArgs e)
        {
            var mapping = new Mapping(DsnName);
            int cusIdx;

            switch (Tables.SelectedIndex)
            {
                case 1:
                    string mappedCustomerDsnName = mapping.GetDsnName("Customers");
                    cusIdx = customersDataSource.FindString(mappedCustomerDsnName);
                    if (cusIdx != -1)
                    {
                        customersDataSource.SetSelected(cusIdx, true);
                    }
                    DisplayActiveCustomerTableMapping();
                    break;
                case 2:
                    string mappedProductDsnName = mapping.GetDsnName("Products");
                    cusIdx = productsDataSource.FindString(mappedProductDsnName);
                    if (cusIdx != -1)
                    {
                        productsDataSource.SetSelected(cusIdx, true);
                    }
                    DisplayActiveProductTableMapping();
                    break;
                case 3:
                    string mappedPriceLevelsDsnName = mapping.GetDsnName("PriceLevels");
                    cusIdx = priceLevelsDataSource.FindString(mappedPriceLevelsDsnName);
                    if (cusIdx != -1)
                    {
                        priceLevelsDataSource.SetSelected(cusIdx, true);
                    }
                    DisplayActivePriceLevelsTableMapping();
                    break;
                case 4:
                    string mappedPricingDsnName = mapping.GetDsnName("PriceLevelPrices");
                    cusIdx = pricingDataSource.FindString(mappedPricingDsnName);
                    if (cusIdx != -1)
                    {
                        pricingDataSource.SetSelected(cusIdx, true);
                    }
                    DisplayActivePricingTableMapping();
                    break;
                case 5:
                    string mappedSuppliersDsnName = mapping.GetDsnName("Suppliers");
                    cusIdx = suppliersDataSource.FindString(mappedSuppliersDsnName);
                    if (cusIdx != -1) {
                        suppliersDataSource.SetSelected(cusIdx, true);
                    }
                    DisplayActiveSupplierTableMapping();
                    break;
                case 6:
                    string mappedSupplierInventoryDsnName = mapping.GetDsnName("SupplierInventories");
                    cusIdx = supplierInventoryDataSource.FindString(mappedSupplierInventoryDsnName);
                    if (cusIdx != -1) {
                        supplierInventoryDataSource.SetSelected(cusIdx, true);
                    }
                    DisplayActiveSupplierInventoryTableMapping();
                    break;
                case 7:
                    var mappedBuyerInventoryDsnName = mapping.GetDsnName("BuyerInventories");
                    cusIdx = buyerInventoryDataSource.FindString(mappedBuyerInventoryDsnName);
                    if (cusIdx != -1) {
                        buyerInventoryDataSource.SetSelected(cusIdx, true);
                    }
                    DisplayActiveBuyerInventoryTableMapping();
                    break;
                default:
                    MessageBox.Show("Invalid tab selected", "Invalid Tab");
                    break;
            }
        }

        // Custom methods
        public List<string> GetOdbcDataSources()
        {
            int envHandle = 0;
            const int SQL_FETCH_NEXT = 1;
            const int SQL_FETCH_FIRST_SYSTEM = 32;
            List<string> dsnNames = new List<string>();

            if (OdbcWrapper.SQLAllocEnv(ref envHandle) != -1)
            {
                int ret;
                StringBuilder serverName = new StringBuilder(1024);
                StringBuilder driverName = new StringBuilder(1024);
                int snLen = 0;
                int driverLen = 0;
                ret = OdbcWrapper.SQLDataSources(envHandle, SQL_FETCH_FIRST_SYSTEM, serverName, serverName.Capacity, ref snLen,
                    driverName, driverName.Capacity, ref driverLen);
                while (ret == 0)
                {
                    dsnNames.Add(serverName.ToString());
                    ret = OdbcWrapper.SQLDataSources(envHandle, SQL_FETCH_NEXT, serverName, serverName.Capacity, ref snLen,
                        driverName, driverName.Capacity, ref driverLen);
                }
            }

            return dsnNames;
        }

        private void DisplayActiveTableMapping()
        {
            var mapping = new Mapping(DsnName);
            string mappedTableName = mapping.GetTableMapping("Categories");
            string mappedDsnName = mapping.GetDsnName("Categories");
            if (!string.IsNullOrEmpty(mappedTableName))
            {
                string activeTableMapping = mappedDsnName + " > " + mappedTableName;
                activeCategoriesTableMappingValue.Text = activeTableMapping;
                mappedTableFieldsLabel.Text = activeTableMapping;
            }
        }

        private void DisplayActiveCustomerTableMapping()
        {
            var mapping = new Mapping(DsnName);
            string mappedTableName = mapping.GetTableMapping("Customers");
            string mappedDsnName = mapping.GetDsnName("Customers");
            if (!string.IsNullOrEmpty(mappedTableName))
            {
                string activeTableMapping = mappedDsnName + " > " + mappedTableName;
                activeCustomerTableMappingValue.Text = activeTableMapping;
                mappedCustomerTableFieldsLabel.Text = activeTableMapping;
            }
        }

        private void DisplayActiveProductTableMapping()
        {
            string mappedTableName = Mapping.GetTableMapping("Products");
            string mappedDsnName = Mapping.GetDsnName("Products");
            if (!string.IsNullOrEmpty(mappedTableName))
            {
                string activeTableMapping = mappedDsnName + " > " + mappedTableName;
                activeProductTableMappingValue.Text = activeTableMapping;
                mappedProductTableFieldsLabel.Text = activeTableMapping;
            }
        }

        private void DisplayActiveBuyerInventoryTableMapping()
        {
            string mappedTableName = Mapping.GetTableMapping("BuyerInventories");
            string mappedDsnName = Mapping.GetDsnName("BuyerInventories");
            if (!string.IsNullOrEmpty(mappedTableName)) {
                string activeTableMapping = mappedDsnName + " > " + mappedTableName;
                activeBuyerInventoryFieldMappingValue.Text = activeTableMapping;
                mappedBuyerInventoryTableFieldsLabel.Text = activeTableMapping;
            }
        }

        private void DisplayActivePricingTableMapping()
        {
            string mappedTableName = Mapping.GetTableMapping("PriceLevelPrices");
            string mappedDsnName = Mapping.GetDsnName("PriceLevelPrices");
            if (!string.IsNullOrEmpty(mappedTableName))
            {
                string activeTableMapping = mappedDsnName + " > " + mappedTableName;
                activePricingTableMappingValue.Text = activeTableMapping;
                mappedPricingTableFieldsLabel.Text = activeTableMapping;
            }
        }

        private void DisplayActivePriceLevelsTableMapping()
        {
            string mappedTableName = Mapping.GetTableMapping("PriceLevels");
            string mappedDsnName = Mapping.GetDsnName("PriceLevels");
            if (!string.IsNullOrEmpty(mappedTableName))
            {
                string activeTableMapping = mappedDsnName + " > " + mappedTableName;
                activePriceLevelsTableMappingValue.Text = activeTableMapping;
                mappedPriceLevelsTableFieldsLabel.Text = activeTableMapping; 
            }
        }

        private void DisplayActiveSupplierTableMapping()
        {
            var mapping = new Mapping(DsnName);
            string mappedTableName = mapping.GetTableMapping("Suppliers");
            string mappedDsnName = mapping.GetDsnName("Suppliers");
            if (!string.IsNullOrEmpty(mappedTableName)) {
                string activeTableMapping = mappedDsnName + " > " + mappedTableName;
                activeSupplierTableMappingValue.Text = activeTableMapping;
                mappedSuppliersTableFieldsLabel.Text = activeTableMapping;
            }

            var suppliersTableItem = suppliersTableName.Items.Cast<string>().FirstOrDefault(i => i == mappedTableName);
            if (suppliersTableItem != null) {
                suppliersTableName.SelectedItem = suppliersTableItem;
            }
        }

        private void DisplayActiveSupplierInventoryTableMapping()
        {
            var mapping = new Mapping(DsnName);
            string mappedTableName = mapping.GetTableMapping("SupplierInventories");
            string mappedDsnName = mapping.GetDsnName("SupplierInventories");
            if (!string.IsNullOrEmpty(mappedTableName)) {
                string activeTableMapping = mappedDsnName + " > " + mappedTableName;
                activeSupplierInventoryTableMappingValue.Text = activeTableMapping;
                mappedSupplierInventoryTableFieldsLabel.Text = activeTableMapping;
            }

            var supplierInventoryTableItem = supplierInventoryTableName.Items.Cast<string>().FirstOrDefault(i => i == mappedTableName);
            if (supplierInventoryTableItem != null) {
                supplierInventoryTableName.SelectedItem = supplierInventoryTableItem;
            }
        }

        private void DisplayActiveFieldMapping(string displayName, string fieldName)
        {
            string mappedFieldName = Mapping.GetFieldMapping("Categories", fieldName);
            if (!string.IsNullOrEmpty(mappedFieldName))
            {
                activeFieldMappingLabel.Text = displayName + " : ";
                activeFieldMappingValue.Text = mappedFieldName;
            }
            else
            {
                activeFieldMappingLabel.Text = displayName + " : ";
                activeFieldMappingValue.Text = "N/A";
            }
        }

        private void DisplayActiveCustomerFieldMapping(string displayName, string fieldName)
        {
            string mappedFieldName = Mapping.GetFieldMapping("Customers", fieldName);
            if (!string.IsNullOrEmpty(mappedFieldName))
            {
                activeCustomerFieldMappingLabel.Text = displayName + " : ";
                activeCustomerFieldMappingValue.Text = mappedFieldName;
            }
            else
            {
                activeCustomerFieldMappingLabel.Text = displayName + " : ";
                activeCustomerFieldMappingValue.Text = "N/A";
            }
        }

        private void DisplayActiveSupplierFieldMapping(string displayName, string fieldName)
        {
            string mappedFieldName = Mapping.GetFieldMapping("Suppliers", fieldName);
            if (!string.IsNullOrEmpty(mappedFieldName)) {
                activeSupplierFieldMappingLabel.Text = displayName + " : ";
                activeSupplierFieldMappingValue.Text = mappedFieldName;
            } else {
                activeSupplierFieldMappingLabel.Text = displayName + " : ";
                activeSupplierFieldMappingValue.Text = "N/A";
            }
        }

        private void DisplayActiveSupplierInventoryFieldMapping(string displayName, string fieldName)
        {
            string mappedFieldName = Mapping.GetFieldMapping("SupplierInventories", fieldName);
            if (!string.IsNullOrEmpty(mappedFieldName)) {
                activeSupplierInventoryFieldMappingLabel.Text = displayName + " : ";
                activeSupplierInventoryFieldMappingValue.Text = mappedFieldName;
            } else {
                activeSupplierInventoryFieldMappingLabel.Text = displayName + " : ";
                activeSupplierInventoryFieldMappingValue.Text = "N/A";
            }
        }

        private void DisplayActiveProductFieldMapping(string displayName, string fieldName)
        {
            string mappedFieldName = Mapping.GetFieldMapping("Products", fieldName);
            if (!string.IsNullOrEmpty(mappedFieldName))
            {
                activeProductFieldMappingLabel.Text = displayName + " : ";
                activeProductFieldMappingValue.Text = mappedFieldName;
            }
            else
            {
                activeProductFieldMappingLabel.Text = displayName + " : ";
                activeProductFieldMappingValue.Text = "N/A";
            }
        }

        private void DisplayActivePricingFieldMapping(string displayName, string fieldName)
        {
            string mappedFieldName = Mapping.GetFieldMapping("PriceLevelPrices", fieldName);
            if (!string.IsNullOrEmpty(mappedFieldName))
            {
                activePricingFieldMappingLabel.Text = displayName + " : ";
                activePricingFieldMappingValue.Text = mappedFieldName;
            }
            else
            {
                activePricingFieldMappingLabel.Text = displayName + " : ";
                activePricingFieldMappingValue.Text = "N/A";
            }
        }

        private void DisplayActivePriceLevelsFieldMapping(string displayName, string fieldName)
        {
            string mappedFieldName = Mapping.GetFieldMapping("PriceLevels", fieldName);
            if (!string.IsNullOrEmpty(mappedFieldName))
            {
                activePriceLevelsFieldMappingLabel.Text = displayName + " : ";
                activePriceLevelsFieldMappingValue.Text = mappedFieldName;
            }
            else
            {
                activePriceLevelsFieldMappingLabel.Text = displayName + " : ";
                activePriceLevelsFieldMappingValue.Text = "N/A";
            }
        }

        private void DisplayFieldDescription(string tableName, string fieldName)
        {   
            string dataType = Mapping.GetFieldProperty(tableName, fieldName, "DataType");
            string description = Mapping.GetFieldProperty(tableName, fieldName, "Description");
            if (!string.IsNullOrEmpty(description))
            {
                fieldDescription.Text = description + " (" + dataType + ")";
            }
        }

        private void DisplayCustomerFieldDescription(string tableName, string fieldName)
        {
            string dataType = Mapping.GetFieldProperty(tableName, fieldName, "DataType");
            string description = Mapping.GetFieldProperty(tableName, fieldName, "Description");
            if (!string.IsNullOrEmpty(description))
            {
                customerFieldDescription.Text = description + " (" + dataType + ")";
            }
        }

        private void DisplaySupplierFieldDescription(string tableName, string fieldName)
        {
            string dataType = Mapping.GetFieldProperty(tableName, fieldName, "DataType");
            string description = Mapping.GetFieldProperty(tableName, fieldName, "Description");
            if (!string.IsNullOrEmpty(description)) {
                supplierFieldDescription.Text = description + " (" + dataType + ")";
            }
        }

        private void DisplaySupplierInventoryFieldDescription(string tableName, string fieldName)
        {
            string dataType = Mapping.GetFieldProperty(tableName, fieldName, "DataType");
            string description = Mapping.GetFieldProperty(tableName, fieldName, "Description");
            if (!string.IsNullOrEmpty(description)) {
                supplierInventoryFieldDescription.Text = description + " (" + dataType + ")";
            }
        }

        private void DisplayProductFieldDescription(string tableName, string fieldName)
        {  
            string dataType = Mapping.GetFieldProperty(tableName, fieldName, "DataType");
            string description = Mapping.GetFieldProperty(tableName, fieldName, "Description");
            if (!string.IsNullOrEmpty(description))
            {
                productFieldDescriptionValue.Text = description + " (" + dataType + ")";
            }
        }

        private void DisplayPricingFieldDescription(string tableName, string fieldName)
        {   
            string dataType = Mapping.GetFieldProperty(tableName, fieldName, "DataType");
            string description = Mapping.GetFieldProperty(tableName, fieldName, "Description");
            if (!string.IsNullOrEmpty(description))
            {
                pricingFieldDescriptionValue.Text = description + " (" + dataType + ")";
            }
        }

        private void DisplayPriceLevelsFieldDescription(string tableName, string fieldName)
        {
            string dataType = Mapping.GetFieldProperty(tableName, fieldName, "DataType");
            string description = Mapping.GetFieldProperty(tableName, fieldName, "Description");
            if (!string.IsNullOrEmpty(description))
            {
                priceLevelsFieldDescriptionValue.Text = description + " (" + dataType + ")";
            }
        }

        private void SetupMappingFields()
        {
            //Set the required fields from access db
            List<MappingField> requiredFields = Mapping.GetTableFields("Categories");
            requiredCategoryFields.Items.Clear();
            foreach (MappingField requiredField in requiredFields)
            {
                ListItem item = new ListItem();
                string nameToShow = string.IsNullOrEmpty(requiredField.DisplayName) ? requiredField.FieldName : requiredField.DisplayName;
                item.Text = string.IsNullOrEmpty(requiredField.MappingName) ? nameToShow + " : Not Mapped" : nameToShow + " : " + requiredField.MappingName;
                if (requiredField.Required)
                {
                    item.Text = "* " + item.Text;
                }
                item.Value = requiredField.FieldName;
                item.Display = nameToShow;
                requiredCategoryFields.DisplayMember = "Text";
                requiredCategoryFields.Items.Add(item);
            }

            //Set the available fields to map
            if (categoriesDataSource.SelectedItem != null)
            {
                string mappedTableName = Mapping.GetTableMapping("Categories");
                string mappedDsnName = Mapping.GetDsnName("Categories");

                mappingCategoryFields.Items.Clear();
                if (!string.IsNullOrEmpty(mappedTableName))
                {
                    List<string> columns = Mapping.GetColumns(mappedTableName, mappedDsnName); 
                    foreach (string column in columns) 
                    {
                        mappingCategoryFields.Items.Add(column);
                    }
                }
            } 
        }

        private void SetupCustomerMappingFields()
        {
            //Set the required fields from access db
            List<MappingField> tableFields = Mapping.GetTableFields("Customers");
            customerFields.Items.Clear();
            foreach (MappingField tableField in tableFields)
            {
                ListItem item = new ListItem();
                string nameToShow = string.IsNullOrEmpty(tableField.DisplayName) ? tableField.FieldName : tableField.DisplayName;
                item.Text = string.IsNullOrEmpty(tableField.MappingName) ? nameToShow + " : Not Mapped" : nameToShow + " : " + tableField.MappingName;
                if (tableField.Required)
                {
                    item.Text = "* " + item.Text;
                }
                item.Value = tableField.FieldName;
                item.Display = nameToShow;
                customerFields.DisplayMember = "Text";
                customerFields.Items.Add(item);
            }

            //Set the available fields to map
            if (customersDataSource.SelectedItem != null)
            {
                string mappedTableName = Mapping.GetTableMapping("Customers");
                string mappedDsnName = Mapping.GetDsnName("Customers");

                mappingCustomerFields.Items.Clear();
                if (!string.IsNullOrEmpty(mappedTableName))
                {
                    List<string> columns = Mapping.GetColumns(mappedTableName, mappedDsnName);
                    foreach (string column in columns)
                    {
                        mappingCustomerFields.Items.Add(column);
                    }
                }
            }
        }

        private void SetupSupplierMappingFields()
        {
            //Set the required fields from access db
            List<MappingField> tableFields = Mapping.GetTableFields("Suppliers");
            supplierFields.Items.Clear();
            foreach (MappingField tableField in tableFields) {
                ListItem item = new ListItem();
                string nameToShow = string.IsNullOrEmpty(tableField.DisplayName) ? tableField.FieldName : tableField.DisplayName;
                item.Text = string.IsNullOrEmpty(tableField.MappingName) ? nameToShow + " : Not Mapped" : nameToShow + " : " + tableField.MappingName;
                if (tableField.Required) {
                    item.Text = "* " + item.Text;
                }
                item.Value = tableField.FieldName;
                item.Display = nameToShow;
                supplierFields.DisplayMember = "Text";
                supplierFields.Items.Add(item);
            }

            //Set the available fields to map
            if (suppliersDataSource.SelectedItem != null) {
                string mappedTableName = Mapping.GetTableMapping("Suppliers");
                string mappedDsnName = Mapping.GetDsnName("Suppliers");

                mappingSupplierFields.Items.Clear();
                if (!string.IsNullOrEmpty(mappedTableName)) {
                    List<string> columns = Mapping.GetColumns(mappedTableName, mappedDsnName);
                    foreach (string column in columns) {
                        mappingSupplierFields.Items.Add(column);
                    }
                }
            }
        }

        private void SetupSupplierInventoryMappingFields()
        {
            //Set the required fields from access db
            List<MappingField> tableFields = Mapping.GetTableFields("SupplierInventories");
            requiredSupplierInventoryFields.Items.Clear();
            foreach (MappingField tableField in tableFields) {
                ListItem item = new ListItem();
                string nameToShow = string.IsNullOrEmpty(tableField.DisplayName) ? tableField.FieldName : tableField.DisplayName;
                item.Text = string.IsNullOrEmpty(tableField.MappingName) ? nameToShow + " : Not Mapped" : nameToShow + " : " + tableField.MappingName;
                if (tableField.Required) {
                    item.Text = "* " + item.Text;
                }
                item.Value = tableField.FieldName;
                item.Display = nameToShow;
                requiredSupplierInventoryFields.DisplayMember = "Text";
                requiredSupplierInventoryFields.Items.Add(item);
            }

            //Set the available fields to map
            if (supplierInventoryDataSource.SelectedItem != null) {
                string mappedTableName = Mapping.GetTableMapping("SupplierInventories");
                string mappedDsnName = Mapping.GetDsnName("SupplierInventories");

                mappingSupplierInventoryFields.Items.Clear();
                if (!string.IsNullOrEmpty(mappedTableName)) {
                    List<string> columns = Mapping.GetColumns(mappedTableName, mappedDsnName);
                    foreach (string column in columns) {
                        mappingSupplierInventoryFields.Items.Add(column);
                    }
                }
            }
        }

        private void SetupProductMappingFields()
        {
            //Set the required fields from access db
            List<MappingField> tableFields = Mapping.GetTableFields("Products");
            this.productFields.Items.Clear();
            foreach (MappingField tableField in tableFields)
            {
                ListItem item = new ListItem();
                string nameToShow = string.IsNullOrEmpty(tableField.DisplayName) ? tableField.FieldName : tableField.DisplayName;
                item.Text = string.IsNullOrEmpty(tableField.MappingName) ? nameToShow + " : Not Mapped" : nameToShow + " : " + tableField.MappingName;
                if (tableField.Required)
                {
                    item.Text = "* " + item.Text;
                }
                item.Value = tableField.FieldName;
                item.Display = nameToShow;
                this.productFields.DisplayMember = "Text";
                this.productFields.Items.Add(item);
            }

            //Set the available fields to map
            if (productsDataSource.SelectedItem != null)
            {
                string mappedTableName = Mapping.GetTableMapping("Products");
                string mappedDsnName = Mapping.GetDsnName("Products");

                mappingProductFields.Items.Clear();
                if (!string.IsNullOrEmpty(mappedTableName))
                {
                    List<string> columns = Mapping.GetColumns(mappedTableName, mappedDsnName);
                    foreach (string column in columns)
                    {
                        mappingProductFields.Items.Add(column);
                    }
                }
            }
        }

        private void SetupBuyerInventoryMappingFields()
        {
            //Set the required fields from access db
            List<MappingField> tableFields = Mapping.GetTableFields("BuyerInventories");
            requiredBuyerInventoryFields.Items.Clear();
            foreach (MappingField tableField in tableFields) {
                ListItem item = new ListItem();
                string nameToShow = string.IsNullOrEmpty(tableField.DisplayName) ? tableField.FieldName : tableField.DisplayName;
                item.Text = string.IsNullOrEmpty(tableField.MappingName) ? nameToShow + " : Not Mapped" : nameToShow + " : " + tableField.MappingName;
                if (tableField.Required) {
                    item.Text = "* " + item.Text;
                }
                item.Value = tableField.FieldName;
                item.Display = nameToShow;
                requiredBuyerInventoryFields.DisplayMember = "Text";
                requiredBuyerInventoryFields.Items.Add(item);
            }

            //Set the available fields to map
            if (buyerInventoryDataSource.SelectedItem != null) {
                string mappedTableName = Mapping.GetTableMapping("BuyerInventories");
                string mappedDsnName = Mapping.GetDsnName("BuyerInventories");

                mappingBuyerInventoryFields.Items.Clear();
                if (!string.IsNullOrEmpty(mappedTableName)) {
                    List<string> columns = Mapping.GetColumns(mappedTableName, mappedDsnName);
                    foreach (string column in columns) {
                        mappingBuyerInventoryFields.Items.Add(column);
                    }
                }
            }
        }

        private void SetupPricingMappingFields()
        {
            //Set the required fields from access db
            List<MappingField> tableFields = Mapping.GetTableFields("PriceLevelPrices");
            pricingFields.Items.Clear();
            foreach (MappingField tableField in tableFields)
            {
                ListItem item = new ListItem();
                string nameToShow = string.IsNullOrEmpty(tableField.DisplayName) ? tableField.FieldName : tableField.DisplayName;
                item.Text = string.IsNullOrEmpty(tableField.MappingName) ? nameToShow + " : Not Mapped" : nameToShow + " : " + tableField.MappingName;
                if (tableField.Required)
                {
                    item.Text = "* " + item.Text;
                }
                item.Value = tableField.FieldName;
                item.Display = nameToShow;
                pricingFields.DisplayMember = "Text";
                pricingFields.Items.Add(item);
            }

            //Set the available fields to map
            if (pricingDataSource.SelectedItem != null)
            {
                string mappedTableName = Mapping.GetTableMapping("PriceLevelPrices");
                string mappedDsnName = Mapping.GetDsnName("PriceLevelPrices");

                mappingPricingFields.Items.Clear();
                if (!string.IsNullOrEmpty(mappedTableName))
                {
                    List<string> columns = Mapping.GetColumns(mappedTableName, mappedDsnName);
                    foreach (string column in columns)
                    {
                        mappingPricingFields.Items.Add(column);
                    }
                }
            }
        }

        private void SetupPriceLevelsMappingFields()
        {
            //Set the required fields from access db
            var requiredMapping = new Mapping();
            List<MappingField> tableFields = requiredMapping.GetTableFields("PriceLevels");
            priceLevelFields.Items.Clear();
            foreach (MappingField tableField in tableFields)
            {
                ListItem item = new ListItem();
                string nameToShow = string.IsNullOrEmpty(tableField.DisplayName) ? tableField.FieldName : tableField.DisplayName;
                item.Text = string.IsNullOrEmpty(tableField.MappingName) ? nameToShow + " : Not Mapped" : nameToShow + " : " + tableField.MappingName;
                if (tableField.Required)
                {
                    item.Text = "* " + item.Text;
                }
                item.Value = tableField.FieldName;
                item.Display = nameToShow;
                priceLevelFields.DisplayMember = "Text";
                priceLevelFields.Items.Add(item);
            }

            //Set the available fields to map
            if (priceLevelsDataSource.SelectedItem != null)
            {
                string mappedTableName = Mapping.GetTableMapping("PriceLevels");
                string mappedDsnName = Mapping.GetDsnName("PriceLevels");

                mappingPriceLevelFields.Items.Clear();
                if (!string.IsNullOrEmpty(mappedTableName))
                {
                    List<string> columns = Mapping.GetColumns(mappedTableName, mappedDsnName);
                    foreach (string column in columns)
                    {
                        mappingPriceLevelFields.Items.Add(column);
                    }
                }
            }
        }


        // Event methods
        private void EmptyCategoriesTransferTable_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var categories = new Categories();
                if (categories.Empty())
                {
                    MessageBox.Show("Categories transfer table emptied.", "Emptied Successfully");
                }
            }
        }

        private void PublishCategories_Click(object sender, EventArgs e)
        {
            var categories = new Categories();
            if (categories.Publish())
            {
                MessageBox.Show("Categories Published", "Success");
            }
            else
            {
                MessageBox.Show("Categories failed to publish. No API Key was found", "Publish Failure");
            }
        }

        private void CategoriesDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (categoriesDataSource.SelectedItem != null)
            {
                string dsnName = categoriesDataSource.SelectedItem.ToString();
                var mapping = new Mapping(dsnName);
                List<string> tableNames = mapping.GetTableNames();

                categoriesTableName.Items.Clear();
                foreach (string tableName in tableNames)
                {
                    categoriesTableName.Items.Add(tableName);
                }

                DisplayActiveTableMapping();
                SetupMappingFields();
            }
        }

        private void categoriesTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var categories = new Categories();
            if (categoriesTableName.SelectedItem != null)
            {
                string dsnName = categoriesDataSource.SelectedItem.ToString();
                string tableName = categoriesTableName.SelectedItem.ToString();
                categories.SaveTableMapping(dsnName, tableName);

                DisplayActiveTableMapping();
                SetupMappingFields();
            }
            else
            {
                MessageBox.Show("Please select your categories table!", "No Category Table Selected");
            }
        }

        private void requiredCategoryFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((requiredCategoryFields.SelectedItem as ListItem).Value != null)
            {
                DisplayActiveFieldMapping((requiredCategoryFields.SelectedItem as ListItem).Display, (requiredCategoryFields.SelectedItem as ListItem).Value);
                
                DisplayFieldDescription("Categories", (requiredCategoryFields.SelectedItem as ListItem).Value);
            }
        }

        private void mapCategoryFields_Click(object sender, EventArgs e)
        {
            if (requiredCategoryFields.SelectedItem != null && mappingCategoryFields.SelectedItem != null)
            {
                var categories = new Categories();
                categories.SaveFieldMapping((requiredCategoryFields.SelectedItem as ListItem).Value, mappingCategoryFields.SelectedItem.ToString());
                DisplayActiveFieldMapping((requiredCategoryFields.SelectedItem as ListItem).Display, (requiredCategoryFields.SelectedItem as ListItem).Value);
                SetupMappingFields();
            }
            else
            {
                MessageBox.Show("Please select a required field followed by one of your fields.", "Both Fields Are Required"); 
            }
        }

        private void migrateCategoryData_Click(object sender, EventArgs e)
        {
            string mappedDsnName = Mapping.GetDsnName("Categories");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Categories"))
            {
                MessageBox.Show("Categories migrated successfully.", "Categories Migrated");
                Logger.Instance.Debug($"Categories migrated using DSN: {mappedDsnName}");
            }
            else
            {
                MessageBox.Show("Failed to migrate categories.", "Migration Failed");
                Logger.Instance.Warning("Failed to migrate categories.");
            }
        }

        private void previewCategoriesMappingOutput_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Mapping.GetTableMapping("Categories")))
            {
                string mappedDsnName = Mapping.GetDsnName("Categories");
                MappingPreview mappingPreview = new MappingPreview("Categories", mappedDsnName);
                mappingPreview.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please setup a table mapping.", "No Table Mapping");
            }
        }

        private void customersDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (categoriesDataSource.SelectedItem != null)
            {
                string dsnName = customersDataSource.SelectedItem.ToString();
                var mapping = new Mapping(dsnName);
                List<string> tableNames = mapping.GetTableNames();

                customersTableName.Items.Clear();
                foreach (string tableName in tableNames)
                {
                    customersTableName.Items.Add(tableName);
                }

                DisplayActiveCustomerTableMapping();
                SetupCustomerMappingFields();
            }
        }

        private void suppliersDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (suppliersDataSource.SelectedItem != null) {
                string dsnName = suppliersDataSource.SelectedItem.ToString();
                var mapping = new Mapping(dsnName);
                List<string> tableNames = mapping.GetTableNames();

                suppliersTableName.Items.Clear();
                foreach (string tableName in tableNames) {
                    suppliersTableName.Items.Add(tableName);
                }

                DisplayActiveSupplierTableMapping();
                SetupSupplierMappingFields();
            }
        }

        private void supplierInventoryDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (supplierInventoryDataSource.SelectedItem != null) {
                string dsnName = supplierInventoryDataSource.SelectedItem.ToString();
                var mapping = new Mapping(dsnName);
                var tableNames = mapping.GetTableNames();

                supplierInventoryTableName.Items.Clear();
                foreach (string tableName in tableNames) {
                    supplierInventoryTableName.Items.Add(tableName);
                }

                DisplayActiveSupplierInventoryTableMapping();
                SetupSupplierInventoryMappingFields();
            }
        }

        private void emptyCustomersTransferTable_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var customers = new Customers();
                if (customers.Empty())
                {
                    MessageBox.Show("Customers transfer table emptied.", "Emptied Successfully");
                }
            }
        }

        private void emptySuppliersTransferTable_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes) {
                var suppliers = new Suppliers();
                if (suppliers.Empty()) {
                    MessageBox.Show("Suppliers transfer table emptied.", "Emptied Successfully");
                }
            }
        }

        private void emptyBuyerInventoriesTransferTable_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes) {
                var buyerInventories = new BuyerInventories();
                if (buyerInventories.Empty()) {
                    MessageBox.Show("Buyer Inventory transfer table emptied.", "Emptied Successfully");
                }
            }
        }

        private void emptySupplierInventoriesTransferTable_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes) {
                var supplierInventories = new SupplierInventories();
                if (supplierInventories.Empty()) {
                    MessageBox.Show("Supplier Inventories transfer table emptied.", "Emptied Successfully");
                }
            }
        }

        private void customersTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var customers = new Customers();
            if (customersTableName.SelectedItem != null)
            {
                string dsnName = customersDataSource.SelectedItem.ToString();
                string tableName = customersTableName.SelectedItem.ToString();
                customers.SaveTableMapping(dsnName, tableName);

                DisplayActiveCustomerTableMapping();
                SetupCustomerMappingFields();
            }
            else
            {
                MessageBox.Show("Please select your customers table!", "No Customers Table Selected");
            }
        }

        private void suppliersTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var suppliers = new Suppliers();
            if (suppliersTableName.SelectedItem != null) {
                string dsnName = suppliersDataSource.SelectedItem.ToString();
                string tableName = suppliersTableName.SelectedItem.ToString();
                suppliers.SaveTableMapping(dsnName, tableName);

                DisplayActiveSupplierTableMapping();
                SetupSupplierMappingFields();
            } else {
                MessageBox.Show("Please select your suppliers table!", "No Suppliers Table Selected");
            }
        }

        private void supplierInventoryTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var supplierInventories = new SupplierInventories();
            if (supplierInventoryTableName.SelectedItem != null) {
                string dsnName = supplierInventoryDataSource.SelectedItem.ToString();
                string tableName = supplierInventoryTableName.SelectedItem.ToString();
                supplierInventories.SaveTableMapping(dsnName, tableName);

                DisplayActiveSupplierInventoryTableMapping();
                SetupSupplierInventoryMappingFields();
            } else {
                MessageBox.Show("Please select your supplier inventory table!", "No Supplier Inventory Table Selected");
            }
        }

        private void customerFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((customerFields.SelectedItem as ListItem).Value != null)
            {
                DisplayActiveCustomerFieldMapping((customerFields.SelectedItem as ListItem).Display, (customerFields.SelectedItem as ListItem).Value);

                DisplayCustomerFieldDescription("Customers", (customerFields.SelectedItem as ListItem).Value);
            }
        }

        private void supplierFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((supplierFields.SelectedItem as ListItem).Value != null) {
                DisplayActiveSupplierFieldMapping((supplierFields.SelectedItem as ListItem).Display, (supplierFields.SelectedItem as ListItem).Value);

                DisplaySupplierFieldDescription("Suppliers", (supplierFields.SelectedItem as ListItem).Value);
            }
        }

        private void requiredSupplierInventoryFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((requiredSupplierInventoryFields.SelectedItem as ListItem).Value != null) {
                DisplayActiveSupplierFieldMapping((requiredSupplierInventoryFields.SelectedItem as ListItem).Display, (requiredSupplierInventoryFields.SelectedItem as ListItem).Value);

                DisplaySupplierInventoryFieldDescription("SupplierInventories", (requiredSupplierInventoryFields.SelectedItem as ListItem).Value);
            }
        }

        private void mapCustomerFields_Click(object sender, EventArgs e)
        {
            if (customerFields.SelectedItem != null && mappingCustomerFields.SelectedItem != null)
            {
                var customers = new Customers();
                customers.SaveFieldMapping((customerFields.SelectedItem as ListItem).Value, mappingCustomerFields.SelectedItem.ToString());
                DisplayActiveCustomerFieldMapping((customerFields.SelectedItem as ListItem).Display, (customerFields.SelectedItem as ListItem).Value);
                SetupCustomerMappingFields();
            }
            else
            {
                MessageBox.Show("Please select a required field followed by one of your fields.", "Both Fields Are Required");
            }
        }

        private void mapSupplierFields_Click(object sender, EventArgs e)
        {
            if (supplierFields.SelectedItem != null && mappingSupplierFields.SelectedItem != null) {
                var suppliers = new Suppliers();
                suppliers.SaveFieldMapping((supplierFields.SelectedItem as ListItem).Value, mappingSupplierFields.SelectedItem.ToString());
                DisplayActiveSupplierFieldMapping((supplierFields.SelectedItem as ListItem).Display, (supplierFields.SelectedItem as ListItem).Value);
                SetupSupplierMappingFields();
            } else {
                MessageBox.Show("Please select a required field followed by one of your fields.", "Both Fields Are Required");
            }
        }

        private void mapBuyerInventoryFields_Click(object sender, EventArgs e)
        {
            if (requiredBuyerInventoryFields.SelectedItem != null && mappingBuyerInventoryFields.SelectedItem != null) {
                var buyerInventories = new BuyerInventories();
                buyerInventories.SaveFieldMapping((requiredBuyerInventoryFields.SelectedItem as ListItem).Value, mappingBuyerInventoryFields.SelectedItem.ToString());
                DisplayActiveSupplierFieldMapping((requiredBuyerInventoryFields.SelectedItem as ListItem).Display, (requiredBuyerInventoryFields.SelectedItem as ListItem).Value);
                SetupBuyerInventoryMappingFields();
            } else {
                MessageBox.Show("Please select a required field followed by one of your fields.", "Both Fields Are Required");
            }
        }

        private void mapSupplierInventoryFields_Click(object sender, EventArgs e)
        {
            if (requiredSupplierInventoryFields.SelectedItem != null && mappingSupplierInventoryFields.SelectedItem != null) {
                var supplierInventories = new SupplierInventories();
                supplierInventories.SaveFieldMapping((requiredSupplierInventoryFields.SelectedItem as ListItem).Value, mappingSupplierInventoryFields.SelectedItem.ToString());
                DisplayActiveSupplierInventoryFieldMapping((requiredSupplierInventoryFields.SelectedItem as ListItem).Display, (requiredSupplierInventoryFields.SelectedItem as ListItem).Value);
                SetupSupplierInventoryMappingFields();
            } else {
                MessageBox.Show("Please select a required field followed by one of your fields.", "Both Fields Are Required");
            }
        }

        private void previewCustomerMappingOutput_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Mapping.GetTableMapping("Customers")))
            {
                string mappedDsnName = Mapping.GetDsnName("Customers");
                MappingPreview mappingPreview = new MappingPreview("Customers", mappedDsnName);
                mappingPreview.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please setup a table mapping.", "No Table Mapping");
            }
        }

        private void previewSupplierMappingOutput_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Mapping.GetTableMapping("Suppliers"))) {
                var mappedDsnName = Mapping.GetDsnName("Suppliers");
                var mappingPreview = new MappingPreview("Suppliers", mappedDsnName);
                mappingPreview.ShowDialog();
            } else {
                MessageBox.Show("Please setup a table mapping.", "No Table Mapping");
            }
        }

        private void previewSupplierInventoryMappingOutput_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Mapping.GetTableMapping("SupplierInventories"))) {
                var mappedDsnName = Mapping.GetDsnName("SupplierInventories");
                var mappingPreview = new MappingPreview("SupplierInventories", mappedDsnName);
                mappingPreview.ShowDialog();
            } else {
                MessageBox.Show("Please setup a table mapping.", "No Table Mapping");
            }
        }

        private void previewBuyerInventoryMappingOutput_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Mapping.GetTableMapping("BuyerInventories"))) {
                var mappedDsnName = Mapping.GetDsnName("BuyerInventories");
                var mappingPreview = new MappingPreview("BuyerInventories", mappedDsnName);
                mappingPreview.ShowDialog();
            } else {
                MessageBox.Show("Please setup a table mapping.", "No Table Mapping");
            }
        }

        private void migrateCustomerData_Click(object sender, EventArgs e)
        {   
            string mappedDsnName = Mapping.GetDsnName("Customers");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Customers"))
            {
                MessageBox.Show("Customers migrated successfully.", "Customers Migrated");
                Logger.Instance.Debug($"Customers migrated using DSN: {mappedDsnName}");
            }
            else
            {
                MessageBox.Show("Failed to migrate customers.", "Migration Failed");
                Logger.Instance.Warning("Failed to migrate customers.");
            }
        }

        private void migrateSupplierData_Click(object sender, EventArgs e)
        {
            string mappedDsnName = Mapping.GetDsnName("Suppliers");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Suppliers")) {
                MessageBox.Show("Suppliers migrated successfully.", "Suppliers Migrated");
                Logger.Instance.Debug($"Suppliers migrated using DSN: {mappedDsnName}");
            } else {
                MessageBox.Show("Failed to migrate suppliers.", "Migration Failed");
                Logger.Instance.Warning("Failed to migrate suppliers.");
            }
        }

        private void publishCustomers_Click(object sender, EventArgs e)
        {
            var customers = new Customers();
            if (customers.Publish())
            {
                MessageBox.Show("Customers Published", "Success");
            }
            else
            {
                MessageBox.Show("Customers failed to publish. No API Key was found", "Publish Failure");
            }
        }

        private void syncSuppliers_Click(object sender, EventArgs e)
        {
            var suppliers = new Suppliers();
            var result = suppliers.Publish();
            if(result) {
                MessageBox.Show("Suppliers Synced", "Success");
            } else {
                MessageBox.Show("Suppliers failed to sync. No API Key was found", "Sync Failure");
            }
        }

        private void syncSupplierInventory_Click(object sender, EventArgs e)
        {
            var supplierInventories = new SupplierInventories();
            var result = supplierInventories.Publish();
            if (result) {
                MessageBox.Show("Supplier Inventories Synced", "Success");
            } else {
                MessageBox.Show("Supplier Inventories failed to sync. No API Key was found", "Sync Failure");
            }
        }

        private void pullSuppliersFromLinkGreen_Click(object sender, EventArgs e)
        {
            var suppliers = new Suppliers();
            var result = suppliers.Download();
            if (result) {
                MessageBox.Show("Suppliers Downloaded", "Success");
            } else {
                MessageBox.Show("Suppliers failed to download. No API Key was found", "Download Failure");
            }
        }

        private void pullSupplierInventoriesFromLinkGreen_Click(object sender, EventArgs e)
        {
            var supplierInventories = new SupplierInventories();
            var result = supplierInventories.Download();
            if (result) {
                MessageBox.Show("Supplier Inventories Downloaded", "Success");
            } else {
                MessageBox.Show("Supplier Inventories failed to download. No API Key was found", "Download Failure");
            }
        }

        private void productsDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (productsDataSource.SelectedItem != null)
            {
                string dsnName = productsDataSource.SelectedItem.ToString();
                var mapping = new Mapping(dsnName);
                List<string> tableNames = mapping.GetTableNames();

                productsTableName.Items.Clear();
                foreach (string tableName in tableNames)
                {
                    productsTableName.Items.Add(tableName);
                }

                DisplayActiveProductTableMapping();
                SetupProductMappingFields();
            }
        }

        private void buyerInventoryDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (buyerInventoryDataSource.SelectedItem != null) {
                string dsnName = buyerInventoryDataSource.SelectedItem.ToString();
                var mapping = new Mapping(dsnName);
                List<string> tableNames = mapping.GetTableNames();

                buyerInventoryTableName.Items.Clear();
                foreach (string tableName in tableNames) {
                    buyerInventoryTableName.Items.Add(tableName);
                }

                DisplayActiveBuyerInventoryTableMapping();
                SetupBuyerInventoryMappingFields();
            }
        }

        private void productsTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var products = new Products();
            if (productsTableName.SelectedItem != null)
            {
                string dsnName = productsDataSource.SelectedItem.ToString();
                string tableName = productsTableName.SelectedItem.ToString();
                products.SaveTableMapping(dsnName, tableName);

                DisplayActiveProductTableMapping();
                SetupProductMappingFields();
            }
            else
            {
                MessageBox.Show("Please select your products table!", "No Products Table Selected");
            }
        }

        private void buyerInventoryTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var buyerInventories = new BuyerInventories();
            if (buyerInventoryTableName.SelectedItem != null) {
                string dsnName = buyerInventoryDataSource.SelectedItem.ToString();
                string tableName = buyerInventoryTableName.SelectedItem.ToString();
                buyerInventories.SaveTableMapping(dsnName, tableName);

                DisplayActiveBuyerInventoryTableMapping();
                SetupBuyerInventoryMappingFields();
            } else {
                MessageBox.Show("Please select your buyer inventory table!", "No Buyer Inventory Table Selected");
            }
        }

        private void productFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((productFields.SelectedItem as ListItem).Value != null)
            {
                DisplayActiveProductFieldMapping((productFields.SelectedItem as ListItem).Display, (productFields.SelectedItem as ListItem).Value);

                DisplayProductFieldDescription("Products", (productFields.SelectedItem as ListItem).Value);
            }
        }

        private void previewProductsMappingOutput_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Mapping.GetTableMapping("Products")))
            {
                string mappedDsnName = Mapping.GetDsnName("Products");
                MappingPreview mappingPreview = new MappingPreview("Products", mappedDsnName);
                mappingPreview.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please setup a table mapping.", "No Table Mapping");
            }
        }

        private void mapProductFields_Click(object sender, EventArgs e)
        {
            if (productFields.SelectedItem != null && mappingProductFields.SelectedItem != null)
            {
                var products = new Products();
                products.SaveFieldMapping((productFields.SelectedItem as ListItem).Value, mappingProductFields.SelectedItem.ToString());
                DisplayActiveProductFieldMapping((productFields.SelectedItem as ListItem).Display, (productFields.SelectedItem as ListItem).Value);
                SetupProductMappingFields();
            }
            else
            {
                MessageBox.Show("Please select a required field followed by one of your fields.", "Both Fields Are Required");
            }
        }

        private void emptyProductsTransferTable_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var products = new Products();
                if (products.Empty())
                {
                    MessageBox.Show("Products transfer table emptied.", "Emptied Successfully");
                }
            }
        }

        private void migrateProductData_Click(object sender, EventArgs e)
        {
            var products = new Products();
            products.UpdateTemporaryTables();

            
            string mappedDsnName = Mapping.GetDsnName("Products");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Products"))
            {
                MessageBox.Show("Products migrated successfully.", "Products Migrated");
                Logger.Instance.Debug($"Products migrated using DSN: {mappedDsnName}");
            }
            else
            {
                MessageBox.Show("Failed to migrate products.", "Migration Failed");
                Logger.Instance.Warning("Failed to migrate products.");
            }
        }

        private void migrateBuyerInventoryData_Click(object sender, EventArgs e)
        {
            var buyerInventories = new BuyerInventories();
            buyerInventories.UpdateTemporaryTables();


            string mappedDsnName = Mapping.GetDsnName("BuyerInventories");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("BuyerInventories")) {
                MessageBox.Show("Buyer Inventory migrated successfully.", "Buyer Inventory Migrated");
                Logger.Instance.Debug($"Buyer Inventory migrated using DSN: {mappedDsnName}");
            } else {
                MessageBox.Show("Failed to migrate Buyer Inventory.", "Migration Failed");
                Logger.Instance.Warning("Failed to migrate Buyer Inventory.");
            }
        }

        private void publishProducts_Click(object sender, EventArgs e)
        {
            var products = new Products();
            if (products.Publish())
            {
                MessageBox.Show("Products Published", "Success");
            }
            else
            {
                MessageBox.Show("Products failed to publish. No API Key was found", "Publish Failure");
            }
        }

        private void publishBuyerInventories_Click(object sender, EventArgs e)
        {
            var buyerInventories = new BuyerInventories();
            if (buyerInventories.Publish()) {
                MessageBox.Show("Buyer Inventory Published", "Success");
            } else {
                MessageBox.Show("Buyer Inventory failed to publish. No API Key was found", "Publish Failure");
            }
        }

        private void pricingDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pricingDataSource.SelectedItem != null)
            {
                string dsnName = pricingDataSource.SelectedItem.ToString();
                var mapping = new Mapping(dsnName);
                List<string> tableNames = mapping.GetTableNames();

                pricingTableName.Items.Clear();
                foreach (string tableName in tableNames)
                {
                    pricingTableName.Items.Add(tableName);
                }

                DisplayActivePricingTableMapping();
                SetupPricingMappingFields();
            }
        }

        private void pricingTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var pricing = new PriceLevelPrices();
            if (pricingTableName.SelectedItem != null)
            {
                string dsnName = pricingDataSource.SelectedItem.ToString();
                string tableName = pricingTableName.SelectedItem.ToString();
                pricing.SaveTableMapping(dsnName, tableName);

                DisplayActivePricingTableMapping();
                SetupPricingMappingFields();
            }
            else
            {
                MessageBox.Show("Please select your pricing table!", "No Pricing Table Selected");
            }
        }

        private void pricingFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((pricingFields.SelectedItem as ListItem) != null && (pricingFields.SelectedItem as ListItem).Value != null)
            {
                DisplayActivePricingFieldMapping((pricingFields.SelectedItem as ListItem).Display, (pricingFields.SelectedItem as ListItem).Value);

                DisplayPricingFieldDescription("PriceLevelPrices", (pricingFields.SelectedItem as ListItem).Value);
            }
        }

        private void previewPricingMappingOutput_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Mapping.GetTableMapping("PriceLevelPrices")))
            {
                string mappedDsnName = Mapping.GetDsnName("PriceLevelPrices");
                MappingPreview mappingPreview = new MappingPreview("PriceLevelPrices", mappedDsnName);
                mappingPreview.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please setup a table mapping.", "No Table Mapping");
            }
        }

        private void mapPricingFields_Click(object sender, EventArgs e)
        {
            if (pricingFields.SelectedItem != null && mappingPricingFields.SelectedItem != null)
            {
                var pricing = new PriceLevelPrices();
                pricing.SaveFieldMapping((pricingFields.SelectedItem as ListItem).Value, mappingPricingFields.SelectedItem.ToString());
                DisplayActivePricingFieldMapping((pricingFields.SelectedItem as ListItem).Display, (pricingFields.SelectedItem as ListItem).Value);
                SetupPricingMappingFields();
            }
            else
            {
                MessageBox.Show("Please select a required field followed by one of your fields.", "Both Fields Are Required");
            }
        }

        private void emptyPricingTransferTable_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var pricing = new PriceLevelPrices();
                if (pricing.Empty())
                {
                    MessageBox.Show("Pricing transfer table emptied.", "Emptied Successfully");
                }
            }
        }

        private void migratePricingData_Click(object sender, EventArgs e)
        {
            var pricing = new PriceLevelPrices();
            pricing.UpdateTemporaryTables();
            
            string mappedDsnName = Mapping.GetDsnName("PriceLevelPrices");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("PriceLevelPrices"))
            {
                MessageBox.Show("Pricing migrated successfully.", "Pricing Migrated");
                Logger.Instance.Debug($"Pricing migrated using DSN: {mappedDsnName}");
            }
            else
            {
                MessageBox.Show("Failed to migrate pricing.", "Migration Failed");
                Logger.Instance.Warning("Failed to migrate pricing.");
            }
        }

        private void publishPricing_Click_1(object sender, EventArgs e)
        {
            var pricing = new PriceLevelPrices();
            if (pricing.Publish())
            {
                MessageBox.Show("Pricing Published", "Success");
            }
            else
            {
                MessageBox.Show("Pricing failed to publish. No API Key was found", "Publish Failure");
            }
        }

        private void priceLevelsDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (priceLevelsDataSource.SelectedItem != null)
            {
                string dsnName = priceLevelsDataSource.SelectedItem.ToString();
                var mapping = new Mapping(dsnName);
                List<string> tableNames = mapping.GetTableNames();

                priceLevelsTableName.Items.Clear();
                foreach (string tableName in tableNames)
                {
                    priceLevelsTableName.Items.Add(tableName);
                }

                DisplayActivePriceLevelsTableMapping();
                SetupPriceLevelsMappingFields();
            }
        }

        private void priceLevelsTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var priceLevels = new PriceLevels();
            if (priceLevelsTableName.SelectedItem != null)
            {
                string dsnName = priceLevelsDataSource.SelectedItem.ToString();
                string tableName = priceLevelsTableName.SelectedItem.ToString();
                priceLevels.SaveTableMapping(dsnName, tableName);

                DisplayActivePriceLevelsTableMapping();
                SetupPriceLevelsMappingFields();
            }
            else
            {
                MessageBox.Show("Please select your price levels table!", "No Price Levels Table Selected");
            }
        }

        private void priceLevelFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((priceLevelFields.SelectedItem as ListItem).Value != null)
            {
                DisplayActivePriceLevelsFieldMapping((priceLevelFields.SelectedItem as ListItem).Display, (priceLevelFields.SelectedItem as ListItem).Value);

                DisplayPriceLevelsFieldDescription("PriceLevels", (priceLevelFields.SelectedItem as ListItem).Value);
            }
        }

        private void previewPriceLevelsMappingOutput_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Mapping.GetTableMapping("PriceLevels")))
            {
                string mappedDsnName = Mapping.GetDsnName("PriceLevels");
                MappingPreview mappingPreview = new MappingPreview("PriceLevels", mappedDsnName);
                mappingPreview.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please setup a table mapping.", "No Table Mapping");
            }
        }

        private void mapPriceLevelsFields_Click(object sender, EventArgs e)
        {
            if (priceLevelFields.SelectedItem != null && mappingPriceLevelFields.SelectedItem != null)
            {
                var priceLevels = new PriceLevels();
                priceLevels.SaveFieldMapping((priceLevelFields.SelectedItem as ListItem).Value, mappingPriceLevelFields.SelectedItem.ToString());
                DisplayActivePriceLevelsFieldMapping((priceLevelFields.SelectedItem as ListItem).Display, (priceLevelFields.SelectedItem as ListItem).Value);
                SetupPriceLevelsMappingFields();
            }
            else
            {
                MessageBox.Show("Please select a required field followed by one of your fields.", "Both Fields Are Required"); 
            }
        }

        private void emptyPriceLevelsTransferTable_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var priceLevels = new PriceLevels();
                if (priceLevels.Empty())
                {
                    MessageBox.Show("Price Levels transfer table emptied.", "Emptied Successfully");
                }
            }
        }

        private void migratePriceLevelsData_Click(object sender, EventArgs e)
        {
            var priceLevels = new PriceLevels();
            priceLevels.UpdateTemporaryTables();
            
            string mappedDsnName = Mapping.GetDsnName("PriceLevels");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("PriceLevels"))
            {
                MessageBox.Show("Price levels migrated successfully.", "Price Levels Migrated");
                Logger.Instance.Debug($"Price Levels migrated using DSN: {mappedDsnName}");
            }
            else
            {
                MessageBox.Show("Failed to migrate price levels.", "Migration Failed");
                Logger.Instance.Warning("Failed to migrate price levels.");
            }
        }

        private void publishPriceLevels_Click(object sender, EventArgs e)
        {
            var priceLevels = new PriceLevels();
            if (priceLevels.Publish())
            {
                MessageBox.Show("Price Levels Published", "Success");
            }
            else
            {
                MessageBox.Show("Price levels failed to publish. No API Key was found", "Publish Failure");
            }
        }
    }
}
