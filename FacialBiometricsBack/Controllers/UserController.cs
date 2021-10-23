﻿using FacialBiometrics.Models;
using FacialBiometricsBack.Models;
using FacialBiometricsBack.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FacialBiometricsBack.Controllers
{
    [Route("User")]
    [ApiController]
    public class UserController : Controller
    {
        private IFacialBiometricsServices _facialBiometricsService;

        public UserController(IFacialBiometricsServices facialBiometricsServices)
        {
            _facialBiometricsService = facialBiometricsServices;
        }

        [HttpGet]
        public JsonResult GetTeste()
        {
            var dados = new
            {
                message = "Teste de api"
            };            

            return Json(dados);
        }

        [HttpPost("Register")]
        public JsonResult CadastroUser(UserFrontModel dadosUser)
        {
            if (ModelState.IsValid)
            {
                if (dadosUser.face_images.Count()==0)
                {
                    return Json(new { message = "Nenhuma imagem recebida",statusCode=HttpStatusCode.BadRequest });
                }

                List<UserFaceImg> imageDados = new List<UserFaceImg>();

                foreach(var img in dadosUser.face_images)
                {
                    string[] imgDados = img.Split(',');

                    imageDados.Add(new UserFaceImg
                    {
                        metaData = imgDados[0],
                        extension = imgDados[0].Split(';')[0].Split('/')[1],
                        imageBytes = Convert.FromBase64String(imgDados[1])
                    });

                }


                int idUser = _facialBiometricsService.CreateUser(new UserInfo{
                    NameUser = dadosUser.name,
                    Username = dadosUser.username,
                    Password = dadosUser.password,
                    SaltPassword = dadosUser.salt_password,
                    UserPositionInfo = new UserPosition{ IdUserPosition = dadosUser.id_user_position}
                });

                foreach(var faceImg in imageDados){
                    _facialBiometricsService.CreateFacialBiometrics(new UsersFacialBiometrics{
                        ImageName = Guid.NewGuid().ToString(),
                        ImageBytes = faceImg.imageBytes,
                        IdUser = idUser
                    });
                }

                return Json(new { message = "Cadastrado com sucesso", statusCode = HttpStatusCode.OK });
            }
            else
            {
                return Json(new { message = "Cadastro Inválido", statusCode = HttpStatusCode.BadRequest });
            }

        }

        [HttpPost("validate")]
        public JsonResult validateLogin(string username, string password, List<string> face_images){
            //Se login inválido retorna false
            //return Json(new { isValid = true});
            Console.WriteLine($">validateLogin: username({username}), password({password}), face_images({String.Join(",",face_images)})");
            //Inserir validação das imagens enviadas do rosto
            
            return Json(new { isValid = true, levelAccess = 1});
        }

        [HttpGet("articles")]
        public JsonResult getArticles(int idUser){
            //SELECT * FROM ARTIGOS A WHERE A.NIVEL_ACESSO IN (SELECT U.LEVEL FROM USUARIO U WHERE U.ID = ID_USER)
            //Fazer query no banco que busca os artigos liberados pro nível do idUser passado

            Console.WriteLine(">getArticles: idUser("+idUser+")");
            
            List<ArticleModel> list_articles = new List<ArticleModel>();
            list_articles.Add(new ArticleModel{
                idArticle = 1,
                title = "Teste de título 1",
                content = "Conteúdo teste aiodhi9asd io ",
            });
            list_articles.Add(new ArticleModel{
                idArticle = 2,
                title = "Teste de título 2",
                content = "Conteúdo teste aiodhi9asd io ",
            });
            list_articles.Add(new ArticleModel{
                idArticle = 3 ,
                title = "Teste de título 3",
                content = "Conteúdo teste aiodhi9asd io ",
            });

            return Json(new { listArticles = list_articles });
        }

    }
}
