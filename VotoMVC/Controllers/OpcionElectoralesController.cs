using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VotoModelos;

namespace VotoMVC.Controllers
{
    public class OpcionElectoralesController : Controller
    {
        public OpcionElectoralesController()
        {
            Voto.ApiConsumer.Crud<VotoModelos.OpcionElectoral>.UrlBase = "http://10.241.253.223:5208/api/OpcionElectorales";
        }
        // GET: OpcionElectoralesController
        public ActionResult Index()
        {
            var apiResult = Voto.ApiConsumer.Crud<OpcionElectoral>.ReadAll();
            var modelo = apiResult?.Data ?? new List<OpcionElectoral>();
            return View(modelo);
        }

        // GET: OpcionElectoralesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: OpcionElectoralesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OpcionElectoralesController/Create
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

        // GET: OpcionElectoralesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OpcionElectoralesController/Edit/5
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

        // GET: OpcionElectoralesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OpcionElectoralesController/Delete/5
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
