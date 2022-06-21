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
using System.Web.Http.Cors;
using System.Web.UI.WebControls;
using Login = CheckIn.API.Models.ModelCliente.Login;

namespace CheckIn.API.Controllers
{
    [EnableCors("*", "*", "*")]
    public class LoginController: ApiController
    {
        ModelCliente db = new ModelCliente();
 
        G G = new G();

        [Route("api/Login/Conectar")]
        public async Task<HttpResponseMessage> GetLoginAsync([FromUri] string email, string clave )
        {
            try
            {
                var Usuario = db.Login.Where(a => a.Email.ToLower().Contains(email.ToLower())).FirstOrDefault();
                if(Usuario == null)
                {
                    throw new Exception("Clave o Usuario incorrectos");
                }

               if(! BCrypt.Net.BCrypt.Verify(clave, Usuario.Clave))
                {
                    throw new Exception("Clave o Usuario incorrectos");
                }


                
                var token = TokenGenerator.GenerateTokenJwt(Usuario.Clave, Usuario.id.ToString());

                DevolucionLogin de = new DevolucionLogin();
                 

                var SeguridadModulos = db.SeguridadRolesModulos.Where(a => a.CodRol == Usuario.idRol).ToList();
             


                de.idLogin = Usuario.id ;
                de.NombreUsuario = Usuario.Nombre;
                de.Email = Usuario.Email;
                de.token = token;
                de.idRol = Usuario.idRol.Value;
                de.Seguridad = SeguridadModulos;               
                de.CambiarClave = Usuario.CambiarClave.Value;
                return Request.CreateResponse(HttpStatusCode.OK, de);

            }
            catch (Exception ex)
            {
                BE bitacora = new BE();
                bitacora.Descripcion = ex.Message.Substring(0, 5000);
                bitacora.StackTrace = ex.StackTrace.ToString();
                bitacora.Fecha = DateTime.Now;
                db.BE.Add(bitacora);
                db.SaveChanges();
                G.GuardarTxt("ErrorLogin.txt", ex.Message + " => " + ex.StackTrace);
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        public async Task<HttpResponseMessage> Get([FromUri] Filtros filtro)
        {
            try
            {
                 
                var Login = db.Login.ToList();

                if (!string.IsNullOrEmpty(filtro.Texto))
                {
                    Login = Login.Where(a => a.Nombre.ToUpper().Contains(filtro.Texto.ToUpper()) || a.Email.ToUpper().Contains(filtro.Texto.ToUpper())).ToList();
                }
                

             
                return Request.CreateResponse(HttpStatusCode.OK, Login);

            }
            catch (Exception ex)
            {
                BE bitacora = new BE();
                bitacora.Descripcion = ex.Message.Substring(0, 5000);
                bitacora.StackTrace = ex.StackTrace.ToString();
                bitacora.Fecha = DateTime.Now;
                db.BE.Add(bitacora);
                db.SaveChanges();

        
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        [Route("api/Login/Consultar")]
        public async Task<HttpResponseMessage> GetOne([FromUri] int id)
        {
            try
            {
                 
                var Login = db.Login.Where(a => a.id == id).FirstOrDefault();

                if(Login == null)
                {
                    throw new Exception("Usuario no existe");
                }


            
                return Request.CreateResponse(HttpStatusCode.OK, Login);

            }
            catch (Exception ex)
            {
                BE bitacora = new BE();
                bitacora.Descripcion = ex.Message.Substring(0, 5000);
                bitacora.StackTrace = ex.StackTrace.ToString();
                bitacora.Fecha = DateTime.Now;
                db.BE.Add(bitacora);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody] Login usuario)
        {
             
            var t = db.Database.BeginTransaction();
            
            try
            {

                 
                var User = db.Login.Where(a => a.Email.ToUpper().Contains(usuario.Email.ToUpper()) && a.Activo == true).FirstOrDefault();
                if (User == null)
                {
                    

                    Login login = new Login();
                    login.Nombre = User.Nombre;
                    login.Clave = User.Clave;
                    login.Activo = true;
                    login.idRol = usuario.idRol;
                    login.Email = User.Email;
                     
                    login.CambiarClave = true;
           
                    db.Login.Add(login);

                    
                    t.Commit();
                }
                else
                {
                    throw new Exception("Este usuario YA existe");
                }

                
                return Request.CreateResponse(HttpStatusCode.OK);
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

        //Actualiza la contraseña del usuario 
        [HttpPut]
        [Route("api/Login/Actualizar")]
        public HttpResponseMessage Put([FromBody] Login usuario)
        {
            try
            {
                

                var User = db.Login.Where(a => a.id == usuario.id).FirstOrDefault();  
                

                if (  User != null)
                {
                     
                    db.Entry(User).State = EntityState.Modified;

                    if (!string.IsNullOrEmpty(usuario.Clave))
                    {

                        User.Clave = BCrypt.Net.BCrypt.HashPassword(usuario.Clave);
                        
                        User.CambiarClave = false;
                    }
       
                    if (!string.IsNullOrEmpty(usuario.Nombre))
                    {
                        User.Nombre = usuario.Nombre;
                       
                    }

                    if (!string.IsNullOrEmpty(usuario.Email))
                    {
                        User.Email = usuario.Email;
                        
                    }

                    if (usuario.idRol > 0)
                    {
                        User.idRol = usuario.idRol;
                    }
                   
                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("Usuario no existe");
                }
                 
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {

                BE bitacora = new BE();
                bitacora.Descripcion = ex.Message.Substring(0, 5000);
                bitacora.StackTrace = ex.StackTrace.ToString();
                bitacora.Fecha = DateTime.Now;
                db.BE.Add(bitacora);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpDelete]
        [Route("api/Login/Eliminar")]
        public HttpResponseMessage Delete([FromUri] int id, string CedulaJuridica)
        {
            try
            {
               

                var Usuario = db.Login.Where(a => a.id == id).FirstOrDefault();
             
                if ( Usuario != null)
                {

                    db.Entry(User).State = EntityState.Modified;
                  

                    if(Usuario.Activo.Value)
                    {
                        Usuario.Activo = false;
                         
                    }
                    else
                    {
                        Usuario.Activo = true;
                       
                    }



                    
                    db.SaveChanges();

                }
                else
                {
                    throw new Exception("Usuario no existe");
                }
    
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {


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

    internal class DevolucionLogin
    {
        public DevolucionLogin()
        {
        }
        public int idLogin { get; set; }
        public string NombreUsuario { get; set; }
 
        public string Email { get; set; }
 
        public int idRol { get; set; }
        public string token { get; set; }
 
        public bool CambiarClave { get; set; }
        public List<SeguridadRolesModulos> Seguridad { get; set; }
    }
}