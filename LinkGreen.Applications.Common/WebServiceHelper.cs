using LinkGreen.Applications.Common.Model;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;

namespace LinkGreen.Applications.Common
{
    public class WebServiceHelper
    {
        protected static RestClient Client;
        protected static RestClient NewApiClient;
        protected static RestClient ClientIntegrationApiClient;
        public static string Key { get; set; }
        protected static string BaseUrl;
        protected static string NewApiBaseUrl;
        protected static string ClientIntegrationApiBaseUrl;
        protected static string OrderStatuses;

        private static WebServiceHelper _instance = new WebServiceHelper();

        public WebServiceHelper()
        {
            Key = ConfigurationManager.AppSettings["ApiKey"];
            BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            NewApiBaseUrl = ConfigurationManager.AppSettings["NewApiBaseUrl"];
            ClientIntegrationApiBaseUrl = ConfigurationManager.AppSettings["ClientIntegrationApiBaseUrl"];
            OrderStatuses = ConfigurationManager.AppSettings["OrderStatuses"];
            Client = new RestClient(BaseUrl)
            {
                Timeout = (int)new TimeSpan(0, 2, 0).TotalMilliseconds,
                ReadWriteTimeout = (int)new TimeSpan(0, 2, 0).TotalMilliseconds
            };

            NewApiClient = new RestClient(NewApiBaseUrl)
            {
                Timeout = (int)new TimeSpan(0, 2, 0).TotalMilliseconds,
                ReadWriteTimeout = (int)new TimeSpan(0, 2, 0).TotalMilliseconds
            };
            NewApiClient.AddDefaultHeader("Authorization", $"Bearer {Key}");
            NewApiClient.UserAgent = "OrderEaseAsync/1.5.0";

            ClientIntegrationApiClient = new RestClient(ClientIntegrationApiBaseUrl)
            {
                Timeout = (int)new TimeSpan(0, 2, 0).TotalMilliseconds,
                ReadWriteTimeout = (int)new TimeSpan(0, 2, 0).TotalMilliseconds
            };
            ClientIntegrationApiClient.AddDefaultHeader("Authorization", $"Bearer {Key}");
            ClientIntegrationApiClient.UserAgent = "OrderEaseAsync/1.5.0";

        }

        public static UserAndCompany GetUserInfoByApiKey(string apiKey)
        {
            var requestUrl = $"UserRemoteService/rest/GetUserInfoByApiKey/{apiKey}";
            var request = new RestRequest(requestUrl, Method.GET)
            {
                RequestFormat = DataFormat.Json
            };

            var response = Client.Execute<ApiResult<UserAndCompany>>(request);

            if (string.IsNullOrEmpty(response.Content))
                return null;

            var user = JsonConvert.DeserializeObject<UserAndCompany>(response.Content);
            return user;
        }

        public static Order DownloadOrderDetails(int id)
        {
            var requestUrl = $"Order/GetForSupplierWithDetails/{id}";
            var request = new RestRequest(requestUrl, Method.GET);

            var response = NewApiClient.Execute<ApiResult<Order>>(request);

            return response.Data.Result == null ? null : response.Data.Item;
        }

        public static List<InventoryItemResponse> GetAllInventory()
        {
            var requestUrl = "SupplierInventorySearch/GetAll";
            var request = new RestRequest(requestUrl, Method.GET);

            var response = NewApiClient.Execute<ApiResult<List<InventoryItemResponse>>>(request);

            return response?.Data?.Result == null ? new List<InventoryItemResponse>() : response.Data.Item;
        }

        public static List<PrivateCategory> GetAllCategories()
        {
            var requestUrl = "Category/GetAll";
            var request = new RestRequest(requestUrl, Method.GET);

            var response = NewApiClient.Execute<ApiResult<List<PrivateCategory>>>(request);

            return response?.Data?.Result == null ? new List<PrivateCategory>() : response.Data.Item;
        }

        public static List<Supplier> GetAllSuppliers()
        {
            var requestUrl = $"buyersupplierservice/rest/listsuppliers/{Key}";
            var request = new RestRequest(requestUrl, Method.GET);

            var response = NewApiClient.Execute<ApiResult<List<Supplier>>>(request);

            return response?.Data?.Result == null ? new List<Supplier>() : response.Data.Item;
        }

        public static List<SupplierInventory> GetSupplierInventory(int supplierId)
        {
            var requestUrl = $"BuyerSupplier/SupplierInventory/{Key}/{supplierId}";
            var request = new RestRequest(requestUrl, Method.GET);
            var response = NewApiClient.Execute<ApiResult<List<SupplierInventory>>>(request);

            if (response?.Data?.Result == null) return new List<SupplierInventory>();

            var inventory = response.Data.Item;
            return inventory;
        }

        public static void UpdateSupplierContactInfo(Supplier supplier, string supplierNumber)
        {
            var requestUrl = $"buyersupplierservice/rest/updatesuppliercontactinfo/{Key}";
            var request = new RestRequest(requestUrl, Method.POST);
            var body = supplier.OurContactInfo.Clone();
            body.OurSupplierNumber = supplierNumber;
            request.AddJsonBody(body);
            var response = Client.Execute<ApiResult<SupplierContact>>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Error updating Supplier contact info");
            }
        }

        public static void UpdateSupplierInventory(SupplierInventory inventory, string buyerLinkedSku)
        {
            var requestUrl = $"buyerinventoryservice/rest/linkitem/{Key}";
            var request = new RestRequest(requestUrl, Method.POST);
            var body = new
            {
                BuyerSKU = buyerLinkedSku,
                SupplierId = inventory.SupplierId.Value,
                SupplierSKU = inventory.SupplierSku
            };
            request.AddJsonBody(body);
            var response = Client.Execute<ApiResult<BuyerInventoryLinkItemResult>>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Error linking the buyer inventory item");
            }
        }

        public static void UpdateInventoryItemQuantity(string sku, int newQty, string catalog)
        {
            var requestUrl = $"/SupplierInventoryService/rest/UpdateProductQuantity/{Key}/{sku}/{newQty}";
            if (!string.IsNullOrEmpty(catalog))
            {
                requestUrl += $"/{catalog}";
            }
            var request = new RestRequest(requestUrl, Method.POST);

            var response = Client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Error updating inventory item quantity");
        }

        public static List<PricingLevel> GetExistingPricingLevels()
        {
            var requestUrl = "SupplierInventory/GetPricingLevels";
            var request = new RestRequest(requestUrl, Method.GET);

            var response = NewApiClient.Execute<ApiResult<List<PricingLevel>>>(request);

            return response.Data?.Result == null ? null : response.Data.Item;
        }

        public static bool PushPricingLevel(PricingLevelRequest item)
        {
            var requestUrl = "SupplierInventory/AddPricingLevelItem";
            var request = new RestRequest(requestUrl, Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");

            var settings = new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat };
            var json = JsonConvert.SerializeObject(item, settings);

            request.AddParameter("application/json", json, null, ParameterType.RequestBody);

            var response = NewApiClient.Execute(request);

            return response.StatusCode == HttpStatusCode.OK;
        }

        //public static bool PushPricingLevelPrice(PricingLevelItemRequest item)
        //{
        //    var requestUrl = $"/SupplierInventoryService/rest/AddPricingLevelPrice/{Key}";
        //    var request = new RestRequest(requestUrl, Method.POST);
        //    request.RequestFormat = DataFormat.Json;

        //    request.AddHeader("Content-Type", "application/json");

        //    var settings = new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat };
        //    var json = JsonConvert.SerializeObject(item, settings);

        //    request.AddParameter("application/json", json, null, ParameterType.RequestBody);

        //    var response = Client.Execute(request);

        //    return response.StatusCode == HttpStatusCode.OK;
        //}

        public static bool PushPricingLevel(string pricingLevelName, PricingLevelItemRequest[] inventoryItems,
            DateTime effectiveDate, DateTime? endDate = null)
        {
            var requestUrl = "SupplierInventory/AddPricingLevelItem";
            var request = new RestRequest(requestUrl, Method.POST);

            var body = new PricingLevelRequest
            {
                Name = pricingLevelName,
                EffectiveDate = effectiveDate,
                EndDate = null,
                InventoryItems = inventoryItems
            };

            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");

            var settings = new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat };
            var json = JsonConvert.SerializeObject(body, settings);

            request.AddParameter("application/json", json, null, ParameterType.RequestBody);

            var response = NewApiClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                //lock (_debugLock) {
                //    System.IO.File.AppendAllLines("C:\\temp\\publishlog.txt",
                //        new[] {response.StatusCode + ":" + response.Content});
                //}
            }

            return response.StatusCode == HttpStatusCode.OK;
        }

        public static bool PushInventoryQuantity(List<IdSkuQuantity> items)
        {
            var requestUrl = "SupplierInventory/UpdateProductQuantityBulk";
            var request = new RestRequest(requestUrl, Method.POST);

            request.AddJsonBody(items);
            var response = NewApiClient.Execute(request);

            return response.StatusCode == HttpStatusCode.OK;
        }

        public static bool PushBuyerInventory(dynamic item)
        {
            var requestUrl = $"/BuyerInventoryService/rest/AddOrUpdateItem/{Key}";

            var request = new RestRequest(requestUrl, Method.POST);

            request.AddJsonBody(item);

            var response = Client.Execute(request);

            return response.StatusCode == HttpStatusCode.OK;
        }

        public static OperationResult<int> AddOrUpdateBuyer(CompanyAndRelationshipResult buyer)
        {
            var requestUrl = $"Relationship/AddOrUpdateCompany/{buyer.OurCompanyNumber}";
            var request = new RestRequest(requestUrl, Method.POST);
            var requestBody = new AddCompanyRequest
            {
                OurCompanyNumber = buyer.OurCompanyNumber,
                ContactName = buyer.ContactName?.Trim(),
                OurBillToNumber = buyer.OurBillToNumber,
                Company = new CompanyViewModel
                {
                    Address1 = buyer.Address1.Trim(),
                    Address2 = buyer.Address2?.Trim(),
                    City = buyer.City?.Trim(),
                    CommonName = buyer.Name?.Trim(),
                    CompanyTypeId = 1,
                    IndustryTypeId = 1,
                    Contact1 = buyer.Contact1?.Trim(),
                    Contact2 = buyer.Contact2?.Trim(),
                    Country = buyer.Country?.Trim(),
                    Email1 = buyer.Email1?.Trim(),
                    FormattedPhone1 = buyer.FormattedPhone1?.Trim(),
                    IsBuyer = true,
                    PostalCode = buyer.PostalCode,
                    ProvState = buyer.ProvState?.Trim(),
                    Web = buyer.Web?.Trim(),
                    Name = buyer.Name?.Trim()
                },
                SalesRepEmail = buyer.SalesRepEmail,
                ReplaceSalesRep = true,
                ForceRemoveSalesRep = false
            };
            request.AddJsonBody(requestBody);
            var response = NewApiClient.Execute<OperationResult<int>>(request);
            if (response.StatusCode != HttpStatusCode.OK && response.Data.Result == 0)
                throw new Exception("Error inviting buyers: " + response.ErrorException?.Message);

            return response.Data;
        }

        public static List<SupplierBuyerGroupBuyerParticipationRemoteModel> GetAllBuyerGroups()
        {
            var requestUrl = "Relationship/GetAllBuyerGroups";
            var request = new RestRequest(requestUrl, Method.GET);
            var response = NewApiClient.Execute<OperationResult<List<SupplierBuyerGroupBuyerParticipationRemoteModel>>>(request);
            return response.Data.Success ? response.Data.Result : null;
        }

        public static string AddBuyerToGroup(int buyerId, int[] groupIds)
        {
            var requestUrl = $"Relationship/AddBuyerToGroups/{buyerId}";
            var body = groupIds;
            var request = new RestRequest(requestUrl, Method.POST);
            request.AddJsonBody(body);
            var response = NewApiClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Error inviting buyers: " + response.ErrorException?.Message);

            return response.Content;
        }

        public static string InviteBuyers(List<CompanyAndRelationshipResult> buyers)
        {
            var requestUrl = $"/RelationshipService/rest/Import/{Key}?suppressMessages=true&mode=1";
            var request = new RestRequest(requestUrl, Method.POST);
            var companies = buyers.Select(b => new
            {
                b.Name,
                b.FormattedPhone1,
                CompanyType = "Retail",
                b.Address1,
                Address2 = b.Address2 ?? string.Empty,
                b.City,
                b.Country,
                b.ProvState,
                b.PostalCode,
                Contact1 = b.Contact1 ?? string.Empty,
                b.OurCompanyNumber,
                b.OurBillToNumber,
                b.Email1,
                BuyerGroupOne = b.BuyerGroup,

                PrivateContactName = b.ContactName,
                PrivateContactEmail = b.ContactEmail,
                PrivateContactPhone = b.ContactPhone,
                b.UserDefinedField1,
                b.UserDefinedField2,
                b.UserDefinedField3,
                b.UserDefinedField4
            }).ToArray();

            request.AddJsonBody(companies);
            // ReSharper disable once UnusedVariable
            var body = JsonConvert.SerializeObject(companies);

            var response = Client.Execute(request);



            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Error inviting buyers: " + response.ErrorException?.Message);

            return response.Content;
        }

        public static PrivateCategory PushCategory(CreateCategoryRequest category)
        {
            var requestUrl = "Category/Add";
            var request = new RestRequest(requestUrl, Method.POST);
            request.AddJsonBody(category);

            var response = NewApiClient.Execute<int>(request);

            return response?.Data == null ? null : new PrivateCategory
            {
                Id = response.Data,
                Name = category.data,
                Depth = category.Depth,
                ParentCategoryId = category.ParentCategoryId
            };
        }

        public static List<OrderStatus> GetAllOrderStatuses()
        {
            var requestUrl = "OrderStatus/ForSupplier";
            var request = new RestRequest(requestUrl, Method.GET);
            var response = NewApiClient.Execute<ApiResult<List<OrderStatus>>>(request);

            return response.Data?.Result == null ? null : response.Data.Item;
        }

        public static List<OrderFromLinkGreen> GetOrdersForStatus(int[] status)
        {
            var requestUrl = $"Order/GetByStatusForSeller?{string.Join("&", status.Select(statusItem => $"statusIds={statusItem}"))}";

            var request = new RestRequest(requestUrl, Method.GET);

            var response = NewApiClient.Execute<List<OrderFromLinkGreen>>(request);

            if (!response.IsSuccessful || response.Data == null)
            {
                throw new Exception("Error retrieving orders: " + response.ErrorException?.Message);
            }

            return response.Data;
        }

        public static int ImportProducts(ProductImportRequest productImportRequest)
        {
            var requestUrl = "import/type/1";

            var request = new RestRequest(requestUrl, Method.POST);

            request.AddJsonBody(productImportRequest);

            var response = ClientIntegrationApiClient.Execute<int>(request);

            return response.IsSuccessful ? response.Data : 0;
        }

        public static int ImportCustomers(CompanyImportRequest companyImportRequest)
        {
            var requestUrl = "import/type/0";

            var request = new RestRequest(requestUrl, Method.POST);

            request.AddJsonBody(companyImportRequest);

            var response = ClientIntegrationApiClient.Execute<int>(request);

            return response.IsSuccessful ? response.Data : 0;
        }

    }
}