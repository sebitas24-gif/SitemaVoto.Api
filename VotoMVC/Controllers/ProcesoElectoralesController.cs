using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VotoModelos;

namespace VotoMVC.Controllers
{
    public class ProcesoElectoralesController : Controller
    {
        public ProcesoElectoralesController()
        {
            Voto.ApiConsumer.Crud<VotoModelos.ProcesoElectoral>.UrlBase = "http://10.241.253.223:5208/api/ProcesoElectorales";
        }
        // GET: ProcesoElectoralesController
        public ActionResult Index()
        {
            var apiResult = Voto.ApiConsumer.Crud<ProcesoElectoral>.ReadAll();
            var modelo = apiResult?.Data ?? new List<ProcesoElectoral>();
            return View(modelo);
        }

        // GET: ProcesoElectoralesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProcesoElectoralesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProcesoElectoralesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProcesoElectoralesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProcesoElectoralesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProcesoElectoralesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProcesoElectoralesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
