using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RpgMvc.Models;

namespace RpgMvc.Controllers
{
    public class PersonagensController : Controller
    {
        public string uriBase = "http://luizsouza.somee.com/RpgApi/Personagens/";

        [HttpGet]
        public async Task<ActionResult> IndexAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("SessionIdUsuario")))
                {
                    return RedirectToAction("Sair", "Usuarios");
                }
                string perfil = HttpContext.Session.GetString("SessionPerfilUsuario");
                ViewBag.Perfil = perfil;

                //string uriComplementar = "GetAll";
                int usuarioId = int.Parse(HttpContext.Session.GetString("SessionIdUsuario"));
                string uriComplementar = (perfil == "Admin") ? "GetAll" : $"GetByPerfil/{usuarioId}";


                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await httpClient.GetAsync(uriBase + uriComplementar);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<PersonagemViewModel> listaPersonagens = await Task.Run(() => JsonConvert.DeserializeObject<List<PersonagemViewModel>>(serialized));

                    return View(listaPersonagens);
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
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(PersonagemViewModel p)
        {
            try
            {
                if (p.Forca > 20 || p.Inteligencia > 20 || p.Defesa > 20)
                    throw new Exception("Força, defesa e inteligência não podem ser maior que 20");

                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                if (string.IsNullOrEmpty(token))
                {
                    int? usuarioId = int.Parse(HttpContext.Session.GetString("SessionIdUsuario"));
                    p.UsuarioId = usuarioId;
                }

                var content = new StringContent(JsonConvert.SerializeObject(p));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(uriBase, content);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] = string.Format("Personagem {0}, Id {1} salvo com sucesso!", p.Nome, serialized);
                    return RedirectToAction("Index");
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Create");
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
                    PersonagemViewModel p = await Task.Run(() =>
                    JsonConvert.DeserializeObject<PersonagemViewModel>(serialized));
                    return View(p);
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
                string token = HttpContext.Session.GetString("SessionTokenUsuario");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.GetAsync(uriBase + id.ToString());

                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    PersonagemViewModel p = await Task.Run(() =>
                    JsonConvert.DeserializeObject<PersonagemViewModel>(serialized));
                    return View(p);
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
        public async Task<ActionResult> EditAsync(PersonagemViewModel p)
        {
            try
            {
                if (p.Forca > 20 || p.Inteligencia > 20 || p.Defesa > 20)
                    throw new Exception("Força, defesa e inteligência não podem ser maior que 20");

                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var content = new StringContent(JsonConvert.SerializeObject(p));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await httpClient.PutAsync(uriBase, content);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] =
                        string.Format("Personagem {0}, classe {1} atualizado com sucesso!", p.Nome, p.Classe);

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
                    TempData["Mensagem"] = string.Format("Personagem Id {0} removido com sucesso!", id);
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
        public async Task<ActionResult> ZerarRankingRestaurarVidasAsync()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                string uriComplementar = "ZerarRankingRestaurarVidas";
                HttpResponseMessage response = await httpClient.PutAsync(uriBase + uriComplementar, null);
                string serialized = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    TempData["Mensagem"] = "Rankings zerados e vidas dos personagens restauradas com sucesso.";
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            { TempData["MensagemErro"] = ex.Message; }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<ActionResult> RestaurarPontosVidaAsync(int id)
        {
            try
            {
                string uriComplementar = "RestaurarPontosVida";
                PersonagemViewModel p = new PersonagemViewModel();
                p.Id = id;

                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var content = new StringContent(JsonConvert.SerializeObject(p));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await httpClient.PutAsync(uriBase + uriComplementar, content);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] = "Pontos de vida do personagem restaurados com sucesso";
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> RestaurarPontosVidaGeralAsync()
        {
            try
            {
                HttpClient httpClient = new HttpClient();

                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string uriBuscaPersonagens = "http://luizsouza.somee.com/RpgApi/Personagens/GetAll";

                HttpResponseMessage response = await httpClient.GetAsync(uriBuscaPersonagens);

                string serialized = await response.Content.ReadAsStringAsync();

                List<PersonagemViewModel> listaPersonagens = await Task.Run(() =>
                    JsonConvert.DeserializeObject<List<PersonagemViewModel>>(serialized));

                string mensagemPersonagem = string.Empty;
                foreach (PersonagemViewModel p in listaPersonagens)
                {
                    string uriComplementarPersonagem = "RestaurarPontosVida";
                    HttpClient httpClientPersonagem = new HttpClient();
                    httpClientPersonagem.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var contentPersonagem = new StringContent(JsonConvert.SerializeObject(p));
                    contentPersonagem.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage responsePersonagem = await httpClient.PutAsync(uriBase + uriComplementarPersonagem, contentPersonagem);
                    string serializedPersonagem = await response.Content.ReadAsStringAsync();

                    
                    if (responsePersonagem.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        mensagemPersonagem += $"Pontos de vida do personagem {p.Nome} restaurados com sucesso.<br/>";
                    }
                    else
                        throw new System.Exception(serialized);
                }
                
                TempData["Mensagem"] = mensagemPersonagem;
                return RedirectToAction("Index", "Personagens");
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index", "Personagens");
            }
        }



        [HttpGet]
        public async Task<ActionResult> ZerarRankingAsync(int id)
        {
            try
            {
                string uriComplementar = "ZerarRanking";
                PersonagemViewModel p = new PersonagemViewModel();
                p.Id = id;
                p.Nome = string.Empty;

                HttpClient httpClient = new HttpClient();
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var content = new StringContent(JsonConvert.SerializeObject(p));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await httpClient.PutAsync(uriBase + uriComplementar, content);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["Mensagem"] = "Ranking do personagem zerado com sucesso";
                }
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
            }
            return RedirectToAction("Index");
        }







    }
}