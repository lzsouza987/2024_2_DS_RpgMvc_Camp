using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RpgMvc.Models;

namespace RpgMvc.Controllers
{
    public class PersonagemHabilidadesController : Controller
    {
        public string uriBase = "http://luizfernando.somee.com/RpgApi/PersonagemHabilidades/";
        //xyz será substituído pelo nome do seu site na API.        

        [HttpGet("PersonagemHabilidades/{id}")]
        public async Task<ActionResult> IndexAsync(int id)
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
                    List<PersonagemHabilidadeViewModel> lista = await Task.Run(() =>
                        JsonConvert.DeserializeObject<List<PersonagemHabilidadeViewModel>>(serialized));

                    return View(lista);
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

        [HttpGet("Delete/{habilidadeId}/{personagemId}")]
        public async Task<ActionResult> DeleteAsync(int habilidadeId, int personagemId)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string uriComplentar = "DeletePersonagemHabilidade";
                string token = HttpContext.Session.GetString("SessionTokenUsuario");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                PersonagemHabilidadeViewModel ph = new PersonagemHabilidadeViewModel();
                ph.HabilidadeId = habilidadeId;
                ph.PersonagemId = personagemId;

                var content = new StringContent(JsonConvert.SerializeObject(ph));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(uriBase + uriComplentar, content);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    TempData["Mensagem"] = "Habilidade removida com sucesso";
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
            }
            return RedirectToAction("Index", new { Id = personagemId });
        }

        [HttpGet]
        public async Task<ActionResult> CreateAsync(int id, string nome)
        {
            try
            {
                string token = HttpContext.Session.GetString("SessionTokenUsuario");

                string uriComplentar = "GetHabilidades";
                HttpClient httpClient = new HttpClient();
                
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.GetAsync(uriBase + uriComplentar);

                string serialized = await response.Content.ReadAsStringAsync();
                List<HabilidadeViewModel> habilidades = await Task.Run(() =>
                JsonConvert.DeserializeObject<List<HabilidadeViewModel>>(serialized));
                ViewBag.ListaHabilidades = habilidades;

                PersonagemHabilidadeViewModel ph = new PersonagemHabilidadeViewModel();
                ph.Personagem = new PersonagemViewModel();
                ph.Habilidade = new HabilidadeViewModel();
                ph.PersonagemId = id;
                ph.Personagem.Nome = nome;

                return View(ph);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Create", new { id, nome });
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(PersonagemHabilidadeViewModel ph)
        {
            try
            {
                /*if(ph.Habilidade.Dano > 20)
                    throw new System.Exception("Dano não pode ser maior que 20");*/

                string token = HttpContext.Session.GetString("SessionTokenUsuario");

                HttpClient httpClientHabilidades = new HttpClient();                
                httpClientHabilidades.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage responseHabilidades = await httpClientHabilidades.GetAsync(uriBase + ph.PersonagemId.ToString());
                string serializedHabilidade = await responseHabilidades.Content.ReadAsStringAsync();

                if (responseHabilidades.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<PersonagemHabilidadeViewModel> lista = await Task.Run(() =>
                        JsonConvert.DeserializeObject<List<PersonagemHabilidadeViewModel>>(serializedHabilidade));

                    if(lista.Count == 3)
                        throw new System.Exception("O Personagem só pode possuir três habilidades.");
                }

                HttpClient httpClient = new HttpClient();                
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent(JsonConvert.SerializeObject(ph));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await httpClient.PostAsync(uriBase, content);
                string serialized = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)                
                    TempData["Mensagem"] = "Habilidade cadastrada com sucesso";                                    
                else
                    throw new System.Exception(serialized);
            }
            catch (System.Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
            }
            return RedirectToAction("Index", new { id = ph.PersonagemId});
        }









    }
}