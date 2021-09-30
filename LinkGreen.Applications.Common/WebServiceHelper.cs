using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using LinkGreen.Applications.Common.Model;
using Newtonsoft.Json;
using RestSharp;

namespace LinkGreen.Applications.Common
{
    public class WebServiceHelper
    {
        protected static RestClient Client;
        protected static RestClient NewApiClient;
        public static string Key { get; set; }
        protected static string BaseUrl;
        protected static string NewApiBaseUrl;
        protected static string OrderStatuses;

        // This line ensures that the constructor is called and the static properties are populated from the config file.
        private static WebServiceHelper _instance = new WebServiceHelper();

        public WebServiceHelper()
        {
            Key = ConfigurationManager.AppSettings["ApiKey"];
            BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            NewApiBaseUrl = ConfigurationManager.AppSettings["NewApiBaseUrl"];
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
            //NewApiClient.AddDefaultHeader("Authorization", $"Bearer {Key}");

        }

        public static OrderSummary GetLastOrderNotDownloaded()
        {
            var requestUrl = $"orderservice/rest/OldestOrderNotExported/{Key}";
            var request = new RestRequest(requestUrl, Method.POST);
            request.AddParameter("application/json; charset=utf-8", OrderStatuses, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            var response = Client.Execute<ApiResult<OrderSummary>>(request);

            if (response.Data?.Result == null)
                return null;

            var order = response.Data.Item;
            return order;
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

        public static Relationship GetRelationship(int companyId)
        {
            var requestUrl = $"/relationshipservice/rest/GetRelationship/{Key}/{companyId}";
            var request = new RestRequest(requestUrl, Method.GET);

            var response = Client.Execute<ApiResult<Relationship>>(request);

            if (response.Data.Result == null)
                return null;

            return response.Data.Item;
        }

        public static Order DownloadOrderDetails(int id)
        {
            var requestUrl = $"orderservice/rest/OrderDetails/{Key}/{id}";
            var request = new RestRequest(requestUrl, Method.GET);

            var response = Client.Execute<ApiResult<Order>>(request);

            if (response.Data.Result == null)
                return null;

            var order = response.Data.Item;
            return order;
        }

        public static void PushTaxInfo(int relationshipId, string taxes)
        {
            var requestUrl = $"/relationshipservice/rest/UpdateTax/{Key}/{relationshipId}";
            var request = new RestRequest(requestUrl, Method.POST);
            request.AddJsonBody(taxes);

            var response = Client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Error pushing taxes");
        }

        public static void PushRelationshipUpdate(Relationship relation)
        {
            var requestUrl = $"/relationshipservice/rest/UpdateDetails/{Key}/{relation.Id}";
            var request = new RestRequest(requestUrl, Method.POST);
            request.AddJsonBody(relation);

            var response = Client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Error pushing relationship updates");
        }

        public static void MarkOrderExported(int orderId)
        {
            var requestUrl = $"/orderservice/rest/SetOrderExported/{Key}/{orderId}";
            var request = new RestRequest(requestUrl, Method.POST);

            var response = Client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Error marking order as exported");
        }

        public static void UpdateOrderAccountingReference(int orderId, string accountingReference)
        {
            var requestUrl = $"/orderservice/rest/SetAccountingReference/{Key}/{orderId}";
            var request = new RestRequest(requestUrl, Method.POST);
            var data = new { AccountingReference = accountingReference };
            request.AddJsonBody(data);

            var response = Client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Error updating order accounting reference");
        }

        public static void UpdateInventoryItemAccountingReference(string sku, string accountingReference)
        {
            var requestUrl = $"/SupplierInventoryService/rest/UpdateAccountingReference/{Key}/{sku}";
            var request = new RestRequest(requestUrl, Method.POST);
            var data = new { AccountingReference = accountingReference };
            request.AddJsonBody(data);

            var response = Client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Error updating inventory item reference");
        }

        public static List<InventoryItemResponse> GetAllInventory()
        {
            var requestUrl = $"SupplierInventorySearchService/rest/{Key}";
            var request = new RestRequest(requestUrl, Method.GET);

            var response
                = Client.Execute<ApiResult<List<InventoryItemResponse>>>(request);

            if (response?.Data?.Result == null)
                return new List<InventoryItemResponse>();

            var inventory = response.Data.Item;
            return inventory;
        }

        public static List<CompanyAndRelationshipResult> GetAllCompaniesAndRelationships()
        {
            var requestUrl = $"relationshipservice/rest/CompaniesAndRelationships/{Key}";
            var request = new RestRequest(requestUrl, Method.GET);

            var response
                = Client.Execute<ApiResult<List<CompanyAndRelationshipResult>>>(request);

            if (response?.Data?.Result == null)
                return new List<CompanyAndRelationshipResult>();

            var companies = response.Data.Item;
            return companies;
        }

        public static List<PrivateCategory> GetAllCategories()
        {
            var requestUrl = $"categoryservice/rest/getall/{Key}";
            var request = new RestRequest(requestUrl, Method.GET);

            var response
                = Client.Execute<ApiResult<List<PrivateCategory>>>(request);

            if (response?.Data?.Result == null)
                return new List<PrivateCategory>();

            var cats = response.Data.Item;
            return cats;
        }

        public static List<Supplier> GetAllSuppliers()
        {
            var requestUrl = $"buyersupplierservice/rest/listsuppliers/{Key}";
            var request = new RestRequest(requestUrl, Method.GET);
            var response = Client.Execute<ApiResult<List<Supplier>>>(request);

            if (response?.Data?.Result == null) return new List<Supplier>();

            var buyers = response.Data.Item;
            return buyers;
        }

        public static List<SupplierInventory> GetSupplierInventory(int supplierId)
        {
            var requestUrl = $"buyersupplierservice/rest/supplierinventoryavailable/{Key}/{supplierId}";
            var request = new RestRequest(requestUrl, Method.GET);
            var response = Client.Execute<ApiResult<List<SupplierInventory>>>(request);

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

        public static void UpdateLinkedSku(LinkedItem item, string buyerLinkedSku)
        {
            var requestUrl = $"buyerinventoryservice/rest/linkitem/{Key}";
            var request = new RestRequest(requestUrl, Method.POST);
            var body = new
            {
                BuyerSKU = buyerLinkedSku,
                SupplierId = item.SupplierId.Value,
                SupplierSKU = item.SupplierSku
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

        public static bool PushProduct(InventoryItemRequest prod)
        {
            var requestUrl = $"/SupplierInventoryService/rest/AddItem/{Key}";
            var request = new RestRequest(requestUrl, Method.POST);

            request.AddJsonBody(prod);

            var response = Client.Execute(request);

            return response.StatusCode == HttpStatusCode.OK;
        }

        public static List<PricingLevel> GetExistingPricingLevels()
        {
            var requestUrl = $"/SupplierInventoryService/rest/GetPricingLevels/{Key}";
            var request = new RestRequest(requestUrl, Method.GET) { RequestFormat = DataFormat.Json };
            request.AddHeader("Content-Type", "application/json");


            var response = Client.Execute<ApiResult<List<PricingLevel>>>(request);

            return response.Data?.Result == null ? null : response.Data.Item;
        }

        public static bool PushPricingLevel(PricingLevelRequest item)
        {
            var requestUrl = $"/SupplierInventoryService/rest/AddPricingLevel/{Key}";
            var request = new RestRequest(requestUrl, Method.POST);
            request.RequestFormat = DataFormat.Json;

            request.AddHeader("Content-Type", "application/json");

            var settings = new JsonSerializerSettings() { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat };
            var json = JsonConvert.SerializeObject(item, settings);

            request.AddParameter("application/json", json, null, ParameterType.RequestBody);

            var response = Client.Execute(request);

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

        private static object _debugLock = new object();
        public static bool PushPricingLevel(string pricingLevelName, PricingLevelItemRequest[] inventoryItems,
            DateTime effectiveDate, DateTime? endDate = null)
        {
            var requestUrl = $"/SupplierInventoryService/rest/AddPricingLevelItem/{Key}";
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

            var response = Client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                //lock (_debugLock) {
                //    System.IO.File.AppendAllLines("C:\\temp\\publishlog.txt",
                //        new[] {response.StatusCode + ":" + response.Content});
                //}
            }

            return response.StatusCode == HttpStatusCode.OK;
        }

        public static bool PushInventoryItem(InventoryItemRequest item, out int statusCode, out string responseContent)
        {
            var requestUrl = $"/SupplierInventoryService/rest/{(item.Id > 0 ? "Update" : "Add")}Item/{Key}";

            var request = new RestRequest(requestUrl, Method.POST);

            request.AddJsonBody(item);

            var response = Client.Execute(request);

            statusCode = (int)response.StatusCode;
            responseContent = response.Content ?? "No Content";

            return response.StatusCode == HttpStatusCode.OK;
        }

        public static bool PushBulkUpdateInventoryItem(InventoryItemRequest[] item, out int statusCode, out string content)
        {
            var requestUrl = $"/SupplierInventoryService/rest/UpdateItems/{Key}";

            var request = new RestRequest(requestUrl, Method.POST);

            request.AddJsonBody(item);

            var response = Client.Execute(request);

            statusCode = (int)response.StatusCode;
            content = response.Content ?? "";

            return response.StatusCode == HttpStatusCode.OK;
        }

        public static bool PushInventoryQuantity(List<IdSkuQuantity> items)
        {
            var requestUrl = $"/SupplierInventoryService/rest/UpdateProductQuantityBulk/{Key}";
            var request = new RestRequest(requestUrl, Method.POST);

            request.AddJsonBody(items);
            var response = Client.Execute(request);

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
            var requestUrl = $"/api/Relationship/AddOrUpdateCompany/{buyer.OurCompanyNumber}";
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
                    Email2 = buyer.Email2?.Trim(),
                    IsBuyer = true,
                    PostalCode = buyer.PostalCode,
                    ProvState = buyer.ProvState?.Trim(),
                    Web = buyer.Web?.Trim(),
                    Name = buyer.Name?.Trim()
                },
                SalesRepEmail = buyer.SalesRepEmail,
                ReplaceSalesRep = true,
            };
            request.AddJsonBody(requestBody);
            NewApiClient.AddDefaultHeader("Authorization", $"Bearer {Key}");
            var response = NewApiClient.Execute<OperationResult<int>>(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Error inviting buyers: " + response.ErrorException?.Message);

            return response.Data;
        }

        public static List<SupplierBuyerGroupBuyerParticipationRemoteModel> GetAllBuyerGroups()
        {
            var requestUrl = $"/api/Relationship/GetAllBuyerGroups";
            var request = new RestRequest(requestUrl, Method.GET);
            NewApiClient.AddDefaultHeader("Authorization", $"Bearer {Key}");
            var response =
                NewApiClient.Execute<OperationResult<List<SupplierBuyerGroupBuyerParticipationRemoteModel>>>(request);
            return response.Data.Success ? response.Data.Result : null;
        }

        public static string AddBuyerToGroup(int buyerId, int groupId)
        {
            var requestUrl = $"/api/Relationship/AddBuyerToGroups/{buyerId}";
            var body = new[] { groupId };
            var request = new RestRequest(requestUrl, Method.POST);
            NewApiClient.AddDefaultHeader("Authorization", $"Bearer {Key}");
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
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(companies);

            var response = Client.Execute(request);



            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception("Error inviting buyers: " + response.ErrorException?.Message);

            return response.Content;
        }

        public static PrivateCategory PushCategory(PrivateCategory category)
        {
            try
            {

                var requestUrl = $"/CategoryService/rest/Add/{Key}";

                var request = new RestRequest(requestUrl, Method.POST);

                request.AddJsonBody(category);
                var response = Client.Execute<ApiResult<PrivateCategory>>(request);

                if (response.Data.Result == null)
                    return null;

                return response.Data.Item;
            }
            catch (Exception ex)
            {
                throw new Exception(JsonConvert.SerializeObject(category), ex);
            }
        }

        public static void PostInventoryImport()
        {
            Client.Execute(new RestRequest($"/SupplierInventoryService/rest/PostInventoryImport/{Key}", Method.POST));
        }

        public static List<OrderStatus> GetAllOrderStatuses()
        {
            var requestUrl = $"/OrderService/rest/Status/{Key}";
            var request = new RestRequest(requestUrl, Method.GET);

            var response = Client.Execute<ApiResult<List<OrderStatus>>>(request);

            if (response.Data?.Result == null) return null;

            return response.Data.Item;
        }

        public static List<OrderFromLinkGreen> GetOrdersForStatus(int status)
        {
            var requestUrl = $"/OrderService/rest/GetForSupplier/{Key}/50";
            var request = new RestRequest(requestUrl, Method.POST);
            var statuses = new[] { status };
            var json = JsonConvert.SerializeObject(statuses);
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            var response = Client.Execute<ApiResult<List<OrderFromLinkGreen>>>(request);

            if (!response.IsSuccessful || response.Data?.Result == null)
            {
                throw new Exception("Error retrieving orders: " + response.ErrorException?.Message);
            }

            return response.Data.Item;
        }
    }
}