using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BigScreenDataShow.Helper
{
    /// <summary>
    /// 使用HTTP访问光伏平台获取所有数据 然后进行展示
    /// </summary>
    public class RestSharpHelper
    {
        /// <summary>
        /// "http://localhost:8051/api/Category/GetList?parentId=" + parentId
        /// <code><![CDATA[
        /// var json = ERPServiceHelper.GetProductBySKU(CurrentUser.Token, sku, CurrentUser.ShopUserId);
        /// var response = JsonConvert.DeserializeObject<ResponseModel<product_flat>>(json);
        /// ]]></code>
        /// </summary>
        /// <param name="token">可空</param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<string> ExecuteAsync(string url, string token = null, Method method = Method.Get)
        {
            var client = new RestClient(url);
            var request = new RestRequest();
            request.Timeout = TimeSpan.FromSeconds(3);
            request.Method = method;
            if (!string.IsNullOrEmpty(token))
                request.AddHeader("Authorization", token);
            var response = await client.ExecuteAsync(request);
            var json = response.Content;

            //jsonObj.expiration = DateTime.Now.AddHours(24);
            //Authority = jsonObj;
            //return jsonObj.token;
            return json;
            //}
            //return Authority.token;
            //Console.WriteLine(response.Content);
        }
        /// <summary>
        /// "http://localhost:8051/api/Category/GetList?parentId=" + parentId
        /// <code><![CDATA[
        /// await RestSharpHelper.Get<ResponseModel<Sys_Module>>(
        /// await RestSharpHelper.Get<ResponseModel<List<Sys_Module>>>(
        /// await RestSharpHelper.Get<ResponseModel>(
        /// ]]></code>
        /// </summary>
        /// <typeparam name="ReturnType"><![CDATA[
        /// 返回类型可以是ResponseModel ResponseModel<Sys_Module> ResponseModel<List<Sys_Module>> 
        /// ]]></typeparam>
        /// <param name="token"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<ReturnType> ExecuteAsync<ReturnType>(string url, string token = null, Method method = Method.Get)
        {
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest();
                request.Timeout = TimeSpan.FromSeconds(3);
                request.Method = method;
                if (!string.IsNullOrEmpty(token))
                    request.AddHeader("Authorization", token);
                var response = await client.ExecuteAsync<ReturnType>(request);
                //if (response.ErrorException != null)
                //{
                //    const string message = "Error retrieving response.  Check inner details for more info.";
                //    var twilioException = new ApplicationException(message, response.ErrorException);
                //    throw twilioException;
                //}
                /*
                var json = response.Content;
                var responseObject = JsonConvert.DeserializeObject<ReturnType>(json);
                return responseObject;
                */
                return response.Data;
            }
            catch (Exception ex)
            {
                return default(ReturnType);
            }
        }
        /// <summary>
        /// 不知道返回类型用这个
        /// </summary>
        /// <param name="t"></param>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static async Task<string> ExecuteAsync<T>(T t, string url, Method method = Method.Post)
        {
            var client = new RestClient(url);
            var request = new RestRequest();
            request.Timeout = TimeSpan.FromSeconds(3);
            request.Method = method;
            //request.AddHeader("Authorization", token);
            //client.UserAgent = "apifox/1.0.0 (https://www.apifox.cn)";
            request.AddHeader("Content-Type", "application/json");
            string json = JsonConvert.SerializeObject(t);
            //var body = @"{
            //                ""data"":" + json + @"
            //               }";
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);
            return response.Content;
            //Console.WriteLine(response.Content);
        }
        /// <summary>
        /// 无需序列化直接传入现有json
        /// </summary>
        /// <typeparam name="ReturnType">返回结果转为对象</typeparam>
        /// <param name="json">可以把Model或List转为json</param>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static async Task<ReturnType> ExecuteAsync<ReturnType>(string json, string url, string token = null, Method method = Method.Post)
        {
            var client = new RestClient(url);
            var request = new RestRequest();
            request.Timeout = TimeSpan.FromSeconds(3);
            request.Method = method;
            if (!string.IsNullOrEmpty(token))
                request.AddHeader("Authorization", token);
            //client.UserAgent = "apifox/1.0.0 (https://www.apifox.cn)";
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            //var response = await client.ExecuteAsync(request);
            //var responModel = JsonConvert.DeserializeObject<ReturnType>(response.Content);
            //return responModel;
            //Console.WriteLine(response.Content);

            var response = await client.ExecuteAsync<ReturnType>(request);
            return response.Data;
        }
        /// <summary>
        /// 无需序列化直接传入现有json
        /// </summary>
        /// <typeparam name="ReturnType">返回结果转为对象</typeparam>
        /// <param name="json">可以把Model或List转为json</param>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static async Task<ReturnType> ExecuteAsync<ReturnType>(Dictionary<string, string> bodyParas, string url, string token = null, Method method = Method.Post, string contentType = "x-www-form-urlencoded")
        {
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest();
                request.Timeout = TimeSpan.FromSeconds(8);
                request.Method = method;
                if (!string.IsNullOrEmpty(token))
                    request.AddHeader("Authorization", token);
                //client.UserAgent = "apifox/1.0.0 (https://www.apifox.cn)";
                if (contentType == "json")
                {
                    request.AddHeader("Content-Type", "application/json");
                    string json = JsonConvert.SerializeObject(bodyParas);
                    request.AddParameter("application/json", json, ParameterType.RequestBody);
                }
                else if (contentType == "xml")
                {
                    //request.AddHeader("Content-Type", "application/xml");
                    //request.AddParameter("application/xml",value, ParameterType.RequestBody);
                }
                else if (contentType == "x-www-form-urlencoded")
                {
                    request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    foreach (var item in bodyParas)
                        request.AddParameter(item.Key, item.Value);
                }
                else if (contentType == "form-data")
                {
                    request.AddHeader("Content-Type", "multipart/form-data");
                    request.AlwaysMultipartFormData = true;
                    foreach (var item in bodyParas)
                        request.AddParameter(item.Key, item.Value);
                }
                else if (contentType == "raw")
                {
                    //request.AddParameter("text/plain",value, ParameterType.RequestBody);
                }
                else if (contentType == "none")
                {

                }
                //var response = await client.ExecuteAsync(request);
                //var responModel = JsonConvert.DeserializeObject<ReturnType>(response.Content);
                //return responModel;
                //Console.WriteLine(response.Content);
                var response = await client.ExecuteAsync<ReturnType>(request).ConfigureAwait(false);
                return response.Data;
            }
            catch (Exception ex)
            { 
                return default(ReturnType);
            }
        }
        /// <summary>
        /// t可以是Model或List
        /// <code><![CDATA[
        /// await RestSharpHelper.Post<ResponseModel<Product>>()
        /// await RestSharpHelper.Post<ResponseModel<List<Product>>>()
        /// await RestSharpHelper.Post<ResponseModel>()
        /// 接收参数 
        /// public async Task<ResponseModel<Double>> BatchUpdatePrice(List<Product> list)
        /// public async Task<ResponseModel<Double>> BatchUpdatePrice(Product model)
        /// ]]></code>
        /// </summary>
        /// <typeparam name="ReturnType">返回实体类</typeparam>
        /// <param name="t">提交数据参数,可以是Model或List</param>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static async Task<ReturnType> ExecuteAsync<ReturnType, T>(T t, string url, string token, Method method = Method.Post)
        {
            var client = new RestClient(url);
            var request = new RestRequest();
            request.Timeout = TimeSpan.FromSeconds(3);
            request.Method = method;
            request.AddHeader("Authorization", token);
            //client.UserAgent = "apifox/1.0.0 (https://www.apifox.cn)";
            request.AddHeader("Content-Type", "application/json");
            string json = JsonConvert.SerializeObject(t);
            //var body = @"{
            //                ""items"":" + json + @"
            //               }";
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            var response = await client.ExecuteAsync(request);
            var responModel = JsonConvert.DeserializeObject<ReturnType>(response.Content);
            return responModel;
            //Console.WriteLine(response.Content);
        }
        /// <summary>
        /// 上传文件 request.AddParameter(par.Key, par.Value);
        /// <code><![CDATA[
        /// 接收参数 public async Task<ResponseUploadImg> UploadImage([FromForm] string base64String)
        /// ]]></code>
        /// </summary>
        /// <returns></returns>
        public static async Task<ReturnType> ExecuteAsync<ReturnType>(string url, Dictionary<string, string> paras, string token = null)
        {
            var client = new RestClient(url);
            var request = new RestRequest();
            request.Timeout = TimeSpan.FromSeconds(3);
            request.Method = Method.Post;
            if (!string.IsNullOrEmpty(token))
                request.AddHeader("Authorization", token);
            //client.UserAgent = "apifox/1.0.0 (https://www.apifox.cn)";
            //request.AddHeader("content-encoding", "gzip");
            //request.AddHeader("Content-Type", " application/x-www-form-urlencoded");
            //request.AddHeader("Cache-Control", "no-cache");
            //request.AddHeader("Content-Type", "application/json");
            /*
            request.AddHeader("Content-Type", "application/json");
            string json = JsonConvert.SerializeObject(base64String);
            var body = @"{
                            ""base64String"":" + base64String + @"
                           }";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            */
            //request.AddParameter("imgFileName", imgFileName);
            //request.AddParameter("imgFileBase64Code", imgFileBase64Code);
            //request.AddParameter("imgFileSaveRelativePath", imgFileSaveRelativePath);
            foreach (var par in paras)
            {
                request.AddParameter(par.Key, par.Value);
            }
            var response = await client.ExecuteAsync(request);
            var responModel = JsonConvert.DeserializeObject<ReturnType>(response.Content);
            return responModel;
            //Console.WriteLine(response.Content);
        }
    }
}
/*
 *url传参 接收参数在方法上变量和参数名同名
 *
 *实体传参放入
 *request.AddHeader("Content-Type", "application/json");
  string json = JsonConvert.SerializeObject(t);
  request.AddParameter("application/json", json, ParameterType.RequestBody);
 *实体传参接收形式 方法上定义对象直接使用
 *
 *request.AddParameter("base64String", imgFileBase64Code);
 *UploadImage([FromForm] string base64String)
 *
 *request.AddQueryParameter("ParamKey", "ParamValue");
 *
 *form-data和x-www-form-urlencoded
 *
 *request.AddParameter("itemName", "itemValue", ParameterType.Cookie);
 */

/*

*/

//上传下载
//request.AddFile("FileKey", @"F:\Demo.txt");	// 添加文件
//byte[] response = client.DownloadData(request);	// 执行请求
//System.IO.File.WriteAllBytes(@"F:\Demo.txt", response);  // 将返回结果保存到文件

//多项cookie
//public void addRequestCookies(ref RestRequest request, string cookie)
//{
//    string[] cookie_items = cookie.Split(';');
//    foreach (string cookie_item in cookie_items)
//    {
//        if (cookie_item.Trim().Length == 0) continue;
//        string cookie_key = cookie_item.Substring(0, cookie_item.IndexOf('=')).Trim();
//        string cookie_value = cookie_item.Substring(cookie_item.IndexOf('=') + 1).Trim();
//        if (cookie_value.Contains(",")) cookie_value = $"\"{cookie_value}\"";
//        request.AddParameter(cookie_key, cookie_value, ParameterType.Cookie);
//    }
//}
//// 调用示例
//addRequestCookies(ref request, "item1=value1;item2=value2")

/*
构建需要提交的JSON数据：{"Name": "zhangsan", "Score": [81, 92, 86]}
JObject post_json = new JObject();
post_json.Add("Name", "zhangsan");
JArray score = new JArray() { 81, 92, 86 };
post_json.Add("Score", score);
// 序列化JSON数据
string post_data = JsonConvert.SerializeObject(post_json);
// 将JSON参数添加至请求中
request.AddParameter("application/json", post_data, ParameterType.RequestBody);

IRestResponse response = client.Execute(request); // 执行请求
string res_text = response.Content; // 文本结果
JObject res_json = (JObject)JsonConvert.DeserializeObject(res_text); // JSON结果
*/