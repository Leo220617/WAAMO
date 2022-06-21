using CheckIn.API.Models;
using CheckIn.API.Models.ModelCliente;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CheckIn.API.Controllers
{
    [Authorize]
    public class SeguridadRolesModulosController: ApiController
    {
        ModelCliente db = new ModelCliente();
        G G = new G();

        public async Task<HttpResponseMessage> GetModulos([FromUri] Filtros filtro)
        {
            try
            {
                 
                var modulos = db.SeguridadRolesModulos.ToList();


                if (filtro.Codigo1 > 0)
                {
                    modulos = modulos.Where(a => a.CodRol == filtro.Codigo1).ToList();
                }
                return Request.CreateResponse(HttpStatusCode.OK, modulos);

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody] SeguridadRolesModulos[] objeto)
        {
            
            var t = db.Database.BeginTransaction();
            try
            {
                var primero = objeto[0].CodRol;
                var rolesModulos = db.SeguridadRolesModulos.Where(a => a.CodRol == primero).ToList();
                foreach (var item in rolesModulos)
                {
                    var Objeto = db.SeguridadRolesModulos.Where(a => a.CodRol == item.CodRol && a.CodModulo == item.CodModulo).FirstOrDefault();

                    if (Objeto != null)
                    {
                        db.SeguridadRolesModulos.Remove(Objeto);
                        db.SaveChanges();
                    }
                }

                foreach (var item in objeto)
                {



                    var Objeto = db.SeguridadRolesModulos.Where(a => a.CodRol == item.CodRol && a.CodModulo == item.CodModulo).FirstOrDefault();

                    if (Objeto == null)
                    {
                        var Objetos = new SeguridadRolesModulos();
                        Objetos.CodRol = item.CodRol;
                        Objetos.CodModulo = item.CodModulo;


                        db.SeguridadRolesModulos.Add(Objetos);
                        db.SaveChanges();

                    }


                }
                t.Commit();
                return Request.CreateResponse(HttpStatusCode.OK, objeto);
            }
            catch (Exception ex)
            {
                t.Rollback();
                BE bitacora = new BE();
                bitacora.Descripcion = ex.Message.Substring(0, 5000);
                bitacora.StackTrace = ex.StackTrace.ToString();
                bitacora.Fecha = DateTime.Now;
                db.BE.Add(bitacora);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

    }
}