

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RpgMvc.Models;

namespace RpgMvc.Controllers
{
    public class ArmasController : Controller
    {
        public string uriBase = "http://luizfernando.somee.com/RpgApi/Armas/";

        [HttpGet]
        public async Task<ActionResult> IndexAsync()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                string perfil = HttpContext.Session.GetString("SessionPerfilUsuario");
                string uriComplementar = "GetAll";


                HttpResponseMessage response = await httpClient.GetAsync(uriBase + uriComplementar);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<ArmaViewModel> listaArmas = await Task.Run(() =>
                        JsonConvert.DeserializeObject<List<ArmaViewModel>>(serialized));


                    if (!perfil.Equals("Admin"))
                    {
                        List<ArmaViewModel> avm = new List<ArmaViewModel>();

                        foreach (ArmaViewModel av in listaArmas)
                        {
                            int usuarioId = int.Parse(HttpContext.Session.GetString("SessionIdUsuario"));
                            string uriBuscaPersonagens = $"http://luizfernando.somee.com/RpgApi/Personagens/{av.PersonagemId}";

                            HttpResponseMessage responsePersonagens = await httpClient.GetAsync(uriBuscaPersonagens);
                            
                            if (responsePersonagens.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                string serializedPersonagens = await responsePersonagens.Content.ReadAsStringAsync();

                                PersonagemViewModel personagem = await Task.Run(() =>
                                JsonConvert.DeserializeObject<PersonagemViewModel>(serializedPersonagens));

                                //if (personagem.Usuario.Id == usuarioId)
                                if (personagem.UsuarioId == usuarioId)
                                    avm.Add(av);
                            }
                        }
                        listaArmas = avm;
                    }

                    return View(listaArmas);
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index", "Personsagens");
            }
        }

        [HttpGet]
        public async Task<ActionResult> CreateAsync(int? id)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                int usuarioId = int.Parse(HttpContext.Session.GetString("SessionIdUsuario"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);



                string perfil = HttpContext.Session.GetString("SessionPerfilUsuario");
                //string uriBuscaPersonagens = (perfil == "Admin") ? "https://apirpg.azurewebsites.net/Personagens/GetAll" : $"https://apirpg.azurewebsites.net/Personagens/GetByPerfil/{usuarioId}";
                string uriBuscaPersonagens = (perfil == "Admin") ? "http://luizfernando.somee.com/RpgApi/Personagens/GetAll" : $"http://luizfernando.somee.com/RpgApi/Personagens/GetByPerfil/{usuarioId}";

                HttpResponseMessage response = await httpClient.GetAsync(uriBuscaPersonagens);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<PersonagemViewModel> listaPersonagens = await Task.Run(() =>
                    JsonConvert.DeserializeObject<List<PersonagemViewModel>>(serialized));
                    ViewBag.ListaPersonagens = listaPersonagens;
                }
                else
                    throw new System.Exception(serialized);

                return View();
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return View("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(ArmaViewModel a)
        {
            try
            {
                if (a.Dano < 30 || a.Dano > 35)
                {
                    throw new System.Exception("O dano da arma deve estar entre 30 e 35");
                }

                a.Personagem = null;


                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent(JsonConvert.SerializeObject(a));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(uriBase, content);
                string serialized = await response.Content.ReadAsStringAsync();



                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] = string.Format("Arma {0}, Id {1} salvo com sucesso!", a.Nome, serialized);
                    return RedirectToAction("Index");
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<ActionResult> DetailsAsync(int? id)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.GetAsync(uriBase + id.ToString());
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ArmaViewModel a = await Task.Run(() =>
                    JsonConvert.DeserializeObject<ArmaViewModel>(serialized));
                    return View(a);
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        public async Task<ActionResult> EditAsync(int? id)
        {
            try
            {
                HttpClient httpClient = new HttpClient();

                //Busca de Personagens
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);



                int usuarioId = int.Parse(HttpContext.Session.GetString("SessionIdUsuario"));
                string perfil = HttpContext.Session.GetString("SessionPerfilUsuario");

                //string uriBuscaPersonagens = (perfil == "Admin") ? "https://apirpg.azurewebsites.net/Personagens/GetAll" : "https://apirpg.azurewebsites.net/Personagens/GetByUser";
                string uriBuscaPersonagens = (perfil == "Admin") ? "http://luizfernando.somee.com/RpgApi/Personagens/GetAll" : $"http://luizfernando.somee.com/RpgApi/Personagens/GetByPerfil/{usuarioId}";

                HttpResponseMessage responsePersonagem = await httpClient.GetAsync(uriBuscaPersonagens);
                string serializedPersonagem = await responsePersonagem.Content.ReadAsStringAsync();

                if (responsePersonagem.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<PersonagemViewModel> listaPersonagens = await Task.Run(() =>
                        JsonConvert.DeserializeObject<List<PersonagemViewModel>>(serializedPersonagem));

                    ViewBag.ListaPersonagens = listaPersonagens;
                }
                else
                    throw new System.Exception(serializedPersonagem);
                //Fim da busca de Personagens


                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.GetAsync(uriBase + id.ToString());
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ArmaViewModel a = await Task.Run(() =>
                    JsonConvert.DeserializeObject<ArmaViewModel>(serialized));
                    return View(a);
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditAsync(ArmaViewModel a)
        {
            try
            {
                if (a.Dano < 30 || a.Dano > 35)
                {
                    throw new System.Exception("O dano da arma deve estar entre 30 e 35");
                }

                a.Personagem = null;
                
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var content = new StringContent(JsonConvert.SerializeObject(a));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await httpClient.PutAsync(uriBase, content);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] =
                        string.Format("Arma {0}, Id {1} atualizada com sucesso!", a.Nome, a.Id);

                    return RedirectToAction("Index");
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await httpClient.DeleteAsync(uriBase + id.ToString());
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] = string.Format("Arma Id {0} removida com sucesso!", id);
                    return RedirectToAction("Index");
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }



    }
}