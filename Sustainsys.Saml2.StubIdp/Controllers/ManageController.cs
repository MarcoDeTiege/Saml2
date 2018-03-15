using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Sustainsys.Saml2.StubIdp.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Sustainsys.Saml2.StubIdp.Controllers
{
    public class ManageController : BaseController
    {
        public ActionResult Index(Guid idpId)
        {
            var fileName = GetIdpFileNamePath(idpId);
            var model = new ManageIdpModel();
            if (System.IO.File.Exists(fileName))
            {
                model.JsonData = System.IO.File.ReadAllText(fileName);
            }
            else
            {
                var defaultData = System.IO.File.ReadAllText(GetIdpFileNamePath(defaultIdpGuid));
                var defaultJson = JObject.Parse(defaultData);
                defaultJson["DefaultAssertionConsumerServiceUrl"] = "http://www.example.com/Saml2/Acs (optional, you may remove this line)";
                defaultJson["DefaultAudience"] = "http://www.example.com/Saml2 (optional, but usually a good idea to set to Entity ID of SP)";
                defaultJson["IdpDescription"] = "This is my custom IDP description";
                model.JsonData = defaultJson.ToString(Newtonsoft.Json.Formatting.Indented);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(Guid idpId, ManageIdpModel model)
        {
            if (idpId == defaultIdpGuid)
            {
                ModelState.AddModelError("", "Can't update default model");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var schema = JSchema.Parse(System.IO.File.ReadAllText(Server.MapPath("~/Content/IdpConfigurationSchema.json")));

            JObject parsedJson;
            try
            {
                parsedJson = JObject.Parse(model.JsonData);
            }
            catch (Exception)
            {
                ModelState.AddModelError("JsonData", "Invalid Json");
                return View(model);
            }
            IList<string> errorMessages;
            if (!parsedJson.IsValid(schema, out errorMessages))
            {
                ModelState.AddModelError("JsonData", "Json does not match schema. " + string.Join(" ", errorMessages));
                return View(model);
            }

            var fileName = GetIdpFileNamePath(idpId);

            model.JsonData = parsedJson.ToString(Newtonsoft.Json.Formatting.Indented);

            System.IO.File.WriteAllText(fileName, model.JsonData);

            cachedConfigurations.AddOrUpdate(idpId, new IdpConfigurationModel(model.JsonData), (_, __) => new IdpConfigurationModel(model.JsonData));

            return RedirectToAction("Index");
        }

        [Compress]
        public ActionResult CurrentConfiguration(Guid? idpId)
        {
            var fileData = GetCachedConfiguration(idpId.GetValueOrDefault(defaultIdpGuid));
            if (fileData == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Internal server error, no IDP configured");
            }
            return TestETag(fileData.JsonData, fileData.ETag, "application/json");
        }
    }
}