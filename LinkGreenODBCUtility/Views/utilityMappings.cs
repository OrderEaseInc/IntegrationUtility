using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LinkGreenODBCUtility
{
    public partial class UtilityMappings : Form
    {
        public static Mapping Mapping = new Mapping();
        public string DsnName = Settings.ConnectViaDsnName;
        enum SettingsTab
        {
            Categories = 0,
            Customers = 1,
            Products = 2,
            InventoryQuantities = 3,
            PriceLevels = 4,
            PriceLevelPrices = 5,
            Suppliers = 6,
            SupplierInventories = 7,
            LinkedSkus = 8,
            BuyerInventories = 9
        }

        public UtilityMappings()
        {
            InitializeComponent();
            InitDoubleClickMaps();
        }

        private void UtilityLoad(object sender, EventArgs e)
        {
            // Add the dsn data source names to the list box
            List<string> odbcDataSourceNames = GetOdbcDataSources();
            foreach (var sourceName in odbcDataSourceNames)
            {
                if (sourceName != Settings.ConnectViaDsnName)
                {
                    categoriesDataSource.Items.Add(sourceName);
                    customersDataSource.Items.Add(sourceName);
                    productsDataSource.Items.Add(sourceName);
                    inventoryQuantityDataSource.Items.Add(sourceName);
                    priceLevelsDataSource.Items.Add(sourceName);
                    pricingDataSource.Items.Add(sourceName);
                    suppliersDataSource.Items.Add(sourceName);
                    supplierInventoryDataSource.Items.Add(sourceName);
                    linkedSkusDataSource.Items.Add(sourceName);
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

            switch ((SettingsTab)Tables.SelectedIndex)
            {
                case SettingsTab.Categories:
                    cusIdx = customersDataSource.FindString(mapping.GetDsnName("Categories"));
                    if (cusIdx != -1)
                    {
                        categoriesDataSource.SetSelected(cusIdx, true);
                    }
                    DisplayActiveTableMapping();
                    break;
                case SettingsTab.Customers:
                    cusIdx = customersDataSource.FindString(mapping.GetDsnName("Customers"));
                    if (cusIdx != -1)
                    {
                        customersDataSource.SetSelected(cusIdx, true);
                    }
                    DisplayActiveCustomerTableMapping();
                    break;
                case SettingsTab.Products:
                    cusIdx = productsDataSource.FindString(mapping.GetDsnName("Products"));
                    if (cusIdx != -1)
                    {
                        productsDataSource.SetSelected(cusIdx, true);
                    }

                    chkUpdateExistingProducts.Checked = Settings.GetUpdateExistingProducts();
                    DisplayActiveProductTableMapping();
                    break;
                case SettingsTab.InventoryQuantities:
                    cusIdx = productsDataSource.FindString(mapping.GetDsnName("InventoryQuantities"));
                    if (cusIdx != -1)
                    {
                        inventoryQuantityDataSource.SetSelected(cusIdx, true);
                    }
                    DisplayActiveInventoryQuantityTableMapping();
                    break;
                case SettingsTab.PriceLevels:
                    cusIdx = priceLevelsDataSource.FindString(mapping.GetDsnName("PriceLevels"));
                    if (cusIdx != -1)
                    {
                        priceLevelsDataSource.SetSelected(cusIdx, true);
                    }
                    DisplayActivePriceLevelsTableMapping();
                    break;
                case SettingsTab.PriceLevelPrices:
                    cusIdx = pricingDataSource.FindString(mapping.GetDsnName("PriceLevelPrices"));
                    if (cusIdx != -1)
                    {
                        pricingDataSource.SetSelected(cusIdx, true);
                    }
                    DisplayActivePricingTableMapping();
                    break;
                case SettingsTab.Suppliers:
                    cusIdx = suppliersDataSource.FindString(mapping.GetDsnName("Suppliers"));
                    if (cusIdx != -1)
                    {
                        suppliersDataSource.SetSelected(cusIdx, true);
                    }
                    DisplayActiveSupplierTableMapping();
                    break;
                case SettingsTab.SupplierInventories:
                    cusIdx = supplierInventoryDataSource.FindString(mapping.GetDsnName("SupplierInventories"));
                    if (cusIdx != -1)
                    {
                        supplierInventoryDataSource.SetSelected(cusIdx, true);
                    }
                    DisplayActiveSupplierInventoryTableMapping();
                    break;
                case SettingsTab.LinkedSkus:
                    cusIdx = supplierInventoryDataSource.FindString(mapping.GetDsnName("LinkedSkus"));
                    if (cusIdx != -1)
                    {
                        linkedSkusDataSource.SetSelected(cusIdx, true);
                    }
                    DisplayActiveLinkedSkusTableMapping();
                    break;
                case SettingsTab.BuyerInventories:
                    cusIdx = buyerInventoryDataSource.FindString(mapping.GetDsnName("BuyerInventories"));
                    if (cusIdx != -1)
                    {
                        buyerInventoryDataSource.SetSelected(cusIdx, true);
                    }
                    DisplayActiveBuyerInventoryTableMapping();
                    break;
                default:
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
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

        private void DisplayTableMapping(string tableMappingName, string dsnMappingName, Control activeValueLabel, Control mappedFieldsLabel, ListBox listTableName = null)
        {
            var mapping = new Mapping(DsnName);
            var mappedTableName = mapping.GetTableMapping(tableMappingName);
            var mappedDsnName = mapping.GetDsnName(dsnMappingName);
            // ReSharper disable once InvertIf
            if (!string.IsNullOrEmpty(mappedTableName))
            {
                var activeTableMapping = mappedDsnName + " > " + mappedTableName;
                activeValueLabel.Text = activeTableMapping;
                mappedFieldsLabel.Text = activeTableMapping;
            }

            var tableItem = listTableName?.Items.Cast<string>().FirstOrDefault(i => i == mappedTableName);
            if (tableItem != null)
            {
                listTableName.SelectedItem = tableItem;
            }
        }

        #region DisplayTableMapping - Specific cases
        private void DisplayActiveTableMapping() =>
            DisplayTableMapping("Categories", "Categories",
                activeCategoriesTableMappingValue, mappedTableFieldsLabel);


        private void DisplayActiveCustomerTableMapping() =>
            DisplayTableMapping("Customers", "Customers",
                activeCustomerTableMappingValue, mappedCustomerTableFieldsLabel);


        private void DisplayActiveProductTableMapping() =>
            DisplayTableMapping("Products", "Products",
                activeProductTableMappingValue, mappedProductTableFieldsLabel);

        private void DisplayActiveBuyerInventoryTableMapping() =>
            DisplayTableMapping("BuyerInventories", "BuyerInventories",
                activeBuyerInventoryFieldMappingValue, mappedBuyerInventoryTableFieldsLabel);


        private void DisplayActivePricingTableMapping() =>
            DisplayTableMapping("PriceLevelPrices", "PriceLevelPrices",
                activePricingTableMappingValue, mappedPricingTableFieldsLabel);

        private void DisplayActivePriceLevelsTableMapping() =>
            DisplayTableMapping("PriceLevels", "PriceLevels",
                activePriceLevelsTableMappingValue, mappedPriceLevelsTableFieldsLabel);

        private void DisplayActiveSupplierTableMapping() =>
            DisplayTableMapping("Suppliers", "Suppliers",
                activeSupplierTableMappingValue, mappedSuppliersTableFieldsLabel, suppliersTableName);

        private void DisplayActiveSupplierInventoryTableMapping() =>
            DisplayTableMapping("SupplierInventories", "SupplierInventories",
                activeSupplierInventoryTableMappingValue, mappedSupplierInventoryTableFieldsLabel,
                supplierInventoryTableName);

        private void DisplayActiveLinkedSkusTableMapping() =>
            DisplayTableMapping("LinkedSkus", "LinkedSkus",
                activeLinkedSkusTableMappingValue, mappedLinkedSkusTableFieldsLabel,
                linkedSkusTableName);

        private void DisplayActiveInventoryQuantityTableMapping() =>
            DisplayTableMapping("InventoryQuantities", "InventoryQuantities",
                activeInventoryQuantityTableMappingValue, mappedInventoryQuantityTableFieldsLabel,
                inventoryQuantityTableName);

        #endregion

        private static void DisplayFieldMapping(string tableName, string displayName, string fieldName, Control activeFieldLabel, Control activeFieldValue)
        {
            var mappedFieldName = Mapping.GetFieldMapping(tableName, fieldName);
            activeFieldLabel.Text = displayName + @" : ";
            activeFieldValue.Text = string.IsNullOrWhiteSpace(mappedFieldName) ? "N/A" : mappedFieldName;
        }

        private void DisplayActiveFieldMapping(string displayName, string fieldName) =>
            DisplayFieldMapping("Categories", displayName, fieldName,
                activeFieldMappingLabel, activeFieldMappingValue);

        private void DisplayActiveCustomerFieldMapping(string displayName, string fieldName) =>
            DisplayFieldMapping("Customers", displayName, fieldName,
                activeCustomerFieldMappingLabel, activeCustomerFieldMappingValue);

        private void DisplayActiveSupplierFieldMapping(string displayName, string fieldName) =>
            DisplayFieldMapping("Suppliers", displayName, fieldName,
                activeSupplierFieldMappingLabel, activeSupplierFieldMappingValue);


        private void DisplayActiveLinkedSkusFieldMapping(string displayName, string fieldName) =>
            DisplayFieldMapping("LinkedSkus", displayName, fieldName,
                activeLinkedSkuFieldMappingLabel, activeLinkedSkuFieldMappingValue);

        private void DisplayActiveInventoryQuantityFieldMapping(string displayName, string fieldName) =>
            DisplayFieldMapping("InventoryQuantities", displayName, fieldName,
                activeInventoryQuantityFieldMappingLabel, activeInventoryQuantityFieldMappingValue);


        private void DisplayActiveSupplierInventoryFieldMapping(string displayName, string fieldName) =>
            DisplayFieldMapping("SupplierInventories", displayName, fieldName,
                activeSupplierInventoryFieldMappingLabel, activeSupplierInventoryFieldMappingValue);

        private void DisplayActiveProductFieldMapping(string displayName, string fieldName) =>
            DisplayFieldMapping("Products", displayName, fieldName,
                activeProductFieldMappingLabel, activeProductFieldMappingValue);

        private void DisplayActivePricingFieldMapping(string displayName, string fieldName) =>
            DisplayFieldMapping("PriceLevelPrices", displayName, fieldName,
                activePricingFieldMappingLabel, activePricingFieldMappingValue);

        private void DisplayActivePriceLevelsFieldMapping(string displayName, string fieldName) =>
            DisplayFieldMapping("PriceLevels", displayName, fieldName,
                activePriceLevelsFieldMappingLabel, activePriceLevelsFieldMappingValue);

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
            if (!string.IsNullOrEmpty(description))
            {
                supplierFieldDescription.Text = description + " (" + dataType + ")";
            }
        }

        private void DisplayLinkedSkusDescription(string tableName, string fieldName)
        {
            string dataType = Mapping.GetFieldProperty(tableName, fieldName, "DataType");
            string description = Mapping.GetFieldProperty(tableName, fieldName, "Description");
            if (!string.IsNullOrEmpty(description))
            {
                linkedSkusFieldDescription.Text = description + " (" + dataType + ")";
            }
        }

        private void DisplayInventoryQuantityDescription(string tableName, string fieldName)
        {
            string dataType = Mapping.GetFieldProperty(tableName, fieldName, "DataType");
            string description = Mapping.GetFieldProperty(tableName, fieldName, "Description");
            if (!string.IsNullOrEmpty(description))
            {
                inventoryQuantityFieldDescriptionValue.Text = description + " (" + dataType + ")";
            }
        }

        private void DisplaySupplierInventoryFieldDescription(string tableName, string fieldName)
        {
            string dataType = Mapping.GetFieldProperty(tableName, fieldName, "DataType");
            string description = Mapping.GetFieldProperty(tableName, fieldName, "Description");
            if (!string.IsNullOrEmpty(description))
            {
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
                supplierFields.DisplayMember = "Text";
                supplierFields.Items.Add(item);
            }

            //Set the available fields to map
            if (suppliersDataSource.SelectedItem != null)
            {
                string mappedTableName = Mapping.GetTableMapping("Suppliers");
                string mappedDsnName = Mapping.GetDsnName("Suppliers");

                mappingSupplierFields.Items.Clear();
                if (!string.IsNullOrEmpty(mappedTableName))
                {
                    List<string> columns = Mapping.GetColumns(mappedTableName, mappedDsnName);
                    foreach (string column in columns)
                    {
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
                requiredSupplierInventoryFields.DisplayMember = "Text";
                requiredSupplierInventoryFields.Items.Add(item);
            }

            //Set the available fields to map
            if (supplierInventoryDataSource.SelectedItem != null)
            {
                string mappedTableName = Mapping.GetTableMapping("SupplierInventories");
                string mappedDsnName = Mapping.GetDsnName("SupplierInventories");

                mappingSupplierInventoryFields.Items.Clear();
                if (!string.IsNullOrEmpty(mappedTableName))
                {
                    List<string> columns = Mapping.GetColumns(mappedTableName, mappedDsnName);
                    foreach (string column in columns)
                    {
                        mappingSupplierInventoryFields.Items.Add(column);
                    }
                }
            }
        }

        private void SetupLinkedSkusMappingFields()
        {
            //Set the required fields from access db
            List<MappingField> tableFields = Mapping.GetTableFields("LinkedSkus");
            linkedSkuFields.Items.Clear();
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
                linkedSkuFields.DisplayMember = "Text";
                linkedSkuFields.Items.Add(item);
            }

            //Set the available fields to map
            if (linkedSkusDataSource.SelectedItem != null)
            {
                string mappedTableName = Mapping.GetTableMapping("LinkedSkus");
                string mappedDsnName = Mapping.GetDsnName("LinkedSkus");

                mappingLinkedSkuFields.Items.Clear();
                if (!string.IsNullOrEmpty(mappedTableName))
                {
                    List<string> columns = Mapping.GetColumns(mappedTableName, mappedDsnName);
                    foreach (string column in columns)
                    {
                        mappingLinkedSkuFields.Items.Add(column);
                    }
                }
            }
        }

        private void SetupInventoryQuantityMappingFields()
        {
            //Set the required fields from access db
            List<MappingField> tableFields = Mapping.GetTableFields("InventoryQuantities");
            inventoryQuantityFields.Items.Clear();
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
                inventoryQuantityFields.DisplayMember = "Text";
                inventoryQuantityFields.Items.Add(item);
            }

            //Set the available fields to map
            if (inventoryQuantityDataSource.SelectedItem != null)
            {
                string mappedTableName = Mapping.GetTableMapping("InventoryQuantities");
                string mappedDsnName = Mapping.GetDsnName("InventoryQuantities");

                mappingInventoryQuantityFields.Items.Clear();
                if (!string.IsNullOrEmpty(mappedTableName))
                {
                    List<string> columns = Mapping.GetColumns(mappedTableName, mappedDsnName);
                    foreach (string column in columns)
                    {
                        mappingInventoryQuantityFields.Items.Add(column);
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
                requiredBuyerInventoryFields.DisplayMember = "Text";
                requiredBuyerInventoryFields.Items.Add(item);
            }

            //Set the available fields to map
            if (buyerInventoryDataSource.SelectedItem != null)
            {
                string mappedTableName = Mapping.GetTableMapping("BuyerInventories");
                string mappedDsnName = Mapping.GetDsnName("BuyerInventories");

                mappingBuyerInventoryFields.Items.Clear();
                if (!string.IsNullOrEmpty(mappedTableName))
                {
                    List<string> columns = Mapping.GetColumns(mappedTableName, mappedDsnName);
                    foreach (string column in columns)
                    {
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
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
            }
        }

        private void PublishCategories_Click(object sender, EventArgs e)
        {
            var categories = new Categories();
            if (categories.Publish(out var publishDetails))
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void CategoriesDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (categoriesDataSource.SelectedItem != null)
            {
                string dsnName = categoriesDataSource.SelectedItem.ToString();
                var mapping = new Mapping(dsnName);
                List<string> tableNames = mapping.GetTableNames();
                tableNames = tableNames ?? new List<string>();

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
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void requiredCategoryFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((requiredCategoryFields.SelectedItem as ListItem)?.Value != null)
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
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void migrateCategoryData_Click(object sender, EventArgs e)
        {
            string mappedDsnName = Mapping.GetDsnName("Categories");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Categories"))
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                Logger.Instance.Debug($"Categories migrated using DSN: {mappedDsnName}");
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Warning("Failed to migrate categories.");
                }
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
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void customersDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (customersDataSource.SelectedItem != null)
            {
                string dsnName = customersDataSource.SelectedItem.ToString();
                var mapping = new Mapping(dsnName);
                List<string> tableNames = mapping.GetTableNames();
                tableNames = tableNames ?? new List<string>();

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
            if (suppliersDataSource.SelectedItem != null)
            {
                string dsnName = suppliersDataSource.SelectedItem.ToString();
                var mapping = new Mapping(dsnName);
                List<string> tableNames = mapping.GetTableNames();

                suppliersTableName.Items.Clear();
                foreach (string tableName in tableNames)
                {
                    suppliersTableName.Items.Add(tableName);
                }

                DisplayActiveSupplierTableMapping();
                SetupSupplierMappingFields();
            }
        }

        private void supplierInventoryDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (supplierInventoryDataSource.SelectedItem != null)
            {
                string dsnName = supplierInventoryDataSource.SelectedItem.ToString();
                var mapping = new Mapping(dsnName);
                var tableNames = mapping.GetTableNames();

                supplierInventoryTableName.Items.Clear();
                foreach (string tableName in tableNames)
                {
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
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
            }
        }

        private void emptySuppliersTransferTable_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var suppliers = new Suppliers();
                if (suppliers.Empty())
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
            }
        }

        private void emptyBuyerInventoriesTransferTable_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var buyerInventories = new BuyerInventories();
                if (buyerInventories.Empty())
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
            }
        }

        private void emptySupplierInventoriesTransferTable_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var supplierInventories = new SupplierInventories();
                if (supplierInventories.Empty())
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
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
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void suppliersTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var suppliers = new Suppliers();
            if (suppliersTableName.SelectedItem != null)
            {
                string dsnName = suppliersDataSource.SelectedItem.ToString();
                string tableName = suppliersTableName.SelectedItem.ToString();
                suppliers.SaveTableMapping(dsnName, tableName);

                DisplayActiveSupplierTableMapping();
                SetupSupplierMappingFields();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void supplierInventoryTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var supplierInventories = new SupplierInventories();
            if (supplierInventoryTableName.SelectedItem != null)
            {
                string dsnName = supplierInventoryDataSource.SelectedItem.ToString();
                string tableName = supplierInventoryTableName.SelectedItem.ToString();
                supplierInventories.SaveTableMapping(dsnName, tableName);

                DisplayActiveSupplierInventoryTableMapping();
                SetupSupplierInventoryMappingFields();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
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
            if ((supplierFields.SelectedItem as ListItem).Value != null)
            {
                DisplayActiveSupplierFieldMapping((supplierFields.SelectedItem as ListItem).Display, (supplierFields.SelectedItem as ListItem).Value);

                DisplaySupplierFieldDescription("Suppliers", (supplierFields.SelectedItem as ListItem).Value);
            }
        }

        private void requiredSupplierInventoryFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((requiredSupplierInventoryFields.SelectedItem as ListItem).Value != null)
            {
                DisplayActiveSupplierFieldMapping((requiredSupplierInventoryFields.SelectedItem as ListItem).Display, (requiredSupplierInventoryFields.SelectedItem as ListItem).Value);

                DisplaySupplierInventoryFieldDescription("SupplierInventories", (requiredSupplierInventoryFields.SelectedItem as ListItem).Value);
            }
        }

        private void mapCustomerFields_Click(object sender, EventArgs e)
        {
            if (customerFields.SelectedItem != null && mappingCustomerFields.SelectedItem != null)
            {
                var lastMappedItem = mappingCustomerFields.SelectedIndex;
                var lastMappingField = customerFields.SelectedIndex;
                var customers = new Customers();

                customers.SaveFieldMapping((customerFields.SelectedItem as ListItem).Value, mappingCustomerFields.SelectedItem.ToString());
                DisplayActiveCustomerFieldMapping((customerFields.SelectedItem as ListItem).Display, (customerFields.SelectedItem as ListItem).Value);
                SetupCustomerMappingFields();

                mappingCustomerFields.SelectedIndex = lastMappedItem;
                customerFields.SelectedIndex = lastMappingField;
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void mapSupplierFields_Click(object sender, EventArgs e)
        {
            if (supplierFields.SelectedItem != null && mappingSupplierFields.SelectedItem != null)
            {
                var suppliers = new Suppliers();
                suppliers.SaveFieldMapping((supplierFields.SelectedItem as ListItem).Value, mappingSupplierFields.SelectedItem.ToString());
                DisplayActiveSupplierFieldMapping((supplierFields.SelectedItem as ListItem).Display, (supplierFields.SelectedItem as ListItem).Value);
                SetupSupplierMappingFields();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void mapBuyerInventoryFields_Click(object sender, EventArgs e)
        {
            if (requiredBuyerInventoryFields.SelectedItem != null && mappingBuyerInventoryFields.SelectedItem != null)
            {
                var buyerInventories = new BuyerInventories();
                buyerInventories.SaveFieldMapping((requiredBuyerInventoryFields.SelectedItem as ListItem).Value, mappingBuyerInventoryFields.SelectedItem.ToString());
                DisplayActiveSupplierFieldMapping((requiredBuyerInventoryFields.SelectedItem as ListItem).Display, (requiredBuyerInventoryFields.SelectedItem as ListItem).Value);
                SetupBuyerInventoryMappingFields();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void mapSupplierInventoryFields_Click(object sender, EventArgs e)
        {
            if (requiredSupplierInventoryFields.SelectedItem != null && mappingSupplierInventoryFields.SelectedItem != null)
            {
                var supplierInventories = new SupplierInventories();
                supplierInventories.SaveFieldMapping((requiredSupplierInventoryFields.SelectedItem as ListItem).Value, mappingSupplierInventoryFields.SelectedItem.ToString());
                DisplayActiveSupplierInventoryFieldMapping((requiredSupplierInventoryFields.SelectedItem as ListItem).Display, (requiredSupplierInventoryFields.SelectedItem as ListItem).Value);
                SetupSupplierInventoryMappingFields();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
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
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void previewSupplierMappingOutput_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Mapping.GetTableMapping("Suppliers")))
            {
                var mappedDsnName = Mapping.GetDsnName("Suppliers");
                var mappingPreview = new MappingPreview("Suppliers", mappedDsnName);
                mappingPreview.ShowDialog();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void previewSupplierInventoryMappingOutput_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Mapping.GetTableMapping("SupplierInventories")))
            {
                var mappedDsnName = Mapping.GetDsnName("SupplierInventories");
                var mappingPreview = new MappingPreview("SupplierInventories", mappedDsnName);
                mappingPreview.ShowDialog();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void previewBuyerInventoryMappingOutput_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Mapping.GetTableMapping("BuyerInventories")))
            {
                var mappedDsnName = Mapping.GetDsnName("BuyerInventories");
                var mappingPreview = new MappingPreview("BuyerInventories", mappedDsnName);
                mappingPreview.ShowDialog();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void migrateCustomerData_Click(object sender, EventArgs e)
        {
            string mappedDsnName = Mapping.GetDsnName("Customers");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Customers"))
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                Logger.Instance.Debug($"Customers migrated using DSN: {mappedDsnName}");
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Warning("Failed to migrate customers.");
                }
            }
        }

        private void migrateSupplierData_Click(object sender, EventArgs e)
        {
            string mappedDsnName = Mapping.GetDsnName("Suppliers");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("Suppliers"))
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                Logger.Instance.Debug($"Suppliers migrated using DSN: {mappedDsnName}");
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Warning("Failed to migrate suppliers.");
                }
            }
        }

        private void publishCustomers_Click(object sender, EventArgs e)
        {
            var customers = new Customers();
            if (customers.Publish(out var publishDetails))
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void syncSuppliers_Click(object sender, EventArgs e)
        {
            var suppliers = new Suppliers();
            var result = suppliers.Publish(out var publishDetails);
            if (result)
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
            else
            {
                if (!suppliers._validPushFields)
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
            }
        }

        private void syncSupplierInventory_Click(object sender, EventArgs e)
        {
            var supplierInventories = new SupplierInventories();
            var result = supplierInventories.Publish(out var publishDetails);
            if (result)
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
            else
            {
                if (!supplierInventories._validFields)
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
            }
        }

        private void publishMatchedSupplierInventorySkus_Click(object sender, EventArgs e)
        {
            var supplierInventories = new SupplierInventories();
            var result = supplierInventories.PushMatchedSkus();
            if (result)
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
            else
            {
                if (!supplierInventories._validFields)
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
            }
        }

        private void pullSuppliersFromLinkGreen_Click(object sender, EventArgs e)
        {
            var suppliers = new Suppliers();
            var result = suppliers.Download();
            if (result)
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void pullSupplierInventoriesFromLinkGreen_Click(object sender, EventArgs e)
        {
            var supplierInventories = new SupplierInventories();
            var result = supplierInventories.Download();
            if (result)
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void productsDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (productsDataSource.SelectedItem != null)
            {
                string dsnName = productsDataSource.SelectedItem.ToString();
                var mapping = new Mapping(dsnName);
                List<string> tableNames = mapping.GetTableNames();
                tableNames = tableNames ?? new List<string>();

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
            if (buyerInventoryDataSource.SelectedItem != null)
            {
                string dsnName = buyerInventoryDataSource.SelectedItem.ToString();
                var mapping = new Mapping(dsnName);
                List<string> tableNames = mapping.GetTableNames();

                buyerInventoryTableName.Items.Clear();
                foreach (string tableName in tableNames)
                {
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
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void buyerInventoryTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var buyerInventories = new BuyerInventories();
            if (buyerInventoryTableName.SelectedItem != null)
            {
                string dsnName = buyerInventoryDataSource.SelectedItem.ToString();
                string tableName = buyerInventoryTableName.SelectedItem.ToString();
                buyerInventories.SaveTableMapping(dsnName, tableName);

                DisplayActiveBuyerInventoryTableMapping();
                SetupBuyerInventoryMappingFields();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
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
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
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
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
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
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
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
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                Logger.Instance.Debug($"Products migrated using DSN: {mappedDsnName}");
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Warning("Failed to migrate products.");
                }
            }
        }

        private void migrateBuyerInventoryData_Click(object sender, EventArgs e)
        {
            var buyerInventories = new BuyerInventories();
            buyerInventories.UpdateTemporaryTables();


            string mappedDsnName = Mapping.GetDsnName("BuyerInventories");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("BuyerInventories"))
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                Logger.Instance.Debug($"Buyer inventory migrated using DSN: {mappedDsnName}");
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Warning("Failed to migrate inventory.");
                }
            }
        }

        private void publishProducts_Click(object sender, EventArgs e)
        {
            var products = new Products();
            if (products.Publish(out var publishDetails))
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void publishBuyerInventories_Click(object sender, EventArgs e)
        {
            var buyerInventories = new BuyerInventories();
            if (buyerInventories.Publish(out var publishDetails))
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void pricingDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pricingDataSource.SelectedItem != null)
            {
                string dsnName = pricingDataSource.SelectedItem.ToString();
                var mapping = new Mapping(dsnName);
                List<string> tableNames = mapping.GetTableNames();
                tableNames = tableNames ?? new List<string>();

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
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
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
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
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
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
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
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
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
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                Logger.Instance.Debug($"Pricing migrated using DSN: {mappedDsnName}");
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Warning("Failed to migrate pricing.");
                }
            }
        }

        private void publishPricing_Click_1(object sender, EventArgs e)
        {
            var pricing = new PriceLevelPrices();
            if (pricing.Publish(out var publishDetails))
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void priceLevelsDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (priceLevelsDataSource.SelectedItem != null)
            {
                string dsnName = priceLevelsDataSource.SelectedItem.ToString();
                var mapping = new Mapping(dsnName);
                List<string> tableNames = mapping.GetTableNames();
                tableNames = tableNames ?? new List<string>();

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
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
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
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
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
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
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
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
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
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                Logger.Instance.Debug($"Price Levels migrated using DSN: {mappedDsnName}");
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Warning("Failed to migrate price levels.");
                }
            }
        }

        private void publishPriceLevels_Click(object sender, EventArgs e)
        {
            var priceLevels = new PriceLevels();
            if (priceLevels.Publish(out var publishDetails))
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void setCategoriesUsernamePW_Click(object sender, EventArgs e)
        {
            if (categoriesDataSource.SelectedItem != null)
            {
                var DsnCredentials = new DsnCredentials(categoriesDataSource.SelectedItem.ToString());
                DsnCredentials.ShowDialog();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void setCustomersUsernamePW_Click(object sender, EventArgs e)
        {
            if (customersDataSource.SelectedItem != null)
            {
                var DsnCredentials = new DsnCredentials(customersDataSource.SelectedItem.ToString());
                DsnCredentials.ShowDialog();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void setProductsUsernamePW_Click(object sender, EventArgs e)
        {
            if (productsDataSource.SelectedItem != null)
            {
                var DsnCredentials = new DsnCredentials(productsDataSource.SelectedItem.ToString());
                DsnCredentials.ShowDialog();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void setPriceLevelsUsernamePW_Click(object sender, EventArgs e)
        {
            if (priceLevelsDataSource.SelectedItem != null)
            {
                var DsnCredentials = new DsnCredentials(priceLevelsDataSource.SelectedItem.ToString());
                DsnCredentials.ShowDialog();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void setPricingUsernamePW_Click(object sender, EventArgs e)
        {
            if (pricingDataSource.SelectedItem != null)
            {
                var DsnCredentials = new DsnCredentials(pricingDataSource.SelectedItem.ToString());
                DsnCredentials.ShowDialog();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void setSuppliersUsernamePW_Click(object sender, EventArgs e)
        {
            if (suppliersDataSource.SelectedItem != null)
            {
                var DsnCredentials = new DsnCredentials(suppliersDataSource.SelectedItem.ToString());
                DsnCredentials.ShowDialog();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void setSupplierInventoryUsernamePW_Click(object sender, EventArgs e)
        {
            if (supplierInventoryDataSource.SelectedItem != null)
            {
                var DsnCredentials = new DsnCredentials(supplierInventoryDataSource.SelectedItem.ToString());
                DsnCredentials.ShowDialog();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void setBuyerInventoryUsernamePW_Click(object sender, EventArgs e)
        {
            if (buyerInventoryDataSource.SelectedItem != null)
            {
                var DsnCredentials = new DsnCredentials(buyerInventoryDataSource.SelectedItem.ToString());
                DsnCredentials.ShowDialog();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void linkedSkusDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (linkedSkusDataSource.SelectedItem != null)
            {
                string dsnName = linkedSkusDataSource.SelectedItem.ToString();
                var mapping = new Mapping(dsnName);
                var tableNames = mapping.GetTableNames();

                linkedSkusTableName.Items.Clear();
                foreach (string tableName in tableNames)
                {
                    linkedSkusTableName.Items.Add(tableName);
                }

                DisplayActiveLinkedSkusTableMapping();
                SetupLinkedSkusMappingFields();
            }
        }

        private void linkedSkusTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var linkedSkus = new LinkedSkus();
            if (linkedSkusTableName.SelectedItem != null)
            {
                string dsnName = linkedSkusDataSource.SelectedItem.ToString();
                string tableName = linkedSkusTableName.SelectedItem.ToString();
                linkedSkus.SaveTableMapping(dsnName, tableName);

                DisplayActiveLinkedSkusTableMapping();
                SetupLinkedSkusMappingFields();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void setLinkedSkusUsernamePW_Click(object sender, EventArgs e)
        {
            if (linkedSkusDataSource.SelectedItem != null)
            {
                var dsnCredentials = new DsnCredentials(linkedSkusDataSource.SelectedItem.ToString());
                dsnCredentials.ShowDialog();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void linkedSkuFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((linkedSkuFields.SelectedItem as ListItem).Value != null)
            {
                DisplayActiveLinkedSkusFieldMapping((linkedSkuFields.SelectedItem as ListItem).Display, (linkedSkuFields.SelectedItem as ListItem).Value);

                DisplayLinkedSkusDescription("LinkedSkus", (linkedSkuFields.SelectedItem as ListItem).Value);
            }
        }

        private void previewLinkedSkusMappingOutput_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Mapping.GetTableMapping("LinkedSkus")))
            {
                var mappedDsnName = Mapping.GetDsnName("LinkedSkus");
                var mappingPreview = new MappingPreview("LinkedSkus", mappedDsnName);
                mappingPreview.ShowDialog();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void mapLinkedSkuFields_Click(object sender, EventArgs e)
        {
            if (linkedSkuFields.SelectedItem != null && mappingLinkedSkuFields.SelectedItem != null)
            {
                var linkedSkus = new LinkedSkus();
                linkedSkus.SaveFieldMapping((linkedSkuFields.SelectedItem as ListItem).Value, mappingLinkedSkuFields.SelectedItem.ToString());
                DisplayActiveLinkedSkusFieldMapping((linkedSkuFields.SelectedItem as ListItem).Display, (linkedSkuFields.SelectedItem as ListItem).Value);
                SetupLinkedSkusMappingFields();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void emptyLinkedSkusTransferTable_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var linkedSkus = new LinkedSkus();
                if (linkedSkus.Empty())
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
            }
        }

        private void publishLinkedSkus_Click(object sender, EventArgs e)
        {
            var linkedSkus = new LinkedSkus();
            var result = linkedSkus.Publish(out var publishDetails);
            if (result)
            {
                linkedSkus.Empty();
                MessageBox.Show(@"Linked SKUs have been pubished", @"Published Successfully");
            }
            else
            {
                if (!linkedSkus._validFields)
                {
                    MessageBox.Show(@"Please validate your field mappings", @"Publish Failed");
                }
                else
                {
                    MessageBox.Show(@"An unexpected error has occured, please view your logs or contact support", @"Publish Failed");
                }
            }
        }

        private void inventoryQuantityDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (inventoryQuantityDataSource.SelectedItem != null)
            {
                var dsnName = inventoryQuantityDataSource.SelectedItem.ToString();
                var mapping = new Mapping(dsnName);
                var tableNames = mapping.GetTableNames();

                inventoryQuantityTableName.Items.Clear();
                foreach (string tableName in tableNames)
                {
                    inventoryQuantityTableName.Items.Add(tableName);
                }

                DisplayActiveInventoryQuantityTableMapping();
                SetupInventoryQuantityMappingFields();
            }
        }

        private void inventoryQuantityTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var inventoryQuantity = new InventoryQuantity();
            if (inventoryQuantityTableName.SelectedItem != null)
            {
                string dsnName = inventoryQuantityDataSource.SelectedItem.ToString();
                string tableName = inventoryQuantityTableName.SelectedItem.ToString();
                inventoryQuantity.SaveTableMapping(dsnName, tableName);

                DisplayActiveInventoryQuantityTableMapping();
                SetupInventoryQuantityMappingFields();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void setInventoryQuantityUsernamePW_Click(object sender, EventArgs e)
        {
            if (inventoryQuantityDataSource.SelectedItem != null)
            {
                var DsnCredentials = new DsnCredentials(inventoryQuantityDataSource.SelectedItem.ToString());
                DsnCredentials.ShowDialog();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void inventoryQuantityFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((inventoryQuantityFields.SelectedItem as ListItem).Value != null)
            {
                DisplayActiveInventoryQuantityFieldMapping((inventoryQuantityFields.SelectedItem as ListItem).Display, (inventoryQuantityFields.SelectedItem as ListItem).Value);

                DisplayInventoryQuantityDescription("InventoryQuantities", (inventoryQuantityFields.SelectedItem as ListItem).Value);
            }
        }

        private void previewInventoryQuantity_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Mapping.GetTableMapping("InventoryQuantities")))
            {
                var mappedDsnName = Mapping.GetDsnName("InventoryQuantities");
                var mappingPreview = new MappingPreview("InventoryQuantities", mappedDsnName);
                mappingPreview.ShowDialog();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void mapInventoryQuantityFields_Click(object sender, EventArgs e)
        {
            if (inventoryQuantityFields.SelectedItem != null && mappingInventoryQuantityFields.SelectedItem != null)
            {
                var inventoryQuantity = new InventoryQuantity();
                inventoryQuantity.SaveFieldMapping((inventoryQuantityFields.SelectedItem as ListItem).Value, mappingInventoryQuantityFields.SelectedItem.ToString());
                DisplayActiveInventoryQuantityFieldMapping((inventoryQuantityFields.SelectedItem as ListItem).Display, (inventoryQuantityFields.SelectedItem as ListItem).Value);
                SetupInventoryQuantityMappingFields();
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private void emptyInventoryQuantityTransferTable_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var inventoryQuantity = new InventoryQuantity();
                if (inventoryQuantity.Empty())
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
            }
        }

        private void migrateInventoryQuantity_Click(object sender, EventArgs e)
        {
            string mappedDsnName = Mapping.GetDsnName("InventoryQuantities");
            var newMapping = new Mapping(mappedDsnName);
            if (newMapping.MigrateData("InventoryQuantities"))
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                Logger.Instance.Debug($"Inventory Quantities migrated using DSN: {mappedDsnName}");
            }
            else
            {
                if (!newMapping._validFields)
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                }
                else
                {
                    MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
                    Logger.Instance.Warning("Failed to migrate inventory quantities.");
                }
            }
        }

        private void publishInventoryQuantity_Click(object sender, EventArgs e)
        {
            var inventoryQuantities = new InventoryQuantity();
            if (inventoryQuantities.Publish(out var publishDetails))
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
            else
            {
                MessageBox.Show(@"Please select your suppliers table!", @"Emptied Successfully");
            }
        }

        private Tuple<ListBox, Action<object, EventArgs>>[] _doubleClickMaps;

        private void InitDoubleClickMaps()
        {
            _doubleClickMaps = new[] {
                new Tuple<ListBox, Action<object, EventArgs>>( mappingCategoryFields, mapCategoryFields_Click),
                new Tuple<ListBox, Action<object, EventArgs>>( mappingCustomerFields, mapCustomerFields_Click),
                new Tuple<ListBox, Action<object, EventArgs>>( mappingProductFields, mapProductFields_Click),
                new Tuple<ListBox, Action<object, EventArgs>>( mappingInventoryQuantityFields, mapInventoryQuantityFields_Click),
                new Tuple<ListBox, Action<object, EventArgs>>( mappingPriceLevelFields, mapPriceLevelsFields_Click),
                new Tuple<ListBox, Action<object, EventArgs>>( mappingPricingFields, mapPricingFields_Click),
                new Tuple<ListBox, Action<object, EventArgs>>( mappingSupplierFields, mapSupplierFields_Click),
                new Tuple<ListBox, Action<object, EventArgs>>( mappingSupplierInventoryFields, mapSupplierInventoryFields_Click),
                new Tuple<ListBox, Action<object, EventArgs>>( mappingLinkedSkuFields, mapLinkedSkuFields_Click),
                new Tuple<ListBox, Action<object, EventArgs>>( mappingBuyerInventoryFields, mapBuyerInventoryFields_Click)
            };
        }

        private void MappedListDoubleClick(object sender, EventArgs e)
        {
            if (((ListBox)sender).SelectedIndex > -1)
                if (_doubleClickMaps.FirstOrDefault(m => m.Item1 == (ListBox)sender) != null)
                    _doubleClickMaps.First(m => m.Item1 == (ListBox)sender).Item2(sender, e);
        }

        private void chkUpdateExistingProducts_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SaveUpdateExistingProducts(chkUpdateExistingProducts.Checked);
        }
    }
}
