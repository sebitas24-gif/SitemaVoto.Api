using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VotoModelos;
using Voto.ApiConsumer;

namespace VotoMVC.Controllers
{
    public class VotantesController : Controller
    {
        // GET: VotantesController
        public VotantesController()
        {
            Voto.ApiConsumer.Crud<VotoModelos.Votante>.UrlBase = "https://10.241.253.223:5203/api/Votantes";
        }
        public ActionResult Index()
        {
           var apiResult = Crud<Votante>.ReadAll();
           var modelo =apiResult?.Data?? new List<Votante>();
            return View(modelo);
        }

        // GET: VotantesController/Details/5
        public ActionResult Details(int id)
        {
            
            return View();
        }

        // GET: VotantesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VotantesController/Create
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

        // GET: VotantesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: VotantesController/Edit/5
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

        // GET: VotantesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VotantesController/Delete/5
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
