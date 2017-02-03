using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using KendoStudent.Models;
using System.IO;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.Collections;

namespace KendoStudent.Controllers
{
    public class StudentsController : Controller
    {
        private DDStudent db = new DDStudent();
        string  baseUrl = "http://localhost:51514/api/Students";

        public async System.Threading.Tasks.Task<ActionResult> Index()
        {          
            var table = new List<StudentViewModel>();
            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                System.Net.Http.HttpResponseMessage response = await client.GetAsync(baseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync();
                    table = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StudentViewModel>>(data.Result);
                }
            }

            return View(table.AsEnumerable());
        }

        public async System.Threading.Tasks.Task<ActionResult> StudentViewModels_Read([DataSourceRequest]DataSourceRequest request)
        {
            var uri = baseUrl +"?page="+request.Page+"&pageSize="+request.PageSize;
            var table = new List<StudentViewModel>();
            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri(uri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                System.Net.Http.HttpResponseMessage response = await client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync();
                    table = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StudentViewModel>>(data.Result);
                }
            }

            return Json(table);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async System.Threading.Tasks.Task<ActionResult> StudentViewModels_Create([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<StudentViewModel> studentviewmodels)
        {
            var table = new List<StudentViewModel>();
            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                ArrayList paramList = new ArrayList();
                paramList.Add(JsonConvert.SerializeObject(request));
                foreach (StudentViewModel student in studentviewmodels)
                {
                    paramList.Add(JsonConvert.SerializeObject(student));
                }

                var serializer = new JavaScriptSerializer();
                var json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(paramList);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                //System.Net.Http.HttpResponseMessage response1 = await client.PostAsync(baseUrl, new StringContent(new JavaScriptSerializer().Serialize(paramList), Encoding.UTF8, "application/json"));
                System.Net.Http.HttpResponseMessage response =   await client.PostAsync(baseUrl, stringContent);
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync();
                    table = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StudentViewModel>>(data.Result);
                }
            }

            if (studentviewmodels != null && ModelState.IsValid)
            {
            }

                // this will return new data
                return Json(table.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public async System.Threading.Tasks.Task<ActionResult>  StudentViewModels_Update([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<StudentViewModel> studentviewmodels)
        {
            var table = new List<StudentViewModel>();
            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                ArrayList paramList = new ArrayList();
                paramList.Add(JsonConvert.SerializeObject(request));
                foreach (StudentViewModel student in studentviewmodels)
                {
                    paramList.Add(JsonConvert.SerializeObject(student));
                }

                System.Net.Http.HttpResponseMessage response = await client.PutAsync(baseUrl, new StringContent(new JavaScriptSerializer().Serialize(paramList), Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync();
                    table = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StudentViewModel>>(data.Result);
                }
            }

            if (studentviewmodels != null && ModelState.IsValid)
            {
                // it is so ignore for now
            }
            // this will return new data
            return Json(table.ToDataSourceResult(request, ModelState));
        } 

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StudentViewModels_Destroy([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<StudentViewModel> studentviewmodels)
        {
            var entities = new List<StudentViewModel>();
            if (ModelState.IsValid)
            {
                foreach(var studentViewModel in studentviewmodels)
                {
                    var entity = new StudentViewModel
                    {
                        Id = studentViewModel.Id,
                        Studentid = studentViewModel.Studentid,
                        Firstname = studentViewModel.Firstname,
                        Lastname = studentViewModel.Lastname,
                        Age = studentViewModel.Age
                    };

                    entities.Add(entity);
                    db.StudentViewModels.Attach(entity);
                    db.StudentViewModels.Remove(entity);
                }
                db.SaveChanges();
            }

            return Json(entities.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
